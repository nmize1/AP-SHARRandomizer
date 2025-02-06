using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Structs;
using SHARRandomizer.Classes;
using static System.Net.Mime.MediaTypeNames;

namespace SHARRandomizer
{
    class MemoryManip
    {
        Process? p;
        public static AwaitableQueue<string> itemsReceived = new AwaitableQueue<string>();
        List<Reward> REWARDS = new List<Reward>();

        List<string> UnlockedLevels = new List<string>();
        List<string> UnlockedItems = new List<string>();

        FeLanguage language = null;
        bool[][] cards = new bool[7][];
        string[][] missionTitles = new string[7][];

        public async Task MemoryStart()
        {
            Console.WriteLine("Waiting for SHAR process...", "Main");
            while (true)
            {
                do
                {
                    p = Memory.GetSHARProcess();
                } while (p == null);

                Console.WriteLine("Found SHAR process. Initialising memory manager...", "Main");

                Memory memory = new(p);
                Console.WriteLine($"SHAR memory manager initialised. Game version detected: {memory.GameVersion}. Language: {memory.GameSubVersion}.", "Main");

                InitialGameState(memory);
                await GetItems(memory);

                memory.Dispose();
                p.Dispose();
                Console.WriteLine("SHAR closed. Waiting for SHAR process...", "Main");
            }
        }

        /* Setting things up */
        public void InitialGameState(Memory memory)
        {
            Console.WriteLine("Setting up initial game state.", "Main");
            var rewardsManager = memory.Singletons.RewardsManager;
            if (rewardsManager == null)
            {
                Console.WriteLine("Error setting up initial game state. Things will not work correctly.", "Main");
                return;
            }

            while (language == null)
            {
                language = memory.Globals?.TextBible?.CurrentLanguage;
            }

            /* Get all rewards in a list for lookup purposes */
            int i = 0;
            foreach (var rewards in rewardsManager.RewardsList)
            {
                List<Reward> tempRewards = new List<Reward>();
                if (i < 7)
                {
                    tempRewards.Add(rewards.DefaultCar);
                    tempRewards.Add(rewards.BonusMission);
                    tempRewards.Add(rewards.StreetRace);

                    foreach (var reward in rewardsManager.LevelTokenStoreList[i].InventoryList.ToArray())
                    {
                        if (reward.RewardType != Reward.RewardTypes.Null && reward.Name != "Null")
                        {
                            tempRewards.Add(reward);
                        }
                    }
                    i++;
                }
                REWARDS.AddRange(tempRewards);
            }

            /* Lock all default cars since they unlock on level load */
            LockDefaultCarsOnLoad(memory);

            /* Change all mission titles to "Locked" */
            for (i = 0; i < 7; i++)
                missionTitles[i] = new string[7];

            string name = $"MISSION_TITLE_L{0}_M{0}";
            language.SetString(name, "LOCKED");
            for (int level = 0; level < 7; level++)
            {
                for (int mission = 0; mission < 7; mission++)
                {
                    name = $"MISSION_TITLE_L{level + 1}_M{mission + 1}";
                    string title = language.GetString(name);
                    missionTitles[level][mission] = title;
                    language.SetString(name, "LOCKED");
                }
            }


            var watcher = memory.Watcher;
            watcher.Error += Watcher_Error;
            
            /*
            watcher.NewGame += Watcher_NewGame;
            watcher.LoadGame += Watcher_LoadGame;
            */
            watcher.CardCollected += Watcher_CardCollected;
            watcher.MissionStageChanged += Watcher_MissionStageChanged;

            watcher.Start();
        }

        /* Default cars are unlocked on level load, even if it was locked before, so we need to relock them until the item is received. */
        public void LockDefaultCarsOnLoad(Memory memory)
        {
            var rewardsManager = memory.Singletons.RewardsManager;
            foreach (var rewards in rewardsManager.RewardsList)
            {
                var textBible = memory.Globals.TextBible.CurrentLanguage;
                if (!UnlockedItems.Contains(textBible?.GetString(rewards.DefaultCar.Name.ToUpper()) ?? rewards.DefaultCar.Name))
                    rewards.DefaultCar.Earned = false;
            }
        }

