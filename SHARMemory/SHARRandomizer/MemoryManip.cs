﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARRandomizer.Classes;
using Archipelago.MultiClient.Net.Models;
using System.Xml.Linq;
using static SHARRandomizer.ArchipelagoClient;
using System;
using System.Reflection;
using System.Drawing;
using SHARMemory.SHAR.Structs;

namespace SHARRandomizer
{
    static class Extensions
    {
        public static bool InGame(this Memory memory) => memory.IsRunning && memory.Singletons.GameFlow?.NextContext switch
        {
            GameFlow.GameState.NormalInGame or GameFlow.GameState.DemoInGame or GameFlow.GameState.BonusInGame => true,
            _ => false,
        };

    }

    public class MemoryManip
    {
        Process? p;

        private static bool _APCONNECTED = false;
        private static readonly object _APCONNECT_LOCK = new();
        public static bool APCONNECTED
        {
            get
            {
                lock (_APCONNECT_LOCK)
                {
                    return _APCONNECTED;
                }
            }
            set
            {
                lock (_APCONNECT_LOCK)
                {
                    _APCONNECTED = value;
                }
            }
        }


        public ArchipelagoClient ac;
        public static string VerifyID;
        public static Queue<ScoutedItemInfo> ScoutedItems = new Queue<ScoutedItemInfo>();
        LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        RewardTranslations rt = RewardTranslations.LoadFromJson("Configs/Rewards.json");
        public static AwaitableQueue<string> itemsReceived = new AwaitableQueue<string>();
        public static FixedSizeQueue<string> APLog = new FixedSizeQueue<string>(5);
        public static List<long> cardIDs;

        List<Reward> REWARDS = new List<Reward>();
        List<string> UnlockedLevels = new List<string>();
        List<string> UnlockedItems = new List<string>();
        public Dictionary<string, int> fillerInventory = new Dictionary<string, int>();
        List<string> traps = new List<string>();
        List<string> moves = new List<string>();
        FeLanguage language = null;

        float djAllowUp = 2.0f;
        float djAllowDown = 12.0f;
        bool DISABLEEBRAKE = false;
        bool DISABLEDEFAULT = false;
        string CURRENTLEVEL = "";

        public int WalletLevel = 1;
        public static int maxCoins;
        public static int coinScale;
        bool _updatingCoins = false;

        public static bool gagfinder;
        public static bool checkeredflag;

        ActorManager actorManager;
        public bool carwasp = false;

        uint gameLanguage;
        private Watcher _watcher;

        public async Task MemoryStart()
        {
            while (true)
            {
                Common.WriteLog("Waiting for SHAR process...", "MemoryStart");
                while ((p = Memory.GetSHARProcess()) == null)
                    await Task.Delay(100);

                Common.WriteLog("Found SHAR process. Awaiting AP connection...", "MemoryStart");
                while (!APCONNECTED)
                    await Task.Delay(100);

                Common.WriteLog("AP Connected. Initialising memory manager...", "MemoryStart");
                Memory memory = new(p);
                Common.WriteLog($"SHAR memory manager initialised. Game version detected: {memory.GameVersion}. Sub Version: {memory.GameSubVersion}.", "MemoryStart");
                string mod = memory.GetMainMod() ?? "No Mod";
                Common.WriteLog($"Main Mod: {mod}", "MemoryStart");
                if (mod != "APSHARRandomizer")
                {
                    Common.WriteLog($"Main Mod should be APSHARRandomizer, detected {mod}. Exiting.", "MemoryStart");
                    Environment.Exit(1);
                }
           

                var state = memory.Singletons.GameFlow?.CurrentContext;
                while (state == null || state == GameFlow.GameState.PreLicence || state == GameFlow.GameState.Licence)
                {
                    await Task.Delay(100);
                    state = memory.Singletons.GameFlow?.CurrentContext;
                }

                var watcher = memory.Watcher;
                _watcher = watcher;
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
                watcher.CoinsChanged += Watcher_CoinsChanged;
                watcher.RewardUnlocked += Watcher_RewardUnlocked;
                watcher.ButtonBound += Watcher_ButtonBound;
                watcher.NewGame += Watcher_NewGame;
                watcher.NewTrafficVehicle += Watcher_NewTrafficVehicle;

                watcher.Start();
                await LoadState(memory);

                gameLanguage = memory.Globals.FeTextBible.LanguageIndex;
                Common.WriteLog($"SHAR language: {gameLanguage}", "MemoryStart");

                await InitialGameState(memory);
                await GetItems(memory);

                memory.Dispose();
                p.Dispose();
                Common.WriteLog("SHAR closed. Waiting for SHAR process...", "MemoryStart");
            }
        }

