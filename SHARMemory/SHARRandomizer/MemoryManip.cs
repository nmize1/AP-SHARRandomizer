using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.Json;
using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Structs;
using SHARRandomizer.Classes;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Tracing;

namespace SHARRandomizer
{
    static class Extensions
    {
        public static bool InGame(this SHARMemory.SHAR.Memory memory) => memory.IsRunning && memory.Singletons.GameFlow?.NextContext switch
        {
            GameFlow.GameState.NormalInGame or GameFlow.GameState.DemoInGame or GameFlow.GameState.BonusInGame => true,
            _ => false,
        };
    }

    class MemoryManip
    {
        Process? p;

        LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        public static AwaitableQueue<string> itemsReceived = new AwaitableQueue<string>();
        public static string UUID = "";
        List<Reward> REWARDS = new List<Reward>();

        List<string> UnlockedLevels = new List<string>();
        List<string> UnlockedItems = new List<string>();
        public Dictionary<string, int> fillerInventory = new Dictionary<string, int>();
        List<string> traps = new List<string>();

        FeLanguage language = null;



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
                GameFlow.GameState? state = memory.Singletons.GameFlow?.CurrentContext;

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

            var watcher = memory.Watcher;
            watcher.Error += Watcher_Error;
            watcher.CardCollected += Watcher_CardCollected;
            watcher.MissionStageChanged += Watcher_MissionStageChanged;
            watcher.MissionComplete += Watcher_MissionComplete;
            watcher.BonusMissionComplete += Watcher_BonusMissionComplete;
            watcher.StreetRaceComplete += Watcher_StreetRaceComplete;
            watcher.PersistentObjectDestroyed += Watcher_PersistentObjectDestroyed;
            watcher.GagViewed += Watcher_GagViewed;
            watcher.CarPurchased += Watcher_CarPurchased;

            watcher.Start();

            InputListener listener = new InputListener();
            listener.memory = memory;
            listener.ButtonDown += Listener_ButtonDown;

            listener.Start();

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
            var textBible = memory.Globals.TextBible.CurrentLanguage;
            Console.WriteLine("REWARDS:");
            foreach (var r in REWARDS)
            {
                Console.WriteLine((textBible?.GetString(r.Name.ToUpper()) ?? r.Name));
            }

            while (!Extensions.InGame(memory))
            {
                
            }
            while (language == null)
            {
                language = memory.Globals?.TextBible?.CurrentLanguage;
            }
            /* Lock all default cars since they unlock on level load */
            LockDefaultCarsOnLoad(memory);
            InitializeMissionTitles();