        async Task GetItems(Memory memory)
        {
            var cs = memory.Singletons.CharacterSheetManager.CharacterSheet;

            while (!memory.Process.HasExited)
            {
                try
                {
                    GameFlow.GameState? state = memory.Singletons.GameFlow?.CurrentContext;
                    if (!(state == null || !(state == GameFlow.GameState.DemoInGame || state == GameFlow.GameState.NormalInGame || state == GameFlow.GameState.BonusInGame)))
                    {
                        string item = await itemsReceived.DequeueAsync();

                        var rewardsManager = memory.Singletons.RewardsManager;
                        if (rewardsManager == null)
                        {
                            Console.WriteLine("Error retrieving items from AP. Will retry.", "Main");
                            return;
                        }

                        var textBible = memory.Globals.TextBible.CurrentLanguage;
                        Reward matchingReward = REWARDS.FirstOrDefault(reward => (textBible?.GetString(reward.Name.ToUpper()) ?? reward.Name) == item);
                        if (matchingReward != null)
                        {
                            Console.WriteLine($"Unlocking {(textBible?.GetString(matchingReward.Name.ToUpper()) ?? matchingReward.Name)}");
                            matchingReward.Earned = true;
                        }
                        else if (item.StartsWith("Level"))
                        {
                            Console.WriteLine($"Unlocking {item}");
                            UnlockMissionsPerLevel(item);
                        }
                        else
                        {
                            Console.WriteLine($"Error unlocking reward: {item}.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        bool UnlockCurrentMission(Memory memory, MissionStage stage = null)
        {
            Console.WriteLine("Checking to unlock current mission.");
            if (memory?.Globals?.GameplayManager is not MissionManager missionManager)
                return false;

            int? level = (int)missionManager.LevelData.Level;
            int? mission = missionManager.GetCurrentMissionIndex();
            if (!level.HasValue || !mission.HasValue)
                return false;

            if ((mission.Value & 1) != 0) // Check if Sunday Drive
                return false;

            int index = mission.Value / 2;
            if (level == 0)
                index -= 1;
            if (index < 0)
                return false;

            if (!UnlockedLevels.Contains($"Level {level + 1}")) // Skip if item not received
                return false;

            var objective = (stage ?? missionManager.GetCurrentMission()?.GetCurrentStage())?.Objective;
            if (objective?.ObjectiveType != MissionObjective.ObjectiveTypes.Invalid)
                return false;

            objective.Finished = true;
            Console.WriteLine($"Skipped dummy objective for L{level + 1}SD{index + 1}");
            return true;
        }

        bool UnlockMissionsPerLevel(string level)
        {
            UnlockedLevels.Add(level);
            int levelNum = int.Parse(Regex.Match(level, @"\d+").Value);

            for (int mission = 0; mission < 7; mission++)
            {
                string name = $"MISSION_TITLE_L{levelNum}_M{mission + 1}";
                language.SetString(name, missionTitles[levelNum - 1][mission]);
            }

            return true;
        }

        bool HandleCurrentBonusMissions(Memory memory)
        {
            var missionManager = (memory.Globals.GameplayManager as MissionManager);
            var missions = missionManager.Missions.ToArray();
            int? level = (int)missionManager.LevelData.Level;
            bool unlocked = false;
            if (UnlockedLevels.Contains($"Level {level + 1}")) // Skip if level item already received
                unlocked = true;
            
            foreach (var bonusMissionInfo in missionManager.BonusMissions)
            {
                if (bonusMissionInfo.MissionNum < 0) continue; // Avoids empty bonus mission slots

                var bonusMission = missions[bonusMissionInfo.MissionNum];
                List<string> bms = new List<string> { "bm1", "sr1", "sr2", "sr3" };
                
                if (!bms.Contains(bonusMission.Name))
                    continue;

                if (unlocked)
                {
                    Console.WriteLine($"Unlocking {bonusMission.Name}");
                    bonusMissionInfo.EventLocator.Flags = Locator.LocatorFlags.Active;
                    if (bonusMissionInfo.Icon?.DSGEntity?.Drawstuff is tCompositeDrawable compositeDrawable)
                    {
                        foreach (var element in compositeDrawable.Elements)
                        {
                            element.Visible = true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Locking {bonusMission.Name}");
                    bonusMissionInfo.EventLocator.Flags = Locator.LocatorFlags.None;
                    if (bonusMissionInfo.Icon?.DSGEntity?.Drawstuff is tCompositeDrawable compositeDrawable)
                    {
                        foreach (var element in compositeDrawable.Elements)
                        {
                            element.Visible = false;
                        }
                    }
                }
            }
            
            return true;
        }

        Task Watcher_Error(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.ErrorEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Error: {e.Exception}");
            return Task.CompletedTask;
        }


        Task Watcher_MissionStageChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameplayManager.MissionStageChangedEventArgs e, CancellationToken token)
        {
            UnlockCurrentMission(sender, e.NewStage);
            HandleCurrentBonusMissions(sender);
            LockDefaultCarsOnLoad(sender);

            return Task.CompletedTask;
        }

        Task Watcher_CardCollected(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CardGallery.CardCollectedEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"L{e.Level + 1}C{e.Card + 1} collected.");
            ArchipelagoClient.sentLocations.Enqueue(LocationTranslations.Cards[$"L{e.Level + 1}C{e.Card + 1}"]);

            return Task.CompletedTask;
        }
    }
}
