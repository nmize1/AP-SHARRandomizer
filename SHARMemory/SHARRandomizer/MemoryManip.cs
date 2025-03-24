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
using System.Runtime.CompilerServices;
using System.Numerics;
using Archipelago.MultiClient.Net.Models;

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
        public static bool APCONNECTED = false;
        public ArchipelagoClient ac;
        public static Queue<ScoutedItemInfo> ScoutedItems = new Queue<ScoutedItemInfo>();
        LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        public static AwaitableQueue<string> itemsReceived = new AwaitableQueue<string>();
        List<Reward> REWARDS = new List<Reward>();

        List<string> UnlockedLevels = new List<string>();
        List<string> UnlockedItems = new List<string>();
        public Dictionary<string, int> fillerInventory = new Dictionary<string, int>();
        List<string> traps = new List<string>();
        List<string> moves = new List<string>();
        FeLanguage language = null;

        bool DISABLEDOUBLEJUMPS = false;
        bool DISABLEEBRAKE = false;
        bool DISABLEDEFAULT = false;
        string CURRENTLEVEL = "";

        public SaveData sd;
        public int MINSHOPCOST = 100;
        public int MAXSHOPCOST = 1000;

        public async Task MemoryStart()
        {
            Console.WriteLine("Waiting for SHAR process...", "Main");
            while (true)
            {
                do
                {
                    p = Memory.GetSHARProcess();
                } while (p == null);
                do { } while (!APCONNECTED);
                sd = new SaveData();
                sd.Load();

                Console.WriteLine("Found SHAR process. Initialising memory manager...", "Main");

                Memory memory = new(p);
                Console.WriteLine($"SHAR memory manager initialised. Game version detected: {memory.GameVersion}. Language: {memory.GameSubVersion}.", "Main");
                GameFlow.GameState? state = memory.Singletons.GameFlow?.CurrentContext;

                await InitialGameState(memory);
                await GetItems(memory);

                memory.Dispose();
                p.Dispose();
                Console.WriteLine("SHAR closed. Waiting for SHAR process...", "Main");
            }
        }

        /* Setting things up */
        public async Task InitialGameState(Memory memory)
        {
            Console.WriteLine("Setting up initial game state.", "Main");
            var rewardsManager = memory.Singletons.RewardsManager;
            if (rewardsManager == null)
            {
                Console.WriteLine("Error setting up initial game state. Things will not work correctly.", "Main");
                return;
            }

            Console.WriteLine("Waiting till gameplay starts.");
            while (!Extensions.InGame(memory))
            {
                await Task.Delay(100);
            }

            /* Create default list of random shop costs, then replace it with the stored one if a stored one exists */
            List<int> ShopCosts = ArchipelagoClient.ShopCosts;

            /* Get all rewards in a list for lookup purposes */
            int i = 0;
            int s = 0;
            
            var rewardsList = rewardsManager.RewardsList.ToArray();
            var tokenStoreList = rewardsManager.LevelTokenStoreList.ToArray();
            for (int level = 0; level < memory.Globals.LevelCount; level++)
            {
                List<Reward> tempRewards = new List<Reward>();
                var levelRewards = rewardsList[level];
                tempRewards.Add(levelRewards.DefaultCar);
                tempRewards.Add(levelRewards.BonusMission);
                tempRewards.Add(levelRewards.StreetRace);
                Console.WriteLine($"Saving ShopCosts for level {level + 1}");

                var levelTokenStoreList = tokenStoreList[level];
                for (int merchandiseIndex = 0; merchandiseIndex < levelTokenStoreList.Counter; merchandiseIndex++)
                {
                    var merchandise = memory.Functions.GetMerchandise(level, merchandiseIndex);

                    if (merchandise == null)
                        continue;

                    if (merchandise.RewardType == Reward.RewardTypes.Null)
                        continue;

                    if (merchandise.Name == "Null")
                        continue;

                    tempRewards.Add(merchandise);
                    if (merchandise.Name.Contains("APCar"))
                    {
                        merchandise.Cost = ShopCosts[s++];
                        Console.WriteLine($"MERCHANDISE {merchandise.Cost}");
                    }
                }
                REWARDS.AddRange(tempRewards);
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
            watcher.MerchandisePurchased += Watcher_MerchandisePurchased;
            watcher.DialogPlaying += Watcher_DialogPlaying;

            watcher.Start();

            InputListener listener = new InputListener();
            listener.memory = memory;
            listener.ButtonDown += Listener_ButtonDown;

            listener.Start();

            var textBible = memory.Globals.TextBible.CurrentLanguage;
            Console.WriteLine("REWARDS:");
            foreach (var r in REWARDS)
            {
                Console.WriteLine($"{(textBible?.GetString(r.Name.ToUpper()) ?? r.Name)} : {r.Name }");
            }

            await Task.Run(async () =>
            {
                while (language == null)
                {
                    language = memory.Globals?.TextBible?.CurrentLanguage;
                    await Task.Delay(100);
                }
            });

            InitializeMissionTitles(); 
            Task.Run(() => InitializeShopItems());

            var characterSheet = memory.Singletons.CharacterSheetManager;

            if (characterSheet == null)
            {
                Console.WriteLine("Error getting character sheet.");
            }
            else
            { 
                fillerInventory.Add("Hit N Run Reset", sd.GetHitNRunReset());
                fillerInventory.Add("Wrench", sd.GetWrench());
            }

            traps.AddRange(new List<string> { "Hit N Run", "Reset Car", "Duff Trap" });
            Task.Run(() => CheckActions(memory));
        }

        /* Default cars are unlocked on level load, even if it was locked before, so we need to relock them until the item is received. */
        public void LockDefaultCarsOnLoad(Memory memory, int level)
        {
            List<string> dcars = new List<string> { "Family Sedan", "Honor Roller", "Malibu Stacy Car", "Canyonero", "Longhorn", "Ferrini - Red", "70's Sports Car"  };
            var rewardsManager = memory.Singletons.RewardsManager;
            var textBible = memory.Globals.TextBible.CurrentLanguage;
            int i = 0;
            DISABLEDEFAULT = false;
            foreach (var rewards in rewardsManager.RewardsList)
            {
                if (!UnlockedItems.Contains(rewards.DefaultCar.Name))
                {
                    rewards.DefaultCar.Earned = false;
                    if (i == level)
                    {
                        DISABLEDEFAULT = true;
                    }
                }
                i++;
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
                            UnlockedItems.Add(matchingReward.Name);
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
                                    switch (s)
                                    {
                                        case "Hit N Run Reset":
                                            sd.SetHitNRunReset(fillerInventory[s]);
                                            break;

                                        case "Wrench":
                                            sd.SetWrench(fillerInventory[s]);
                                            break;

                                    }
                                    Console.WriteLine($"Received {s}.");
                                    break;

                                case string s when traps.Contains(s):
                                    Console.WriteLine($"Received TRAP {s}.");
                                    HandleTraps(memory, s);
                                    break;

                                case string s when s.Contains("Jump") || s.Contains("Attack") || s.Contains("Brake"):
                                    Console.WriteLine($"Received {s}");
                                    moves.Add(s);
                                    CheckAvailableMoves(memory, CURRENTLEVEL);
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
                List<string> bms = new List<string> { "bm1", "bm2", "sr1", "sr2", "sr3" };
                
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

        public async void InitializeShopItems()
        {
            Dictionary<long, string> locsToScout = new Dictionary<long, string>();
            for (int level = 0; level < 7; level++)
            {
                for (int check = 1; check <= 6; check++)
                {
                    string name = $"APCar{6 * level + check}";
                    long location = lt.getAPID(name, "shop");
                    locsToScout.Add(location, name);
                }
            }

            ac.ScoutShopLocationNoHint(locsToScout, language);
        }


        void CheckAvailableMoves(Memory memory, string level)
        {
            string character = "";
            switch (level)
            {
                case "L1":
                    character = "Homer";
                    break;
                case "L2":
                    character = "Bart";
                    break;
                case "L3":
                    character = "Lisa";
                    break;
                case "L4":
                    character = "Marge";
                    break;
                case "L5":
                    character = "Apu";
                    break;
                case "L6":
                    character = "Bart";
                    break;
                case "L7":
                    character = "Homer";
                    break;
            }

            if (!moves.Contains($"{character} Attack"))
                memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.Attack);
            else
                memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.Attack);

            if (!moves.Contains($"{character} Double Jump"))
                DISABLEDOUBLEJUMPS = true;
            else
                DISABLEDOUBLEJUMPS = false;

            if (!moves.Contains($"{character} E-Brake"))
            {
                memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.HandBrake);
                DISABLEEBRAKE = true;
            }
            else
            {
                memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.HandBrake);
                DISABLEEBRAKE = false;
            }
        }

        async void CheckActions(Memory memory)
        {
            RewardsManager rewardsManager = null;
            while (rewardsManager == null)
            {
                rewardsManager = memory.Singletons.RewardsManager;
            }

            while (memory.IsRunning)
            {
                await System.Threading.Tasks.Task.Delay(1);
                if (memory.InGame())
                {
                    var jumpAction = memory.Singletons.CharacterManager?.Player?.JumpLocomotion;
                    if (jumpAction != null)
                    {
                        if (DISABLEDOUBLEJUMPS)
                            jumpAction.JumpAgain = true;
                    }

                    //Commented out in case we need to check car speed again for debugging later.
                    //Console.WriteLine(memory.Singletons.CharacterManager?.Player?.Car?.Speed.ToString()); 
                    if (DISABLEEBRAKE && memory.Singletons.CharacterManager?.Player?.Car?.Speed >= 1)
                        memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.GetOutCar);
                    else
                        memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.GetOutCar);

                    var player = memory.Singletons.CharacterManager?.Player;
                    if (player == null)
                        return;

                    List<String> defaults = rewardsManager.RewardsList.Select(reward => reward.DefaultCar.Name).ToList();
                    if (DISABLEDEFAULT && player.Car != null && defaults[(CURRENTLEVEL[1] - '0') - 1] == player.Car.Name)
                    {
                        player.Car.Stop();
                        player.Controller.Intention = CharacterController.Intentions.GetOutCar;
                    }
                }  
            }
        }

        async void HandleTraps(Memory memory, string trap)
        {
            Button button;
            var buttonArray = memory.Singletons.InputManager.ControllerArray[0].ButtonArray;
            switch (trap)
            {
                case "Reset Car":
                    button = buttonArray[(int)InputManager.Buttons.ResetCar];
                    button.Value = 1;
                    await Task.Delay(1);
                    button.Value = 0;
                    break;
                case "Duff Trap":
                    List<Button> buttons = new List<Button>
                    {
                        buttonArray[(int)InputManager.Buttons.SteerLeft],
                        buttonArray[(int)InputManager.Buttons.SteerRight],
                        buttonArray[(int)InputManager.Buttons.MoveLeft],
                        buttonArray[(int)InputManager.Buttons.MoveRight]
                    };
                    var endTime = DateTime.UtcNow.AddSeconds(30);

                    while (DateTime.UtcNow < endTime)
                    {
                        Button buttonToPress = buttons[Random.Shared.Next(4)];
 
                        buttonToPress.Value = 1;
                        await Task.Delay(Random.Shared.Next(50, 200)); 
                        buttonToPress.Value = 0;
 
                        await Task.Delay(Random.Shared.Next(100, 500));  
                    }
                    break;
                case "Hit N Run":
                    memory.Singletons.HitNRunManager.CurrHitAndRun = 100f;
                    break;
                default:
                    break;

            }
        }

        void LockBonusCars(Memory memory)
        {
            foreach (Reward reward in REWARDS)
            {
                var textBible = memory.Globals.TextBible.CurrentLanguage;
                string name = textBible?.GetString(reward.Name.ToUpper()) ?? reward.Name;
                if (reward.QuestType == Reward.QuestTypes.BonusMission && !UnlockedItems.Contains(name))
                {
                    reward.Earned = false;
                }
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
                sd.SetHitNRunReset(fillerInventory["Hit N Run Reset"]);
            }
            if (e.Button.ToString() == "DPadDown" || e.Button.ToString() == "D2")
            {
                if (fillerInventory["Wrench"] > 0)
                { 
                    listener.memory.Functions.TriggerEvent(Globals.Events.REPAIR_CAR);
                    fillerInventory["Wrench"]--;
                }
                Console.WriteLine($"Wrenches: {fillerInventory["Wrench"]}");
                sd.SetWrench(fillerInventory["Wrench"]);
            }
        }

        Task Watcher_Error(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.Error.ErrorEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Error: {e.Exception}");
            return Task.CompletedTask;
        }

        Task Watcher_MissionStageChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameplayManager.MissionStageChangedEventArgs e, CancellationToken token)
        {
            CURRENTLEVEL = e.Level.ToString();
            UnlockCurrentMission(sender, e.NewStage);
            HandleCurrentBonusMissions(sender);
            CheckAvailableMoves(sender, CURRENTLEVEL);
            LockDefaultCarsOnLoad(sender, ((int)e.Level));
            if (e.Mission.ToString() == "BM2" || e.Mission.ToString() == "BM3")
            {
                Console.WriteLine($"{(int)e.Level} - bonus2");
                ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{(int)e.Level} - bonus2", "bonus missions"));
                LockBonusCars(sender);
            }
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

        Task Watcher_MerchandisePurchased(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.RewardsManager.MerchandisePurchasedEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Car Purchased: {e.Merchandise.Name}"); 
            if (e.Merchandise.Name.Contains("APCar"))
            {
                Console.WriteLine($"Sending check from {e.Merchandise.Name}");
                ArchipelagoClient.sentLocations.Enqueue(lt.getAPID(e.Merchandise.Name, "shop"));
            }
            return Task.CompletedTask;
        }

        Task Watcher_MissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.MissionCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Mission Complete: {e.Level} - {e.Mission + 1}");
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - {e.Mission + 1}", "missions"));

            
            return Task.CompletedTask;
        }

        Task Watcher_BonusMissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.BonusMissionCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Mission Complete: {e.Level} - bonus");
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - bonus", "bonus missions"));
            LockBonusCars(sender);
            return Task.CompletedTask;
        }

        Task Watcher_StreetRaceComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.StreetRaceCompleteEventArgs e, CancellationToken token)
        {
            Console.WriteLine($"Race Complete: {e.Level} - {e.Race}");
            ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{e.Level} - {e.Race}", "bonus missions"));
            return Task.CompletedTask;
        }

        Task Watcher_DialogPlaying(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.SoundManager.DialogPlayingEventArgs e, CancellationToken token)
        {
            Console.WriteLine(e.Dialog.Event);
            
            if (e.Dialog.Event.ToString() == "HAGGLING_WITH_GIL")
            {
                Console.WriteLine($"Spoke to Gil on level {CURRENTLEVEL}");
                Dictionary<long, string> locsToScout = new Dictionary<long, string>();
                for (int check = 1; check <= 6; check++)
                {
                    string name = $"APCar{6 * (int.Parse(CURRENTLEVEL.Substring(1)) - 1) + check}";
                    long location = lt.getAPID(name, "shop");
                    locsToScout.Add(location, name);
                }

                ac.ScoutShopLocation(locsToScout.Keys.ToArray());
            }

            return Task.CompletedTask;
        }
    }
}