            fillerInventory.Add("Hit N Run Reset", 0);
            fillerInventory.Add("Wrench", 0);
            traps.AddRange(new List<string> { "Car Brake", "Reset Car" });
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
                    if(Extensions.InGame(memory))
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
                        else
                        {
                            switch (item)
                            {
                                case string s when s.StartsWith("Level"):
                                    Console.WriteLine($"Unlocking {item}");
                                    UnlockMissionsPerLevel(item);
                                    break;

                                case string s when s.Contains("Coins"):
                                    int amount = int.Parse(new string(s.TakeWhile(char.IsDigit).ToArray()));
                                    var characterSheet = memory.Singletons.CharacterSheetManager;
                                    
                                    if (characterSheet == null)
                                    {
                                        Console.WriteLine("Error getting character sheet.");
                                        break;
                                    }
                                    characterSheet.CharacterSheet.Coins += amount;
                                    Console.WriteLine($"Received {amount} coins.");
                                    break;

                                case string s when fillerInventory.Keys.Contains(s):
                                    fillerInventory[s]++;
                                    Console.WriteLine($"Received {s}.");
                                    break;

                                case string s when traps.Contains(s):
                                    Console.WriteLine($"Received TRAP {s}.");
                                    HandleTraps(memory, s);
                                    break;

                                default:
                                    Console.WriteLine($"Error unlocking reward: {item}.");
                                    break;
                            }
                            
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
                string missionTitle = lt.getMissionName(mission, levelNum-1);
                Console.WriteLine(missionTitle);
                string name = $"MISSION_TITLE_L{levelNum}_M{mission + 1}";
                language.SetString(name, missionTitle.Trim());
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

        void InitializeMissionTitles()
        { 
            string name = $"MISSION_TITLE_L{0}_M{0}";
            language.SetString(name, "LOCKED");


            for (int level = 0; level < 7; level++)
            {
                for (int mission = 0; mission < 7; mission++)
                {
                    name = $"MISSION_TITLE_L{level + 1}_M{mission + 1}";
                    language.SetString(name, "LOCKED");
                    Console.WriteLine($"{name} is LOCKED");
                }
            }
        }

        async void HandleTraps(Memory memory, string trap)
        {
            Button button;
            switch (trap)
            {
                case "Rainbow":                  
                    break;
                case "Reset Car":
                    button = memory.Singletons.InputManager.ControllerArray[0].ButtonArray[(int)InputManager.Buttons.ResetCar];
                    button.Value = 1;
                    await Task.Delay(1);
                    button.Value = 0;
                    break;
                case "Duff Trap":
                    break;
                case "Flippable":
                    break;
                default:
                    break;

            }
        }

        public void Listener_ButtonDown(Object? sender, InputListener.ButtonEventArgs e)
        {
            InputListener listener = (InputListener)sender;
            if (e.Button.ToString() == "DPadUp" || e.Button.ToString() ==  "D1")
            {
                if (fillerInventory["Hit N Run Reset"] > 0 && listener.memory.Singletons.HitNRunManager.CurrHitAndRun > 0f)
                {
                    listener.memory.Singletons.HitNRunManager.CurrHitAndRun = 0f;
                    fillerInventory["Hit N Run Reset"]--;
                }
                Console.WriteLine($"Hit N Run Resets: {fillerInventory["Hit N Run Reset"]}");
            }
            if (e.Button.ToString() == "DPadDown" || e.Button.ToString() == "D2")
            {
                if (fillerInventory["Wrench"] > 0)
                {
                    //listener.memory.Globals.GameplayManager.RepairCurrentVehicle();

                    fillerInventory["Wrench"]--;
                }
                
                Console.WriteLine($"Wrenches: {fillerInventory["Wrench"]}");
            }
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
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"L{e.Level + 1}C{e.Card + 1}", "card"));

            return Task.CompletedTask;
        }

        Task Watcher_PersistentObjectDestroyed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.PersistentObjectDestroyedEventArts e, CancellationToken token)
        {
            Console.WriteLine($"Destroyed object: {e.Sector} - {e.Index}");
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Sector} - {e.Index}", "wasp"));
            return Task.CompletedTask;
        }

        Task Watcher_GagViewed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.GagViewedEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Gag Viewed: {e.Level} - {e.Gag}");
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - {e.Gag}", "gag"));
            return Task.CompletedTask;
        }

        Task Watcher_CarPurchased(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.CarPurchasedEventArgs e, CancellationToken token)
        {
            //Console.WriteLine($"Car Purchased: {e.}")
            return Task.CompletedTask;
        }

        Task Watcher_MissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.MissionCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Mission Complete: {e.Level} - {e.Mission}");
            //ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - {e.Mission}", "mission"));
            return Task.CompletedTask;
        }

        Task Watcher_BonusMissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.BonusMissionCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Mission Complete: {e.Level} - Bonus");
            //ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - Bonus", "bonus_mission"));
            return Task.CompletedTask;
        }

        Task Watcher_StreetRaceComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.StreetRaceCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Race Complete: {e.Level} - {e.Race}");
            //ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - {e.Race}", "bonus_mission"));
            return Task.CompletedTask;
        }
    }
}
