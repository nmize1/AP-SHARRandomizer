using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using SHARMemory.SHAR.Events.RewardsManager;
using SHARMemory.SHAR.Structs;
using SHARRandomizer.Classes;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using static SHARRandomizer.ArchipelagoClient;

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

        private CancellationTokenSource _cts = new();
        public CancellationToken Token => _cts.Token;

        public ArchipelagoClient ac;
        public static string? VerifyID;
        public static Queue<ScoutedItemInfo> ScoutedItems = new();
        readonly LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        readonly RewardTranslations rt = RewardTranslations.LoadFromJson("Configs/Rewards.json");
        readonly UITranslations uit = UITranslations.LoadFromJson("Configs/UITranslations.json");
        public static AwaitableQueue<string> itemsReceived = new();
        public static FixedSizeQueue<string> APLog = new(5);
        public static List<long>? cardIDs;

        readonly List<Reward> REWARDS = [];
        readonly List<string> UnlockedLevels = [];
        readonly List<string> UnlockedItems = [];
        public Dictionary<string, int> fillerInventory = [];
        readonly List<string> traps = [];
        readonly List<string> moves = [];
        FeLanguage? language = null;

        readonly float djAllowUp = 2.0f;
        readonly float djAllowDown = 12.0f;
        bool DISABLEEBRAKE = false;
        bool DISABLEDEFAULT = false;
        public string CURRENTLEVEL = "";

        public int WalletLevel = 0;
        public static int maxCoins;
        public static int coinScale;
        bool _updatingCoins = false;
        bool missionnames = true;

        public static bool gagfinder;
        public static bool checkeredflag;

        public bool carwasp = false;

        uint gameLanguage;
        private Watcher? _watcher;
        private TrapHandler _trapHandler;
        private TrapWatcher _trapWatcher;

        public MemoryManip(ArchipelagoClient ac)
        {
            this.ac = ac;
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public async Task MemoryStart()
        {
            while (!Token.IsCancellationRequested)
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
                watcher.NewParkedCar += Watcher_NewParkedCar;
                watcher.InGameWindowChanged += Watcher_InGameWindowChanged;
                watcher.CurrentEventChanged += Watcher_CurrentEventChanged;

                var trapWatcher = new TrapWatcher();
                _trapWatcher = trapWatcher;
                var trapHandler = new TrapHandler(memory, this, trapWatcher);
                _trapHandler = trapHandler;

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


            var textBible = memory.Globals.TextBible.CurrentLanguage;

            var gameVerifyID = textBible?.GetString("VerifyID");
            if (gameVerifyID != VerifyID)
            {
                textBible?.SetString("NEW_GAME", uit.GetUITranslation("IncorrectPatch", gameLanguage));
                Common.WriteLog($"SHAR.ini verification ID \"{gameVerifyID ?? "NULL"}\" does not match expected verification ID \"{VerifyID}\".", "MemoryStart");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }

            var checks = ac.GetCheckedLocations(); 
            textBible?.SetString("NEW_GAME",
                 uit.GetUITranslation(
                     checks.Count == 0 ? "NewGame" : "ResumeGame",
                     gameLanguage
            ));

            Common.WriteLog("Waiting till gameplay starts.", "InitialGameState");


            while (!Extensions.InGame(memory))
            {
                await Task.Delay(100);
            }

            /* Create default list of random shop costs, then replace it with the stored one if a stored one exists */
            if (ArchipelagoClient.ShopCosts == null)
            {
                Common.WriteLog("AP Shop Costs null. Things will not work correctly.", "InitialGameState");
                return;
            }
            List<int> ShopCosts = ArchipelagoClient.ShopCosts;

            /* Get all rewards in a list for lookup purposes */
            int s = 0;


            var rewardsList = rewardsManager.RewardsList.ToArray();
            var tokenStoreList = rewardsManager.LevelTokenStoreList.ToArray();
            for (int level = 0; level < memory.Globals.LevelCount; level++)
            {
                List<Reward> tempRewards = [];
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

            InputListener listener = new(memory);
            listener.ButtonDown += Listener_ButtonDown;

            listener.Start();

            APLog.OnEnqueue += item => APLogging();

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

            UpdateMissionTitles();
            InitializeShopItems();

            switch (WalletLevel)
            {
                case 0:
                    language.SetString("APMaxCoins", "/0");
                    break;
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
            //UpdateProgress(0, 0, 0, 0, 0, ac.victory, ac.cardAmount, ac.waspAmount);


            var characterSheet = memory.Singletons.CharacterSheetManager;
            if (characterSheet == null)
            {
                Common.WriteLog("Error getting character sheet.", "InitialGameState");
            }
            else
            {
                int h, w;
                w = await ac.GetDataStorage<int>("wrench");
                h = await ac.GetDataStorage<int>("hnr");
                fillerInventory.Add("Hit N Run Reset", h);
                fillerInventory.Add("Wrench", w);
                language.SetString("APHnR", $"{h:D2}");
                language.SetString("APWrench", $"{w:D2}");
                characterSheet.CharacterSheet.ItchyScratchyTicket = true;
                CheckTicketRequirements(memory, characterSheet);
            }

            traps.AddRange(new List<string> { "Hit N Run", "Reset Car", "Duff Trap", "Eject", "Launch", "Traffic Trap" });
            _ = Task.Run(async () =>
            {
                try
                {
                    await CheckActions(memory);
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"{ex}", "CheckActions");
                }
            });
            _ = Task.Run(async () =>
            {
                try
                {
                    await CheckGags(memory);
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"{ex}", "CheckGags");
                }
            });
            /*
            _ = Task.Run(async () =>
            {
                try
                {
                    await UpdateData(memory);
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"{ex}", "UpdateData");
                }
            });
            */
            _ = Task.Run(async () =>
            {
                try
                {
                    await CardRadar(memory);
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"{ex}", "CardRadar");
                }
            });
        }

        bool CheckTicketRequirements(Memory memory, CharacterSheetManager characterSheet)
        {
            VICTORY goal = ac.victory;
            LevelRecord[] record = characterSheet.CharacterSheet.LevelList.ToArray();
            bool success = true;
            int wasps = 0;
            int cards = 0;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (record[i].Cards.List[j].Completed)
                        cards++;
                }
                wasps += record[i].WaspsDestroyed;
            }

            if (wasps < ac.waspAmount || cards < ac.cardAmount)
                success = false;

            if (goal == VICTORY.AllMissions || goal == VICTORY.AllStory)
            {
                int finMission = 0;
                int finBonus = 0;
                for (int i = 0; i < memory.Globals.LevelCount; i++)
                {
                    if (ac.requiredLevels[i])
                    {
                        MissionList missions = record[i].Missions;
                        for (int j = 0; j < 8; j++)
                        {
                            if (!missions.List[j].Completed)
                                success = false;
                            else
                                finMission++;
                        }

                        if (goal == VICTORY.AllMissions)
                        {
                            if (!record[i].BonusMission.Completed)
                                success = false;
                            else
                                finBonus++;

                            StreetRaceList races = record[i].StreetRaces;
                            for (int j = 0; j < 3; j++)
                            {
                                if (!races.List[j].Completed)
                                    success = false;
                                else
                                    finBonus++;
                            }

                            Console.WriteLine(races);
                        }
                    }
                }
                UpdateProgress(finMission, finBonus, wasps, cards, 0, goal, ac.waspAmount, ac.cardAmount);
            }
            else if (goal == VICTORY.Cars)
            {
                var rewardsManager = memory.Singletons.RewardsManager;
                if (rewardsManager == null)
                {
                    Common.WriteLog("Error accessing rewardsManager.", "CheckTicketRequirements");
                    return false;
                }

                int carCount = 0;

                foreach(var level in rewardsManager.RewardsList)
                {
                    if (level.DefaultCar.Earned)
                        carCount++;
                    if (level.BonusMission.Earned)
                        carCount++;
                    if (level.StreetRace.Earned)
                        carCount++;
                }

                var tokenStoreList = rewardsManager.LevelTokenStoreList.ToArray();
                for (int level = 0; level < memory.Globals.LevelCount; level++)
                {
                    var levelTokenStoreList = tokenStoreList[level];
                    for (int merchandiseIndex = 0; merchandiseIndex < levelTokenStoreList.Counter; merchandiseIndex++)
                    {
                        var merchandise = memory.Functions.GetMerchandise(level, merchandiseIndex);

                        if (merchandise == null)
                            continue;
                        if (merchandise.Earned && merchandise.RewardType == Reward.RewardTypes.PlayerCar)
                            carCount++;
                    }
                }

                if (carCount < ac.carAmount)
                    success = false;


                UpdateProgress(0, 0, wasps, cards, carCount, goal, ac.waspAmount, ac.cardAmount);
            }
            else if (goal == VICTORY.FinalMission)
            {
                if (!record[6].Missions.List[6].Completed)
                    success = false;
                
                UpdateProgress(0, 0, wasps, cards, 0, goal, ac.waspAmount, ac.cardAmount);
            }
            else
            {
                Common.WriteLog("Goal not found.", "CheckGoal");
                return false;
            }

            return success;
        }

        void CheckGiveTicket(Memory memory)
        {
            var characterSheet = memory.Singletons.CharacterSheetManager;

            if (characterSheet == null)
            {
                Common.WriteLog("Character sheet missing", "CheckGoal");
                return;
            }
            if (CheckTicketRequirements(memory, characterSheet))
            {
                characterSheet.CharacterSheet.ItchyScratchyCBGFirst = true;
                var lvl3Data = characterSheet.CharacterSheet.LevelList[2];
                lvl3Data.FMVUnlocked = true;
                characterSheet.CharacterSheet.LevelList[2] = lvl3Data;
            }
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
            List<long> locations = ac.GetCheckedLocations(); //collected checks get restored
            //await ac.GetDataStorage<List<long>>("localchecks"); //collected checks don't get restored (maybe option eventually)
            
            /* get struct from characterSheet.LevelList.ToArray() to update below */
            LevelRecord[] record = characterSheet.CharacterSheet.LevelList.ToArray();
            int[] waspCounters = new int[7];
            int[] gagCounters = new int[7];
            uint[] gagmask = new uint[7];
            List<string> purchased = [];

            foreach (var l in locations)
            {
                var check = lt.getTypeAndNameByAPID(l);
                string id = lt.GetIDByAPID(l);

                if (string.IsNullOrEmpty(id))
                {
                    if (cardIDs != null && cardIDs.Contains(l))
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
                            if (cardIDs == null)
                            {
                                Common.WriteLog($"{nameof(cardIDs)} is null.", "LoadState");
                                break;
                            }
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
                            if (!int.TryParse(id[0].ToString(), out level))
                            {
                                Common.WriteLog($"Invalid gag ID format: {id}", "LoadState");
                                break;
                            }
                            if (!int.TryParse(id[4].ToString(), out var gag))
                            {
                                Common.WriteLog($"Invalid gag ID format: {id}", "LoadState");
                                break;
                            }

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
                                case string s when (s.StartsWith("Level ") || s.StartsWith("Progressive Level")) && !s.StartsWith("Progressive Wallet"):
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
                                    if (fillerInventory[s] < 101)
                                    {
                                        switch (s)
                                        {
                                            case "Hit N Run Reset":
                                                await ac.IncrementDataStorage("hnr");
                                                language?.SetString("APHnR", $"{fillerInventory[s]:D2}");
                                                break;

                                            case "Wrench":
                                                await ac.IncrementDataStorage("wrench");
                                                language?.SetString("APWrench", $"{fillerInventory[s]:D2}");
                                                break;

                                        }
                                    }
                                    Common.WriteLog($"Received {s}.", "GetItems");
                                    break;

                                case string s when traps.Contains(s):
                                    Common.WriteLog($"Received TRAP {s}.", "GetItems");
                                    _trapWatcher.OnTrapDetected(s);
                                    break;

                                case string s when s.Contains("Jump") || s.Contains("Attack") || s.Contains("Brake") || s.Contains("Forward") ||
                                                   s.Contains("Gagfinder") || s.Contains("Checkered Flag") || s.Contains("Frink-o-Matic Wasp Bumper"):
                                    Common.WriteLog($"Received {s}", "GetItems");
                                    moves.Add(s);
                                    CheckAvailableMoves(memory, CURRENTLEVEL);
                                    HandleCurrentRaces(memory);
                                    //CheckGags(memory);
                                    break;

                                case string s when s.Contains("Wallet"):
                                    Common.WriteLog($"Received {s}", "GetItems");
                                    WalletLevel++;
                                    int coincap = WalletLevel == 1 ? maxCoins : maxCoins * WalletLevel * coinScale;
                                    language?.SetString("APMaxCoins", (WalletLevel >= 7 ? "" : $"/{coincap.ToString()}"));
                                    UpdateCoinDrops(memory);                                    
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

        void UpdateCoinDrops(Memory memory)
        {
            //(WalletLevel * coinScale)
            if (WalletLevel > 1)
            {
                var hnr = memory.Singletons.HitNRunManager;
                hnr.HitBreakableCoins = WalletLevel * coinScale;
                hnr.HitKrustyGlassCoins = 5 * WalletLevel * coinScale;
                hnr.HitMoveableCoins = WalletLevel * coinScale;
                hnr.ColaPropDestroyedCoins = 10 * WalletLevel * coinScale;

                hnr.BustedCoins = 50 * WalletLevel * coinScale;
            }
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
            
            UpdateMissionTitles();

            return true;
        }

        bool HandleCurrentBonusMissions(Memory memory)
        {
            if (memory.Globals.GameplayManager is not MissionManager missionManager)
                return false;
            var missions = missionManager.Missions.ToArray();
            int? level = (int)missionManager.LevelData.Level;
            bool unlocked = false;
            if (UnlockedLevels.Contains($"Level {level + 1}")) // Skip if level item already received
                unlocked = true;

            foreach (var bonusMissionInfo in missionManager.BonusMissions)
            {
                if (bonusMissionInfo.MissionNum < 0) continue; // Avoids empty bonus mission slots

                var bonusMission = missions[bonusMissionInfo.MissionNum];
                List<string> bms = ["bm1", "bm2"];

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
            if (memory.Globals.GameplayManager is not MissionManager missionManager)
                return false;
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
                List<string> bms = ["sr1", "sr2", "sr3"];

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
                        foreach  (var element in compositeDrawable.Elements)
                        {
                            element.Visible = false;
                        }
                    }
                }
            }

            return true;
        }

        void UpdateMissionTitles()
        {
            if (missionnames)
            {
                string name = $"MISSION_TITLE_L{0}_M{0}";
                language?.SetString(name, "LOCKED");
                for (int level = 0; level < 7; level++)
                {
                    language?.SetString($"LEVEL_{level + 1}", $"Level {level + 1} Missions");
                    if (!UnlockedLevels.Contains($"Level {level + 1}"))
                    {
                        string replacement = ac.levelLock ? "LOCKED" : "Free Roam Available";
                        for (int mission = 0; mission < 7; mission++)
                        {
                            name = $"MISSION_TITLE_L{level + 1}_M{mission + 1}";
                            language?.SetString(name, replacement);
                        }
                    }
                    else
                    {
                        for (int mission = 0; mission < 7; mission++)
                        {
                            string missionTitle = lt.getMissionName(mission, level, gameLanguage);
                            name = $"MISSION_TITLE_L{level + 1}_M{mission + 1}";
                            language?.SetString(name, missionTitle.Trim());
                        }
                    }
                }
            }
            else
            {
                for (int level = 0; level < 7; level++)
                {
                    language?.SetString($"LEVEL_{level + 1}", $"Level {level + 1} Teleports");
                }
                language?.SetString($"MISSION_TITLE_L{1}_M{1}", "Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{1}_M{2}", "Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{1}_M{3}", "Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{1}_M{4}", "Power Plant");
                language?.SetString($"MISSION_TITLE_L{1}_M{5}", "Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{1}_M{6}", "Grocery Store");
                language?.SetString($"MISSION_TITLE_L{1}_M{7}", "Power Plant Parking Lot");
                        
                language?.SetString($"MISSION_TITLE_L{2}_M{1}", "Park");
                language?.SetString($"MISSION_TITLE_L{2}_M{2}", "Herman's Military Antiques");
                language?.SetString($"MISSION_TITLE_L{2}_M{3}", "Googolplex");
                language?.SetString($"MISSION_TITLE_L{2}_M{4}", "Springfield Stadium");
                language?.SetString($"MISSION_TITLE_L{2}_M{5}", "Construction Krusty Burger");
                language?.SetString($"MISSION_TITLE_L{2}_M{6}", "Springfield Stadium");
                language?.SetString($"MISSION_TITLE_L{2}_M{7}", "Springfield Stadium");
                        
                language?.SetString($"MISSION_TITLE_L{3}_M{1}", "The Android Dungeon");
                language?.SetString($"MISSION_TITLE_L{3}_M{2}", "Across From Krusty Burger");
                language?.SetString($"MISSION_TITLE_L{3}_M{3}", "Krusty Burger");
                language?.SetString($"MISSION_TITLE_L{3}_M{4}", "Observatory Overlook");
                language?.SetString($"MISSION_TITLE_L{3}_M{5}", "Casino");
                language?.SetString($"MISSION_TITLE_L{3}_M{6}", "Captain Chum 'N' Stuff");
                language?.SetString($"MISSION_TITLE_L{3}_M{7}", "Captain Chum 'N' Stuff");
                        
                language?.SetString($"MISSION_TITLE_L{4}_M{1}", "Inside Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{4}_M{2}", "Cletus' House");
                language?.SetString($"MISSION_TITLE_L{4}_M{3}", "Gas Station");
                language?.SetString($"MISSION_TITLE_L{4}_M{4}", "Cemetary");
                language?.SetString($"MISSION_TITLE_L{4}_M{5}", "Springfield Retirement Castle");
                language?.SetString($"MISSION_TITLE_L{4}_M{6}", "Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{4}_M{7}", "Kwik-E-Mart");
                        
                language?.SetString($"MISSION_TITLE_L{5}_M{1}", "Googolplex");
                language?.SetString($"MISSION_TITLE_L{5}_M{2}", "The Legitimate Businessman's Social Club");
                language?.SetString($"MISSION_TITLE_L{5}_M{3}", "General Hospital");
                language?.SetString($"MISSION_TITLE_L{5}_M{4}", "Construction Krusty Burger");
                language?.SetString($"MISSION_TITLE_L{5}_M{5}", "DMV");
                language?.SetString($"MISSION_TITLE_L{5}_M{6}", "DMV");
                language?.SetString($"MISSION_TITLE_L{5}_M{7}", "Lexicon Bookstore");
                        
                language?.SetString($"MISSION_TITLE_L{6}_M{1}", "Across From Krusty Burger");
                language?.SetString($"MISSION_TITLE_L{6}_M{2}", "KrustyLu Studios");
                language?.SetString($"MISSION_TITLE_L{6}_M{3}", "Squidport Entrance");
                language?.SetString($"MISSION_TITLE_L{6}_M{4}", "Observatory");
                language?.SetString($"MISSION_TITLE_L{6}_M{5}", "Call Me Delish-Mael Taffy Shop");
                language?.SetString($"MISSION_TITLE_L{6}_M{6}", "KrustyLu Studios");
                language?.SetString($"MISSION_TITLE_L{6}_M{7}", "Krusty Burger");
                        
                language?.SetString($"MISSION_TITLE_L{7}_M{1}", "Inside Simpsons' House");
                language?.SetString($"MISSION_TITLE_L{7}_M{2}", "School Playground");
                language?.SetString($"MISSION_TITLE_L{7}_M{3}", "Power Plant Parking Lot");
                language?.SetString($"MISSION_TITLE_L{7}_M{4}", "Inside School");
                language?.SetString($"MISSION_TITLE_L{7}_M{5}", "Power Plant Parking Lot");
                language?.SetString($"MISSION_TITLE_L{7}_M{6}", "School Playground");
                language?.SetString($"MISSION_TITLE_L{7}_M{7}", "School Playground");
            }
        }

        public void InitializeShopItems()
        {
            Dictionary<long, string> locsToScout = [];
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

        public void CheckAvailableMoves(Memory memory, string level)
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

            if (!moves.Contains($"{character} Forward"))
            {
                memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.MoveUp);
                memory.Singletons.InputManager.ControllerArray[0].DisableButton(InputManager.Buttons.Accelerate);
            }
            else
            {
                memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.MoveUp);
                memory.Singletons.InputManager.ControllerArray[0].EnableButton(InputManager.Buttons.Accelerate);
            }

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

        async Task CheckActions(Memory memory)
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

                    if (memory.Singletons.ActorManager is ActorManager actorManager)
                    {
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

                                    if (carwasp)
                                    {
                                        Common.WriteLog($"Enabling collision on wasp at 0x{actor.Address:X} with matching hash {beecameraHash}.", "CheckActions");
                                        collisionObject.CollisionEnabled = true;
                                    }
                                    else
                                    {
                                        Common.WriteLog($"Disabling collision on wasp at 0x{actor.Address:X} with matching hash {beecameraHash}.", "CheckActions");
                                        collisionObject.CollisionEnabled = false;
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

        async Task CheckGags(Memory memory)
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

                    while (interiorManager.GagCount == 0)
                        await Task.Delay(100);

                    var gags = interiorManager.Gags.ToArray();
                    foreach (var gag in gags)
                    {
                        var locator = gag?.Locator;
                        if ((!gagfinder && !UnlockedLevels.Contains($"Level {level + 1}"))
                            || (gagfinder && !moves.Contains($"{character} Gagfinder")))
                        {
                            if (locator != null)
                                locator.Flags = Locator.LocatorFlags.None;
                        }
                        else
                        {
                            if (locator != null)
                                locator.Flags = Locator.LocatorFlags.Active;
                        }
                    }
                }
            }
        }

        /*
        async Task UpdateData(Memory memory)
        {
            while (memory.IsRunning)
            {
                Common.WriteLog("Updating", "UpdateData");
                await ac.CheckVictory();
                await Task.Delay(30000);
            }
        }
        */
       

        async Task CardRadar(Memory memory)
        {
            var rumble = new InputListener(memory);
            var characterSheet = memory.Singletons.CharacterSheetManager;
            if (characterSheet == null)
            {
                Common.WriteLog("Character sheet missing", "LoadState");
                return;
            }
            LevelRecord[] record = characterSheet.CharacterSheet.LevelList.ToArray();

            
            while (memory.IsRunning)
            {
                
            }
        }

        public async void Listener_ButtonDown(Object? sender, InputListener.ButtonEventArgs e)
        {
            if (sender is not InputListener listener)
                return;

            /* Things that can be done in the pause menu */
            if (!listener.memory.InGame())
            {
                if (e.Button.ToString() == "LeftShoulder" || e.Button.ToString() == "D8")
                {
                    missionnames = true;
                    UpdateMissionTitles();
                }
                if (e.Button.ToString() == "RightShoulder" || e.Button.ToString() == "D9")
                {
                    missionnames = false;
                    UpdateMissionTitles();
                }
            }
            /* Things that can't be done in the pause menu */
            else
            {
                if (e.Button.ToString() == "DPadUp" || e.Button.ToString() == "D1")
                {

                    CheckGiveTicket(listener.memory);
                    if (fillerInventory["Hit N Run Reset"] > 0 && listener.memory.Singletons.HitNRunManager.CurrHitAndRun > 0f)
                    {
                        listener.memory.Singletons.HitNRunManager.CurrHitAndRun -= ac.hnrEfficiency;
                        fillerInventory["Hit N Run Reset"]--;
                    }
                    Common.WriteLog($"Hit N Run Resets: {fillerInventory["Hit N Run Reset"]}", "Listener_ButtonDown");
                    await ac.SetDataStorage("hnr", fillerInventory["Hit N Run Reset"]);
                    language?.SetString("APHnR", $"{fillerInventory["Hit N Run Reset"]:D2}");
                }
                if (e.Button.ToString() == "DPadDown" || e.Button.ToString() == "D2")
                {
                    if (fillerInventory["Wrench"] > 0)
                    {
                        var car = listener.memory.Singletons.CharacterManager?.Player?.Car;
                        if (car != null)
                        {
                            car.HitPoints += ac.wrenchEfficiency;
                            fillerInventory["Wrench"]--;
                        }
                    }
                    Common.WriteLog($"Wrenches: {fillerInventory["Wrench"]}", "Listener_ButtonDown");
                    await ac.SetDataStorage("wrench", fillerInventory["Wrench"]);
                    language?.SetString("APWrench", $"{fillerInventory["Wrench"]:D2}");
                }
            }
        }

        Task Watcher_Error(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.Error.ErrorEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Error: {e.Exception}", "Watcher_Error");
            return Task.CompletedTask;
        }

        Task Watcher_MissionStageChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameplayManager.MissionStageChangedEventArgs e, CancellationToken token)
        {
            if (!e.Level.HasValue)
                return Task.CompletedTask;

            CURRENTLEVEL = e.Level.Value.ToString();
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

            if (sender.Globals.GameplayManager is MissionManager missionManager)
            {
                var mission = missionManager.GetCurrentMission();
                if (mission is not null)
                {
                    var timer = mission.MissionTimer;
                    if (timer > 0)
                    {
                        timer = (int)Math.Ceiling(timer * ac.timerMod);
                    }
                }
            }

            //var characterSheet = sender.Singletons.CharacterSheetManager;

            //if (characterSheet == null)
            //{
            //    Common.WriteLog("Character sheet missing", "Watcher_MissionStageChanged");
            //    return;
            //}

            return Task.CompletedTask;
        }

        Task SendLocation(long location, Memory memory)
        {
            ArchipelagoClient.sentLocations.Enqueue(location);
            CheckGiveTicket(memory);
            Common.WriteLog($"Sending {location}", "SendLocation");

            return Task.CompletedTask;
        } 

        async Task Watcher_CardCollected(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CardGallery.CardCollectedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"L{e.Level + 1}C{e.Card + 1} collected.", "Watcher_CardCollected");
            if (cardIDs == null)
                return;

            long location = cardIDs[e.Level * 7 + e.Card];
            await SendLocation(location, sender);
            /*if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("cards");*/
        }

        async Task Watcher_PersistentObjectDestroyed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.PersistentObjectDestroyedEventArts e, CancellationToken token)
        {
            Common.WriteLog($"Destroyed object: {e.Sector} - {e.Index}", "Watcher_PersistentObjectDestroyed");
            long location = lt.getAPID($"{e.Sector} - {e.Index}", "wasp");
            await SendLocation(location, sender);
            /*if (location != -1)
            {
                if (!ac.IsLocationCheckedLocally(location))
                    await ac.IncrementDataStorage("wasps");
            }*/
        }

        async Task Watcher_GagViewed(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.GagViewedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Gag Viewed: {e.Level} - {e.Gag}", "Watcher_GagViewed");
            long location = lt.getAPID($"{e.Level} - {e.Gag}", "gag");
            await SendLocation(location, sender);
            /*if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("gags");*/
        }

        async Task Watcher_MerchandisePurchased(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.RewardsManager.MerchandisePurchasedEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Car Purchased: {e.Merchandise.Name}", "Watcher_MerchandisePurchased");
            if (e.Merchandise.Name.Contains("APCar"))
            {
                Common.WriteLog($"Sending check from {e.Merchandise.Name}", "Watcher_MerchandisePurchased");
                long location = lt.getAPID(e.Merchandise.Name, "shop");
                await SendLocation(location, sender);
                /*if (!ac.IsLocationCheckedLocally(location))
                    await ac.IncrementDataStorage("shops");*/
            }
        }

        async Task Watcher_MissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.MissionCompleteEventArgs e, CancellationToken token)
        {
            Common.WriteLog($"Mission Complete: {e.Level + 1} - {e.Mission + 1}", "Watcher_MissionComplete");
            long location = lt.getAPID($"{e.Level} - {e.Mission + 1}", "missions"); //levels are 0 indexed and missions aren't in this context
            await SendLocation(location, sender);
            /*if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("missions");*/
        }

        async Task Watcher_BonusMissionComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.BonusMissionCompleteEventArgs e, CancellationToken token)
        {
            var textBible = sender.Globals.TextBible.CurrentLanguage;
            textBible?.SetString($"RACE_COMPLETE_INFO_ALL_{e.Level}", ac.ExtraHint());

            Common.WriteLog($"Mission Complete: {e.Level} - bonus", "Watcher_BonusMissionComplete");
            long location = lt.getAPID($"{e.Level} - bonus", "bonus missions");
            await SendLocation(location, sender);
            /*if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("bonus");*/

            LockBonusCars(sender);
        }

        async Task Watcher_StreetRaceComplete(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.StreetRaceCompleteEventArgs e, CancellationToken token)
        {
            var characterSheet = sender.Singletons.CharacterSheetManager;
            LevelRecord[] record = characterSheet.CharacterSheet.LevelList.ToArray();
            StreetRaceList races = record[e.Level].StreetRaces;
            var textBible = sender.Globals.TextBible.CurrentLanguage;

            if (races.List.All(r => r.Completed))
            {
                textBible?.SetString($"RACE_COMPLETE_INFO_ALL_{e.Level}", ac.ExtraHint());
            }

            Common.WriteLog($"Race Complete: {e.Level} - {e.Race}", "Watcher_StreetRaceComplete");
            long location = lt.getAPID($"{e.Level} - {e.Race}", "bonus missions");
            await SendLocation(location, sender);
            /*if (!ac.IsLocationCheckedLocally(location))
                await ac.IncrementDataStorage("bonus");*/
        }

        public void UpdateProgress(int missions, int bonus, int wasps, int cards, int cars, ArchipelagoClient.VICTORY victory, int rw, int rc)
        {
            string ret = "";
            int numLevels = ac.requiredLevels.Count(b => b);
            List<int> reqLvls = ac.requiredLevels.Select((value, index) => new { value, index }).Where(x => x.value).Select(x => x.index + 1).ToList();
            int totMiss = numLevels * 7;
            int totBon = numLevels * 4;

            switch (victory)
            {
                case VICTORY.FinalMission:
                    ret += "Final Mission\n";
                    break;
                case VICTORY.AllStory:
                    ret += "Story Missions\n";
                    ret += $"Required Levels:\n{string.Join(", ", reqLvls)}\n";
                    ret += $"Missions: {missions:D2}/{totMiss}\n";
                    break;
                case VICTORY.AllMissions:
                    ret = "All Missions\n";
                    ret += $"Required Levels\n{string.Join(", ", reqLvls)}\n";
                    ret += $"Missions: {missions:D2}/{totMiss}\n";
                    ret += $"Bonus: { bonus:D2}/{totBon}\n";
                    break;
                case VICTORY.Cars:
                    ret += "Cars:\n";
                    ret += $"Cars: {cars:D2}/81";
                    break;
                default:
                    ret += "Goal failed to load?\n";
                    break;
            }

            if (rw > 0)
                ret += $"Wasps: {wasps:D2}/{rw:D2}\n";
            if (rc > 0)
                ret += $"Cards: {cards:D2}/{rc:D2}";

            if (language != null)
                language.SetString("APProgress", ret);
        }

        Task Watcher_DialogPlaying(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.SoundManager.DialogPlayingEventArgs e, CancellationToken token)
        {
            Common.WriteLog(e.Dialog.Event, "Watcher_DialogPlaying");

            if (e.Dialog.Event.ToString() == "HAGGLING_WITH_GIL")
            {
                Common.WriteLog($"Spoke to Gil on level {CURRENTLEVEL}", "Watcher_DialogPlaying");
                Dictionary<long, string> locsToScout = [];
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

        async Task Watcher_CoinsChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CharacterSheet.CoinsChangedEventArgs e, CancellationToken token)
        {
            var characterSheet = sender.Singletons.CharacterSheetManager;
            int coincap = WalletLevel > 1 ? maxCoins * WalletLevel * coinScale : maxCoins;

            if (WalletLevel == 0)
                characterSheet.CharacterSheet.Coins = 0;
            else if (WalletLevel < 7 && characterSheet.CharacterSheet.Coins >= coincap)
                characterSheet.CharacterSheet.Coins = coincap;


            await ac.SetDataStorage("coins", characterSheet.CharacterSheet.Coins);
        }

        async Task Watcher_NewGame(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.GameDataManager.NewGameEventArgs e, CancellationToken token)
        {
            await LoadState(sender);
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
                //Common.WriteLog($"Setting locator flags to none for: {vehicle.Name}", "Watcher_NewTrafficVehicle");
            }

            return Task.CompletedTask;
        }

        /* Exception to no late variables because these are just used here */
        private CancellationTokenSource? _ingameWindowCTS = null;
        private readonly object _ingameWindowLock = new();

        private Task Watcher_InGameWindowChanged(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.CGuiSystem.InGameWindowChangedEventArgs e, CancellationToken token)
        {
            var textBible = sender.Globals.TextBible.CurrentLanguage;

            lock (_ingameWindowLock)
            {
                _ingameWindowCTS?.Cancel();
                _ingameWindowCTS?.Dispose();
                _ingameWindowCTS = null;
            }

            switch (e.NewID)
            {
                case CGuiManager.WindowID.PhoneBooth:
                    if (e.NewWindow is not CGuiScreenPhoneBooth guiScreenPhoneBooth)
                        break;

                    Common.WriteLog($"Entered phonebooth: {guiScreenPhoneBooth.NumPreviewVehicles} preview vehicles", "InGameWindowChanged_Phonebooth");

                    var previewVehicles = guiScreenPhoneBooth.PreviewVehicles;
                    var vehicles = previewVehicles.ToArray().OrderByDescending(x => x.IsUnlocked).ThenBy(x => x.Name);
                    previewVehicles.FromArray(vehicles.ToArray());
                    guiScreenPhoneBooth.CurrentPreviewVehicle = 0;
                    guiScreenPhoneBooth.NumPreviewVehicles = vehicles.Count(x => x.IsUnlocked);

                    break;
                case CGuiManager.WindowID.PurchaseRewards:
                    if (e.NewWindow is not CGuiScreenPurchaseRewards guiScreenPurchaseRewards)
                        break;
                    if (guiScreenPurchaseRewards.CurrentType.ToString() == "Interior")
                    {
                        //if (guiScreenPurchaseRewards.RewardPrice is FeDrawable rewardPrice)
                        //    rewardPrice.Visible = false;

                        textBible?.SetString("COINS", " ");
                        textBible?.SetString("TO_PURCHASE", "LOCKED");
                    }

                    break;
                case CGuiManager.WindowID.MissionSelect:
                    if (e.NewWindow is not CGuiScreenMissionSelect missionSelect)
                        break;
                    UpdateMissionTitles();

                    if (!ac.levelLock)
                        break;

                    if (_ingameWindowCTS != null)
                        break;

                    bool[][] lockedLevels = new bool[7][];
                    for (int level = 0; level < 7; level++)
                    {
                        lockedLevels[level] = new bool[7];
                        if(!UnlockedLevels.Contains($"Level {level + 1}"))
                            Array.Fill(lockedLevels[level], true);
                    }


                    lock (_ingameWindowLock)
                    {
                        _ingameWindowCTS = new();
                        var windowToken = _ingameWindowCTS.Token;

                        Task.Run(async () =>
                        {
                            while (!windowToken.IsCancellationRequested)
                            {
                                try
                                {
                                    var menuLevel = missionSelect.MenuLevel;
                                    if (menuLevel == null)
                                        continue;

                                    var levelText = (SHARMemory.SHAR.Classes.GuiMenuItemText)menuLevel.MenuItems[0];
                                    var level = levelText.Item.Index;

                                    var menu = missionSelect.Menu;

                                    var menuItems = menu.MenuItems.ToArray();
                                    for (int i = 0; i < menu.NumItems; i++)
                                    {
                                        var menuItem = (SHARMemory.SHAR.Classes.GuiMenuItemText)menuItems[i];
                                        if (lockedLevels[level][i])
                                        {
                                            menuItem.Attributes &= ~(SHARMemory.SHAR.Classes.GuiMenuItem.Attribute.SelectionEnabled | SHARMemory.SHAR.Classes.GuiMenuItem.Attribute.Selectable);
                                            menuItem.Item.Colour = SHARMemory.SHAR.Classes.CGuiMenu.DEFAULT_DISABLED_ITEM_COLOUR;
                                        }
                                        else
                                        {
                                            menuItem.Attributes |= SHARMemory.SHAR.Classes.GuiMenuItem.Attribute.SelectionEnabled | SHARMemory.SHAR.Classes.GuiMenuItem.Attribute.Selectable;
                                            menuItem.Item.Colour = menuItem.DefaultColour;
                                        }
                                    }

                                    await Task.Delay(10, windowToken);
                                }
                                catch (TaskCanceledException)
                                {
                                    // Exited mission select
                                }
                            }
                        }, windowToken);
                    }
                    break;
                default:
                    break;
            }
            switch (e.OldID)
            {
                case CGuiManager.WindowID.PhoneBooth:
                    UpdateMissionTitles();
                    break;
                case CGuiManager.WindowID.PurchaseRewards:
                    textBible.SetString("COINS", "coins");
                    textBible.SetString("TO_PURCHASE", "to purchase");
                    break;
                case CGuiManager.WindowID.MissionSelect:
                    for (int i = 1; i < 8; i++)
                        textBible?.SetString($"LEVEL_{i}", $"                    Level {i}");
                    break;
            }

            return Task.CompletedTask;
        }

        private Task Watcher_CurrentEventChanged(SHARMemory.SHAR.Memory memory, SHARMemory.SHAR.Events.PresentationManager.CurrentEventChangedEventArgs e, CancellationToken token)
        {
            if (e.NewEvent?.FileName == "movies\\fmv8.rmv")
            {
                ac.SendCompletion();
            }

            return Task.CompletedTask;
        }

        private Task Watcher_NewParkedCar(SHARMemory.SHAR.Memory sender, SHARMemory.SHAR.Events.ParkedCarManager.NewParkedCarEventArgs e, CancellationToken token)
        {
            if (!UnlockedItems.Contains(e.Vehicle.Name))
            {
                Console.WriteLine($"Locking new parked car: {e.Vehicle} ({e.Vehicle.Name})");
                e.Vehicle.EventLocator.Flags = SHARMemory.SHAR.Classes.Locator.LocatorFlags.None;
            }

            return Task.CompletedTask;
        }

        public void textDC()
        {
            language?.SetString("APProgress", "Disconnected.");
            language?.SetString("APLog", "Disconnected.");
        }
    }
}