        /* Setting things up */
        public async Task InitialGameState(Memory memory)
        {
            Common.WriteLog("Setting up initial game state.", "InitialGameState");
            var rewardsManager = memory.Singletons.RewardsManager;
            if (rewardsManager == null)
            {
                Common.WriteLog("Error setting up initial game state. Things will not work correctly.", "InitialGameState");
                return;
            }

            Common.WriteLog("Waiting till gameplay starts.", "InitialGameState");
            while (!Extensions.InGame(memory))
            {
                await Task.Delay(100);
            }

            /* Create default list of random shop costs, then replace it with the stored one if a stored one exists */
            List<int> ShopCosts = ArchipelagoClient.ShopCosts;

            /* Get all rewards in a list for lookup purposes */
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
                Common.WriteLog($"Saving ShopCosts for level {level + 1}", "InitialGameState");

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
                        Common.WriteLog($"MERCHANDISE {merchandise.Cost}", "InitialGameState");
                    }
                }
                REWARDS.AddRange(tempRewards);
                
            }

            InputListener listener = new InputListener();
            listener.memory = memory;
            listener.ButtonDown += Listener_ButtonDown;

            listener.Start();

            APLog.OnEnqueue += item => APLogging();

            var textBible = memory.Globals.TextBible.CurrentLanguage;

            if (textBible?.GetString("VerifyID") != VerifyID)
            {
                Common.WriteLog($"SHAR.ini outdated. Exiting.", "MemoryStart");
                Environment.Exit(1);
            }

            Common.WriteLog("REWARDS:", "InitialGameState");
            foreach (var r in REWARDS)
            {
                Common.WriteLog($"{textBible?.GetString(r.Name.ToUpper()) ?? r.Name} : {r.Name}", "InitialGameState");
            }

            var lang = memory.Globals?.TextBible?.CurrentLanguage;
            while (lang == null)
            {
                await Task.Delay(100);
                lang = memory.Globals?.TextBible?.CurrentLanguage;
            }
            language = lang;

            while ((actorManager = memory.Singletons.ActorManager) == null)
                await Task.Delay(100);
            Common.WriteLog("Found ActorManager. Checking Wasps...", "InitialGameState");


            InitializeMissionTitles();
            InitializeShopItems();

            switch (WalletLevel)
            {
                case 1:
                    language.SetString("APMaxCoins", $"/{maxCoins}");
                    break;
                case 7:
                    language.SetString("APMaxCoins", "");
                    break;
                default:
                    language.SetString("APMaxCoins", $"/{maxCoins * WalletLevel * coinScale}");
                    break;
            }

            language.SetString("APHnR", "00");
            language.SetString("APWrench", "00");
            UpdateProgress(0, 0, 0, 0, 0, 0, 0, 0);
            var characterSheet = memory.Singletons.CharacterSheetManager;
            if (characterSheet == null)
            {
                Common.WriteLog("Error getting character sheet.", "InitialGameState");
            }
            else
            {
                int h, w;
                w = ac.GetDataStorage<int>("wrench").Result;
                h = ac.GetDataStorage<int>("hnr").Result;
                fillerInventory.Add("Hit N Run Reset", h);
                fillerInventory.Add("Wrench", w);
                language.SetString("APHnR", $"{h:D2}");
                language.SetString("APWrench", $"{w:D2}");
            }

            ac.CheckVictory();
            traps.AddRange(new List<string> { "Hit N Run", "Reset Car", "Duff Trap", "Eject", "Launch" });
            Task.Run(() => CheckActions(memory));
            Task.Run(() => CheckGags(memory));
        }

        async Task LoadState(Memory memory)
        {
            var characterSheet = memory.Singletons.CharacterSheetManager;

            if (characterSheet == null)
            {
                Common.WriteLog("Character sheet missing", "LoadState");
                return;
            }

            characterSheet.CharacterSheet.Coins = await ac.GetDataStorage<int>("coins");
            Common.WriteLog($"Restoring coins to {characterSheet.CharacterSheet.Coins}", "LoadState");
            List<long> locations = await ac.GetDataStorage<List<long>>("localchecks");
            
            /* get struct from characterSheet.LevelList.ToArray() to update below */
            LevelRecord[] record = characterSheet.CharacterSheet.LevelList.ToArray();
            int[] waspCounters = new int[7];
            int[] gagCounters = new int[7];
            uint[] gagmask = new uint[7];
            List<string> purchased = new List<string>();

            foreach (var l in locations.Skip(1))
            {
                var check = lt.getTypeAndNameByAPID(l);
                string id = lt.GetIDByAPID(l);

                if (string.IsNullOrEmpty(id))
                {
                    if (cardIDs.Contains(l))
                        check = ("card", "card");
                    else
                    {
                        Common.WriteLog($"Invalid ID format: {id}", "LoadState");
                        continue;
                    }
                }

                int level;

                Common.WriteLog($"Restoring {check.name}", "LoadState");
                switch (check.type)
                {
                    case "mission":
                        {
                            int mission;
                            int.TryParse(id[0].ToString(), out level);
                            int.TryParse(id[4].ToString(), out mission);

                            MissionList missions = record[level].Missions;
                            missions.List[mission - 1].Completed = true;
                            break;
                        }
                    case "bonus missions":
                        {
                            int.TryParse(id[0].ToString(), out level);
                            if (id.EndsWith("bonus"))
                            {
                                record[level].BonusMission.Completed = true;
                            }
                            else
                            {
                                int race;
                                int.TryParse(id[4].ToString(), out race);

                                StreetRaceList races = record[level].StreetRaces;
                                races.List[race].Completed = true;
                            }
                            break;
                        }
                    case "wasp":
                        {
                            var parts = id.Split(" - ");
                            if (parts.Length == 2 && Enum.TryParse(parts[0], out CharacterSheet.PersistentObjectStateSector sector) && int.TryParse(parts[1], out int index))
                            {
                                characterSheet.CharacterSheet.SetPersistentObjectDestroyed(sector, index, true);
                                waspCounters[(int)sector - 75]++;
                            }
                            else
                                Common.WriteLog($"Invalid wasp ID format: {id}", "LoadState");
                            break;
                        }
                    case "card":
                        {
                            int index = cardIDs.IndexOf(l);
                            level = (index / 7) + 1;
                            int card = (index % 7) + 1;

                            CharCardList cards = record[level - 1].Cards;
                            cards.List[card - 1].Completed = true;
                            cards.List[card - 1].Name = $"card{level}{card}";
                            record[level - 1].Cards = cards;

                            break;
                        }
                    case "gag":
                        {
                            int.TryParse(id[0].ToString(), out level);
                            int gag;
                            int.TryParse(id[4].ToString(), out gag);

                            gagmask[level] |= (uint)1 << gag;
                            gagCounters[level]++;
                            break;
                        }
                    case "shop":
                        {
                            purchased.Add(id);
                            break;
                        }
                    default:
                        break;

                }
            }

            for (int i = 0; i < 7; i++)
            {
                record[i].WaspsDestroyed = waspCounters[i];
                record[i].GagsViewed = gagCounters[i];
                record[i].GagMask = gagmask[i];
            }

            var rewardsManager = memory.Singletons.RewardsManager;
            var levelTokenStores = rewardsManager.LevelTokenStoreList.ToArray();

            for (var level = 0; level < 7; level++)
            {
                Console.WriteLine($"Level {level + 1}");
                var levelMerchCount = levelTokenStores[level].Counter;

                for (var merchIndex = 0; merchIndex < levelMerchCount; merchIndex++)
                {
                    var merchandise = memory.Functions.GetMerchandise(level, merchIndex);
                    if (purchased.Contains(merchandise.Name))
                        merchandise.Earned = true;
                }
            }

            characterSheet.CharacterSheet.LevelList.FromArray(record);
            try
            {
                SyncCardGalleryWithCharacterSheet(memory);
            }
            catch (Exception ex)
            {
                Common.WriteLog($"Error syncing Card Gallery:\n{ex}", "LoadState");
            }

            return;
        }

        /* Proddy wrote this */
        void SyncCardGalleryWithCharacterSheet(SHARMemory.SHAR.Memory mem)
        {
            var characterSheet = mem.Singletons.CharacterSheetManager?.CharacterSheet;
            if (characterSheet == null)
                return;

            var cardGallery = mem.Singletons.CardGallery;
            if (cardGallery == null)
                return;

            var cardsDB = cardGallery.CardsDB;
            if (cardsDB == null)
                return;
            var cards = cardsDB.Cards.ToArray();

            var cardMap = new Dictionary<ulong, Card>();
            for (var i = 0; i < cards.Length; i++)
            {
                if (cards[i] == null)
                    continue;
                cardMap[cards[i].CardName] = cards[i];
            }

            var characterSheetLevels = characterSheet.LevelList.ToArray();
            var cardGalleryLevels = cardGallery.CollectedCards.ToArray();
            if (characterSheetLevels.Length != cardGalleryLevels.Length)
                return;

            for (var level = 0; level < characterSheetLevels.Length; level++)
            {
                var characterSheetCards = characterSheetLevels[level].Cards.List;
                var cardGalleryLevel = cardGalleryLevels[level];

                cardGalleryLevel.NumCards = 0;
                for (var card = 0; card < 7; card++)
                {
                    if (characterSheetCards[card].Completed)
                    {
                        if (!cardMap.TryGetValue(Helpers.radMakeKey(characterSheetCards[card].Name), out var cardDBCard))
                            Common.WriteLog($"Error restoring card {characterSheetCards[card].Name}", "MemoryManip::SyncCardGalleryWithCharacterSheet");
                        cardGalleryLevel.Cards[card] = cardDBCard;
                        cardGalleryLevel.NumCards++;
                    }
                    else
                    {
                        cardGalleryLevel.Cards[card] = null;
                    }
                }

                cardGalleryLevels[level] = cardGalleryLevel;
            }
            cardGallery.NumCollectedCards = cardGalleryLevels.Sum(x => x.NumCards);

            cardGallery.CollectedCards.FromArray(cardGalleryLevels);
        }

        /* Default cars are unlocked on level load, even if it was locked before, so we need to relock them until the item is received. */
        public void LockSpawnedCarsOnLoad(Memory memory, int level)
        {
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
                    if (Extensions.InGame(memory))
                    {
                        string item = await itemsReceived.DequeueAsync();

                        var rewardsManager = memory.Singletons.RewardsManager;
                        if (rewardsManager == null)
                        {
                            Common.WriteLog("Error retrieving items from AP. Will retry.", "Main");
                            return;
                        }

                        var textBible = memory.Globals.TextBible.CurrentLanguage;

                        if (item == "Cell Phone Car")
                            item = $"Cell Phone Car A";

                        var matchingReward = REWARDS.FirstOrDefault(reward => reward.Name == rt.GetInternalName(item));
                        if (matchingReward != null)
                        {
                            Common.WriteLog($"Unlocking {textBible?.GetString(matchingReward.Name.ToUpper()) ?? matchingReward.Name}", "GetItems");
                            matchingReward.Earned = true;
                            UnlockedItems.Add(matchingReward.Name);
                        }
                        else
                        {
                            switch (item)
                            {
                                case string s when s.Contains("Level"):
                                    Common.WriteLog($"Unlocking {item}", "GetItems");
                                    UnlockMissionsPerLevel(item);
                                    break;

                                case string s when s.Contains("Coins"):
                                    int amount = int.Parse(new string(s.TakeWhile(char.IsDigit).ToArray()));
                                    var characterSheet = memory.Singletons.CharacterSheetManager;

                                    if (characterSheet == null)
                                    {
                                        Common.WriteLog("Error getting character sheet.", "GetItems");
                                        break;
                                    }
                                    characterSheet.CharacterSheet.Coins += amount;
                                    Common.WriteLog($"Received {amount} coins.", "GetItems");
                                    break;

                                case string s when fillerInventory.ContainsKey(s):
                                    fillerInventory[s]++;
                                    switch (s)
                                    {
                                        case "Hit N Run Reset":
                                            ac.IncrementDataStorage("hnr");
                                            language.SetString("APHnR", $"{fillerInventory[s]:D2}");
                                            break;

                                        case "Wrench":
                                            ac.IncrementDataStorage("wrench");
                                            language.SetString("APWrench", $"{fillerInventory[s]:D2}");
                                            break;

                                    }
                                    Common.WriteLog($"Received {s}.", "GetItems");
                                    break;

                                case string s when traps.Contains(s):
                                    Common.WriteLog($"Received TRAP {s}.", "GetItems");
                                    HandleTraps(memory, s);
                                    break;

                                case string s when s.Contains("Jump") || s.Contains("Attack") || s.Contains("Brake") ||
                                                   s.Contains("Gagfinder") || s.Contains("Checkered Flag") || s.Contains("Frink-o-Matic Wasp Bumper"):
                                    Common.WriteLog($"Received {s}", "GetItems");
                                    moves.Add(s);
                                    CheckAvailableMoves(memory, CURRENTLEVEL);
                                    break;

                                case string s when s.Contains("Wallet"):
                                    Common.WriteLog($"Received {s}", "GetItems");
                                    WalletLevel++;
                                    language.SetString("APMaxCoins", (WalletLevel >= 7 ? "" : $"/{(maxCoins * WalletLevel * coinScale).ToString()}")); 
                                    break;

                                default:
                                    Common.WriteLog($"Error unlocking reward: {item}.", "GetItems");
                                    break;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"Error getting items: {ex}", "GetItems");
                }
            }
        }

        async void APLogging()
        {
            while (language == null)
            {
                await Task.Delay(100);
            }

            string log = APLog.Print();
            language.SetString("APLog", log);
        }


        bool UnlockCurrentMission(Memory memory, MissionStage? stage = null)
        {
            Common.WriteLog("Checking to unlock current mission.", "UnlockCurrentMission");
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
            Common.WriteLog($"Skipped dummy objective for L{level + 1}SD{index + 1}", "UnlockCurrentMission");
            return true;
        }

        bool UnlockMissionsPerLevel(string level)
        {
            if (level.Contains("Progressive"))
            {
                int nextLevel = UnlockedLevels.Count + 1;
                level = $"Level {nextLevel}";
            }
            UnlockedLevels.Add(level);
            int levelNum = int.Parse(Regex.Match(level, @"\d+").Value);

            for (int mission = 0; mission < 7; mission++)
            {
                string missionTitle = lt.getMissionName(mission, levelNum - 1, gameLanguage);
                Common.WriteLog(missionTitle, "UnlockMissionsPerLevel");
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
                List<string> bms = new List<string> { "bm1", "bm2" };

                if (!bms.Contains(bonusMission.Name))
                    continue;

                if (unlocked)
                {
                    Common.WriteLog($"Unlocking {bonusMission.Name}", "HandleCurrentBonusMissions");
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
                    Common.WriteLog($"Locking {bonusMission.Name}", "HandleCurrentBonusMissions");
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

        bool HandleCurrentRaces(Memory memory)
        {
            var missionManager = (memory.Globals.GameplayManager as MissionManager);
            var missions = missionManager.Missions.ToArray();
            int? level = (int)missionManager.LevelData.Level;
            bool unlocked = false;
            string character = "";

            switch (level)
            {
                case 0: character = "Homer"; break;
                case 1: character = "Bart"; break;
                case 2: character = "Lisa"; break;
                case 3: character = "Marge"; break;
                case 4: character = "Apu"; break;
                case 5: character = "Bart"; break;
                case 6: character = "Homer"; break;
            }

            if ((!checkeredflag && UnlockedLevels.Contains($"Level {level + 1}"))
                || (checkeredflag && moves.Contains($"{character} Checkered Flag")))
                unlocked = true;

            foreach (var bonusMissionInfo in missionManager.BonusMissions)
            {
                if (bonusMissionInfo.MissionNum < 0) continue; 

                var bonusMission = missions[bonusMissionInfo.MissionNum];
                List<string> bms = new List<string> { "sr1", "sr2", "sr3" };

                if (!bms.Contains(bonusMission.Name))
                    continue;

                if (unlocked)
                {
                    Common.WriteLog($"Unlocking {bonusMission.Name}", "HandleCurrentBonusMissions");
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
                    Common.WriteLog($"Locking {bonusMission.Name}", "HandleCurrentBonusMissions");
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
                    Common.WriteLog($"{name} is LOCKED", "InitializeMissionTitles");
                }
            }
        }

        public void InitializeShopItems()
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

            ac.SetShopNames(locsToScout, language);
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

            if (!moves.Contains($"{character} Progressive Jump"))
                memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.Jump);
            else
                memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.Jump);

            if (moves.Count(m => m == $"{character} Progressive Jump") < 2)
            {
                memory.Globals.CharacterTune.DoubleJumpAllowUp = float.MinValue;
                memory.Globals.CharacterTune.DoubleJumpAllowDown = float.MinValue;
            }
            else
            {
                memory.Globals.CharacterTune.DoubleJumpAllowUp = djAllowUp;
                memory.Globals.CharacterTune.DoubleJumpAllowDown = djAllowDown;
            }

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

            carwasp = moves.Contains($"{character} Frink-o-Matic Wasp Bumper") ? true : false;

        }

        async void CheckActions(Memory memory)
        {
            var rewardsManager = memory.Singletons.RewardsManager;
            if (rewardsManager == null)
                return;

            var gameplayManager = memory.Globals.GameplayManager;
            if (gameplayManager == null)
                return;

            var vehicleCentral = memory.Singletons.VehicleCentral;
            if (vehicleCentral == null)
                return;

            var lastWaspUseSize = 0;
            var beecameraHash = SHARMemory.SHAR.Helpers.MakeUID("beecamera");
            while (memory.IsRunning)
            {
                await System.Threading.Tasks.Task.Delay(100);
                if (memory.InGame())
                {
                    //Commented out in case we need to check car speed again for debugging later.
                    //Common.WriteLog(memory.Singletons.CharacterManager?.Player?.Car?.Speed.ToString()); 
                    if (DISABLEEBRAKE && memory.Singletons.CharacterManager?.Player?.Car?.Speed >= 1)
                        memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.GetOutCar);
                    else
                        memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.GetOutCar);

                    var player = memory.Singletons.CharacterManager?.Player;
                    if (player == null)
                        return;

                    var vehicles = vehicleCentral.ActiveVehicles.ToArray();
                    var defaultVehicleName = gameplayManager.DefaultLevelVehicleName;

                    if (DISABLEDEFAULT)
                    {
                        foreach (var vehicle in vehicles)
                        {
                            if (vehicle == null)
                                continue;

                            if (vehicle.Name != defaultVehicleName)
                                continue;

                            var locator = vehicle.EventLocator;
                            if (locator == null)
                                continue;

                            locator.Flags = Locator.LocatorFlags.None;
                        }
                    }
                    else
                    {
                        foreach (var vehicle in vehicles)
                        {
                            if (vehicle == null)
                                continue;

                            if (vehicle.Name != defaultVehicleName)
                                continue;

                            var locator = vehicle.EventLocator;
                            if (locator == null)
                                continue;

                            locator.Flags = Locator.LocatorFlags.Active;
                        }
                    }

                    try
                    {
                        var actorList = actorManager.ActorList;
                        if (actorList.UseSize != lastWaspUseSize)
                        {
                            Common.WriteLog($"Actor list use size changed from {lastWaspUseSize} to {actorList.UseSize}.", "CheckActions");
                            lastWaspUseSize = actorList.UseSize;

                            var actors = actorList.ToArray();
                            foreach (var actor in actors)
                            {
                                if (actor == null)
                                    continue;

                                if (actor.StateProp is not ActorDSG actorDSG)
                                    continue;

                                if (actorDSG.StateProp is not CStateProp stateProp)
                                    continue;

                                if (stateProp.Name != beecameraHash)
                                    continue;

                                if (actorDSG.SimState is not SimState simState)
                                    continue;

                                if (simState.CollisionObject is not CollisionObject collisionObject)
                                    continue;

                                if (!collisionObject.CollisionEnabled)
                                    continue;

                                Common.WriteLog($"Disabling collision on wasp at 0x{actor.Address:X} with matching hash {beecameraHash}.", "CheckActions");
                                collisionObject.CollisionEnabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        async void HandleTraps(Memory memory, string trap)
        {
            Button button;
            var buttonArray = memory.Singletons.InputManager.ControllerArray[0].ButtonArray;

            var car = memory.Singletons.CharacterManager?.Player?.Car;

            switch (trap)
            {
                case "Launch":
                    while ((car = memory.Globals.GameplayManager?.CurrentVehicle) == null)
                        await Task.Delay(100);

                    int chance = Random.Shared.Next(0, 10);
                    if (false) //not working yet.
                    {
                        VehicleCentral vc = memory.Singletons.VehicleCentral;

                        //car.Launch(50, 0);
                        foreach (Vehicle v in vc.ActiveVehicles)
                        {
                            if (v == null)
                                continue;

                            await Task.Delay(100);

                            bool isTraffic = v.VehicleType == Vehicle.VehicleTypes.Traffic;
                            if (isTraffic)
                                v.VehicleType = Vehicle.VehicleTypes.AI;

                            if (v != car)
                            {
                                Vector3 dir = Common.GetVector3Dir(v.Position, car.Position);
                                v.Launch(dir, car.Position.Y - v.Position.Y, 50);
                            }

                            if (isTraffic)
                            {
                                await Task.Delay(100); // Let the physics run a bit
                                v.VehicleType = Vehicle.VehicleTypes.Traffic;
                            }
                        }
                    }
                    else
                        car.Launch(Random.Shared.Next(20, 101), Random.Shared.Next(-20, 51));

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
                case "Eject":
                    while (car == null)
                    {
                        await Task.Delay(100);
                        car = memory.Singletons.CharacterManager?.Player?.Car;
                    }

                    while (car != null)
                    {
                        car.Stop();
                        if (memory.Singletons.CharacterManager?.Player?.Controller is CharacterController controller)
                            controller.Intention = CharacterController.Intentions.GetOutCar;
                        await Task.Delay(100);
                        car = memory.Singletons.CharacterManager?.Player?.Car;
                    }
                    break;
                case "Traffic Trap":
                    /* Loop through traffic
                       turn off coin drops
                       explode all currently spawned traffic
                       set them all the big trucks
                       make them start swerving lanes 
                       wait some seconds
                       set traffic back
                    */
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

                if (reward.QuestType == Reward.QuestTypes.BonusMission && !UnlockedItems.Contains(reward.Name))
                {
                    reward.Earned = false;
                }
            }
        }

        async void CheckGags(Memory memory)
        {
            while (memory.IsRunning)
            {
                await System.Threading.Tasks.Task.Delay(100);
                if (memory.InGame())
                {
                    if (memory?.Globals?.GameplayManager is not MissionManager missionManager)
                        break;

                    if (memory.Singletons.InteriorManager is not InteriorManager interiorManager)
                        break;

                    int? level = (int)missionManager.LevelData.Level;
                    string character = "";

                    switch (level)
                    {
                        case 0: character = "Homer"; break;
                        case 1: character = "Bart"; break;
                        case 2: character = "Lisa"; break;
                        case 3: character = "Marge"; break;
                        case 4: character = "Apu"; break;
                        case 5: character = "Bart"; break;
                        case 6: character = "Homer"; break;
                    }

                    if ((!gagfinder && !UnlockedLevels.Contains($"Level {level + 1}"))
                      || (gagfinder && !moves.Contains($"{character} Gagfinder")))
                    {
                        while (interiorManager.GagCount == 0)
                            await Task.Delay(100);

                        var gags = interiorManager.Gags.ToArray();
                        //Console.WriteLine($"{CURRENTLEVEL + 1}: Gags loadad: {string.Join("; ", gags.ToArray().Where(x => x is not null).Select(x => x.Binding!.Value.GagFileName))}");

                        foreach (var gag in gags)
                        {
                            var locator = gag?.Locator;
                            if (locator != null)
                                locator.Flags = Locator.LocatorFlags.None;
                        }
                    }
                }
            }
        }

        public void Listener_ButtonDown(Object? sender, InputListener.ButtonEventArgs e)
        {
            if (sender is not InputListener listener)
                return;

            if (!listener.memory.InGame())
                return;

            if (e.Button.ToString() == "DPadUp" || e.Button.ToString() == "D1")
            {
                if (fillerInventory["Hit N Run Reset"] > 0 && listener.memory.Singletons.HitNRunManager.CurrHitAndRun > 0f)
                {
                    listener.memory.Singletons.HitNRunManager.CurrHitAndRun = 0f;
                    fillerInventory["Hit N Run Reset"]--;
                }
                Common.WriteLog($"Hit N Run Resets: {fillerInventory["Hit N Run Reset"]}", "Listener_ButtonDown");
                ac.SetDataStorage("hnr", fillerInventory["Hit N Run Reset"]);
                language.SetString("APHnR", $"{fillerInventory["Hit N Run Reset"]:D2}");
            }
            if (e.Button.ToString() == "DPadDown" || e.Button.ToString() == "D2")
            {
                if (fillerInventory["Wrench"] > 0)
                {
                    listener.memory.Functions.TriggerEvent(Globals.Events.REPAIR_CAR);
                    fillerInventory["Wrench"]--;
                }
                Common.WriteLog($"Wrenches: {fillerInventory["Wrench"]}", "Listener_ButtonDown");
                ac.SetDataStorage("wrench", fillerInventory["Wrench"]);
                language.SetString("APWrench", $"{fillerInventory["Wrench"]:D2}");
            }
            if (e.Button.ToString() == "DPadLeft" || e.Button.ToString() == "D4")
            {
                ac.CheckVictory();
            }
        }

        Task Watcher_Error(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.Error.ErrorEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Error: {e.Exception}", "Watcher_Error");
            return Task.CompletedTask;
        }

        Task Watcher_MissionStageChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameplayManager.MissionStageChangedEventArgs e, CancellationToken token)
        {
            CURRENTLEVEL = e.Level.ToString();
            UnlockCurrentMission(sender, e.NewStage);
            HandleCurrentBonusMissions(sender);
            HandleCurrentRaces(sender);
            CheckAvailableMoves(sender, CURRENTLEVEL);
            LockSpawnedCarsOnLoad(sender, ((int)e.Level));

            if (e.Mission.ToString() == "BM2" || e.Mission.ToString() == "BM3")
            {
                Common.WriteLog($"{(int)e.Level} - bonus2", "Watcher_MissionStageChanged");
                ArchipelagoClient.sentLocations.Enqueue(lt.getAPID($"{(int)e.Level} - bonus2", "gag"));
                LockBonusCars(sender);
            }
            var characterSheet = sender.Singletons.CharacterSheetManager;

            if (characterSheet == null)
            {
                Common.WriteLog("Character sheet missing", "Watcher_CoinsChanged");
                return Task.CompletedTask;
            }
            ac.SetDataStorage("coins", characterSheet.CharacterSheet.Coins);

            return Task.CompletedTask;
        }

        async Task Watcher_CardCollected(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CardGallery.CardCollectedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"L{e.Level + 1}C{e.Card + 1} collected.", "Watcher_CardCollected");
            long location = cardIDs[e.Level * 7 + e.Card];
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("cards");
        }

        async Task Watcher_PersistentObjectDestroyed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.PersistentObjectDestroyedEventArts e, CancellationToken token)
        {
            Common.WriteLog($"Destroyed object: {e.Sector} - {e.Index}", "Watcher_PersistentObjectDestroyed");
            long location = lt.getAPID($"{e.Sector} - {e.Index}", "wasp");
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (location != -1)
            {
                if (!ac.IsLocationCheckedLocally(location))
                    await ac.IncrementDataStorage("wasps");
            }
        }

        async Task Watcher_GagViewed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.GagViewedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Gag Viewed: {e.Level} - {e.Gag}", "Watcher_GagViewed");
            long location = lt.getAPID($"{e.Level} - {e.Gag}", "gag");
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("gags");
        }

        async Task Watcher_MerchandisePurchased(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.RewardsManager.MerchandisePurchasedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Car Purchased: {e.Merchandise.Name}", "Watcher_MerchandisePurchased");
            if (e.Merchandise.Name.Contains("APCar"))
            {
                Common.WriteLog($"Sending check from {e.Merchandise.Name}", "Watcher_MerchandisePurchased");
                long location = lt.getAPID(e.Merchandise.Name, "shop");
                ArchipelagoClient.sentLocations.Enqueue(location);
                if (!ac.IsLocationCheckedLocally(location))
                    await ac.IncrementDataStorage("shops");
            }
        }

        async Task Watcher_MissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.MissionCompleteEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Mission Complete: {e.Level} - {e.Mission + 1}", "Watcher_MissionComplete");
            long location = lt.getAPID($"{e.Level} - {e.Mission + 1}", "missions");
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("missions");
        }

        async Task Watcher_BonusMissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.BonusMissionCompleteEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Mission Complete: {e.Level} - bonus", "Watcher_BonusMissionComplete");
            long location = lt.getAPID($"{e.Level} - bonus", "bonus missions");
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("bonus");

            LockBonusCars(sender);
        }

        async Task Watcher_StreetRaceComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.StreetRaceCompleteEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Race Complete: {e.Level} - {e.Race}", "Watcher_StreetRaceComplete");
            long location = lt.getAPID($"{e.Level} - {e.Race}", "bonus missions");
            ArchipelagoClient.sentLocations.Enqueue(location);
            if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("bonus");
        }

        public void UpdateProgress(int missions, int bonus, int wasps, int cards, int gags, ArchipelagoClient.VICTORY victory, int rwp, int rcp)
        {
            string ret = "";
            int wp = (int)Math.Ceiling(140 * rwp / 100.0);
            int cp = (int)Math.Ceiling(49 * rcp / 100.0); 
            

            switch (victory)
            {
                case VICTORY.FinalMission:
                    ret += "Final Mission\n";
                    break;
                case VICTORY.AllStory:
                    ret += "Story Missions\n";
                    ret += $"Missions: {missions:D2}/49\n";
                    break;
                case VICTORY.AllMissions:
                    ret = "All Missions\n";
                    ret += $"Missions: {missions:D2}/49\n";
                    ret += $"Bonus: { bonus:D2}/28\n";
                    break;
                case VICTORY.WaspsCards:
                    ret += "Wasps & Cards\n";
                    break;
                default:
                    ret += "Goal failed to load?\n";
                    break;
            }

            if (wp > 0)
                ret += $"Wasps: {wasps:D2}/{wp:D2}\n";
            if (cp > 0)
                ret += $"Cards: {cards:D2}/{cp:D2}";

            if (language != null)
                language.SetString("APProgress", ret);
        }

        Task Watcher_DialogPlaying(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.SoundManager.DialogPlayingEventArgs e, CancellationToken token)
        {
            Common.WriteLog(e.Dialog.Event, "Watcher_DialogPlaying");

            if (e.Dialog.Event.ToString() == "HAGGLING_WITH_GIL")
            {
                Common.WriteLog($"Spoke to Gil on level {CURRENTLEVEL}", "Watcher_DialogPlaying");
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

        Task Watcher_CoinsChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.CoinsChangedEventArgs e, CancellationToken token)
        {
            if (_updatingCoins)
            {
                _updatingCoins = false;
                Common.WriteLog("Suppressed CoinsChanged event.", "Watcher_CoinsChanged");
                return Task.CompletedTask;
            }

            Common.WriteLog($"Coins Changed: {e.LastCoins} to {e.NewCoins}", "Watcher_CoinsChanged");

            if (e.NewCoins > e.LastCoins)
            {
                var characterSheet = sender.Singletons.CharacterSheetManager;

                if (characterSheet == null)
                {
                    Common.WriteLog("Character sheet missing", "Watcher_CoinsChanged");
                    return Task.CompletedTask;
                }
                int amount = WalletLevel == 1 ? 0 : (WalletLevel * coinScale) - 1;

                _updatingCoins = true;
                characterSheet.CharacterSheet.Coins += amount;
                Common.WriteLog($"Added {amount} coins.", "Watcher_CoinsChanged");
                if (amount != 0)
                {
                    var remainder = e.NewCoins - e.LastCoins - amount;
                    if (remainder > 0)
                    {
                        _updatingCoins = true;
                        characterSheet.CharacterSheet.Coins += (remainder * amount);
                        Common.WriteLog($"Added {remainder * amount} coins.", "Watcher_CoinsChanged");
                    }
                    amount = 0;
                }

                int coincap = WalletLevel > 1 ? maxCoins * WalletLevel * coinScale : maxCoins;

                if (WalletLevel < 7 && characterSheet.CharacterSheet.Coins >= coincap)
                {
                    _updatingCoins = true;
                    characterSheet.CharacterSheet.Coins = coincap;
                }
            }

            return Task.CompletedTask;
        }

        Task Watcher_NewGame(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameDataManager.NewGameEventArgs e, CancellationToken token)
        {
            LoadState(sender);
            return Task.CompletedTask;
        }

        Task Watcher_RewardUnlocked(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.RewardsManager.RewardUnlockedEventArgs e, CancellationToken token)
        {
            if (e.Type is SHARMemory.SHAR.Events.RewardsManager.RewardUnlockedEventArgs.RewardType.StreetRace or SHARMemory.SHAR.Events.RewardsManager.RewardUnlockedEventArgs.RewardType.BonusMission)
                if(!UnlockedItems.Contains(e.Reward.Name))
                    e.Reward.Earned = false;
            return Task.CompletedTask;
        }

        private Task Watcher_ButtonBound(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.InputManager.ButtonBoundEventArgs e, CancellationToken token)
        {
            CheckAvailableMoves(sender, CURRENTLEVEL);
            return Task.CompletedTask;
        }

        private Task Watcher_NewTrafficVehicle(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.TrafficManager.NewTrafficVehicleEventArgs e, CancellationToken token) 
        {
            var vehicle = e.Vehicle;

            if (vehicle.EventLocator is not EventLocator eventLocator)
            {
                //Common.WriteLog("EventLocator is null, skipping.", "Watcher_NewTrafficVehicle");
                return Task.CompletedTask;
            }

            if (eventLocator.Flags == Locator.LocatorFlags.None)
            {
                //Common.WriteLog("EventLocator flags are none, skipping.", "Watcher_NewTrafficVehicle");
                return Task.CompletedTask;
            }

            if (!UnlockedItems.Contains(e.Vehicle.Name))
            {
                eventLocator.Flags = Locator.LocatorFlags.None;
                Common.WriteLog($"Setting locator flags to none for: {vehicle.Name}", "Watcher_NewTrafficVehicle");
            }

            return Task.CompletedTask;
        }

public void textDC()
        {
            language.SetString("APProgress", "Disconnected.");
            language.SetString("APLog", "Disconnected.");
        }
    }
}
