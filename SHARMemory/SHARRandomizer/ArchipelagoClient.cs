﻿using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using SHARRandomizer.Classes;
using SHARMemory.SHAR.Classes;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using Archipelago.MultiClient.Net.Packets;


namespace SHARRandomizer
{
    public class ArchipelagoClient
    {
        public MemoryManip mm;
        List<string> NORESEND = new List<string>() { "Wrench", "10 Coins", "Hit N Run Reset"};
        private const string MinArchipelagoVersion = "0.5.0"; //update to .6.0 soon
        public static AwaitableQueue<long> sentLocations = new AwaitableQueue<long>(); 

        public bool Connected => _session?.Socket.Connected ?? false;
        private bool _attemptingConnection;

        private ArchipelagoSession _session;
        private bool _ignoreLocations;

        public string URI = "";
        public string SLOTNAME = "";
        public string PASSWORD = "";

        public static string SaveName;
        LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        public static List<int> ShopCosts;

        public enum VICTORY
        {
            AllMissions = 0,
            AllStory = 1,
            FinalMission = 2,
            WaspsCards = 3
        }

        public VICTORY victory = VICTORY.FinalMission;
        public int cardPercent = 0;
        public int waspPercent = 0;


        public enum ShopHintPolicy
        {
            All = 0,
            OnlyProg = 1
        }
        public ShopHintPolicy shp = ShopHintPolicy.All;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0; // Width of primary screen in pixels

        private static int GetScreenWidthInPixels()
        {
            return GetSystemMetrics(SM_CXSCREEN);
        }

        public void Connect()
        {
            if (Connected || _attemptingConnection)
            {
                return;
            }


            if (string.IsNullOrWhiteSpace(URI) || string.IsNullOrWhiteSpace(SLOTNAME))
            {
                return;
            }

            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(URI);
                SetupSession();
            }
            catch (Exception e)
            {
                Common.WriteLog($"Error connecting: {e}", "ArchipelagoClient::Connect");
            }

            TryConnect();
        }

        private void SetupSession()
        {
            _session.Socket.ErrorReceived += Session_ErrorReceived;
            _session.Socket.SocketClosed += Session_SocketClosed;
            _session.Items.ItemReceived += Session_ItemReceived;
            _session.MessageLog.OnMessageReceived += Session_OnMessageReceived;
        }

        private async void TryConnect()
        {
            LoginResult loginResult;
            _attemptingConnection = true;
            _ignoreLocations = true;

            do
            {
                try
                {
                    if (_session == null)
                    {
                        try
                        {
                            _session = ArchipelagoSessionFactory.CreateSession(URI);
                            SetupSession();
                        }
                        catch (Exception e)
                        {
                            Common.WriteLog($"Error creating session.", "ArchipelagoClient::TryConnect");
                            loginResult = new LoginFailure("Session creation failed.");
                            if (await HandleRetryDelayAsync())
                            {
                                URI = GetURI();
                                SLOTNAME = GetSlotName();
                                PASSWORD = GetPassword();
                            }
                            continue;
                        }
                    }

                    loginResult = _session.TryConnectAndLogin(
                        "The Simpsons Hit And Run",
                        SLOTNAME,
                        ItemsHandlingFlags.AllItems,
                        new Version(MinArchipelagoVersion),
                        password: PASSWORD,
                        requestSlotData: true);
                }
                catch (Exception e)
                {
                    loginResult = new LoginFailure(e.GetBaseException().Message);
                }

                if (loginResult is LoginFailure loginFailure)
                {
                    _attemptingConnection = false;
                    Common.WriteLog("AP connection failed.", "ArchipelagoClient::TryConnect");
                    _session = null;

                    if (await HandleRetryDelayAsync())
                    {
                        URI = GetURI();
                        SLOTNAME = GetSlotName();
                        PASSWORD = GetPassword();
                    }
                }
            } while (loginResult is LoginFailure);

            var login = loginResult as LoginSuccessful;
            Common.WriteLog($"Successfully connected to {URI} as {SLOTNAME}", "ArchipelagoClient::TryConnect");
            Common.WriteLog("Slot Data:", "ArchipelagoClient::TryConnect");
            foreach (var kvp in login.SlotData)
            {
                Common.WriteLog($"  {kvp.Key}: {kvp.Value}", "ArchipelagoClient::TryConnect");
            }
            try
            {
                SaveName = $"{SLOTNAME}{login.Slot}-{login.SlotData["id"]}";
                victory = (VICTORY)int.Parse(login.SlotData["goal"].ToString());
                waspPercent = Convert.ToInt32(login.SlotData["EnableWaspPercent"]) == 1 ? Convert.ToInt32(login.SlotData["wasppercent"]) : 0;
                cardPercent = Convert.ToInt32(login.SlotData["EnableCardPercent"]) == 1 ? Convert.ToInt32(login.SlotData["cardpercent"]) : 0;
                MemoryManip.maxCoins = Convert.ToInt32(login.SlotData["maxprice"]);
                MemoryManip.coinScale = Convert.ToInt32(login.SlotData["shopscalemod"]);
                MemoryManip.gagfinder = Convert.ToBoolean(login.SlotData["shufflegagfinder"]);
                MemoryManip.checkeredflag = Convert.ToBoolean(login.SlotData["shufflecheckeredflags"]);
                MemoryManip.cardIDs = ((JArray)login.SlotData["card_locations"]).ToObject<List<long>>();
                JArray costsArray = (JArray)login.SlotData["costs"];
                ShopCosts = costsArray.ToObject<List<int>>();
                shp = (ShopHintPolicy)int.Parse(login.SlotData["shophintpolicy"].ToString());
                MemoryManip.VerifyID = (string)login.SlotData["VerifyID"];
            }
            catch (Exception ex)
            {
                Common.WriteLog($"{ex} This likely means this game was generated on an older .apworld.", "ArchipelagoClient::TryConnect");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            _session.DataStorage[Scope.Slot, "missions"].Initialize(0);
            _session.DataStorage[Scope.Slot, "bonus"].Initialize(0);
            _session.DataStorage[Scope.Slot, "wasps"].Initialize(0);
            _session.DataStorage[Scope.Slot, "cards"].Initialize(0);
            _session.DataStorage[Scope.Slot, "gags"].Initialize(0);
            _session.DataStorage[Scope.Slot, "shops"].Initialize(0);
            _session.DataStorage[Scope.Slot, "hnr"].Initialize(0);
            _session.DataStorage[Scope.Slot, "wrench"].Initialize(0);
            _session.DataStorage[Scope.Slot, "coins"].Initialize(0);
            _session.DataStorage[Scope.Slot, "localchecks"].Initialize(new[] {(long)1});

            MemoryManip.APCONNECTED = true;
            while (true)
            {
                await SendLocs();
            }
        }

        static async Task<bool> HandleRetryDelayAsync()
        {
            Common.WriteLog("Reattempting connection in 5 seconds. Press any key to re-enter connection info.", "ArchipelagoClient::TryConnect");

            int steps = 50;
            for (int i = 0; i < steps; i++)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true); 
                    return true; 
                }
                await Task.Delay(100);
            }

            return false; 
        }

        static string GetURI()
        {
            Common.WriteLog("Enter ip or port. If entry is just a port, then address will be assumed as archipelago.gg:", "Main");
            string uri = Console.ReadLine();
            if (int.TryParse(uri, out int porttest))
                uri = $"archipelago.gg:{uri}";
            return uri;
        }

        static string GetSlotName()
        {
            Common.WriteLog("Enter slot name:", "Main");
            return Console.ReadLine();
        }

        static string GetPassword()
        {
            Common.WriteLog("Enter password:", "Main");
            return Console.ReadLine();
        }


        public void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            _attemptingConnection = false;
            Task.Run(() => { _session.Socket.DisconnectAsync(); }).Wait();
            _session = null;
        }

        public void Session_SocketClosed(string reason)
        {
            Common.WriteLog("Connection to Archipelago lost: " + reason, "ArchipelagoClient::Session_SocketClosed");
            Disconnect();
        }

        public void Session_ErrorReceived(Exception e, string message)
        {
            Common.WriteLog(message, "ArchipelagoClient::Session_ErrorReceived");
            if (e != null)
            {
                Common.WriteLog(e, "ArchipelagoClient::Session_ErrorReceived");
            }

            Disconnect();
        }

        public void Session_OnMessageReceived(LogMessage message)
        {
            Common.WriteLog(message, "ArchipelagoClient::Session_OnMessageReceived");

            int screenWidthPixels = GetScreenWidthInPixels();

            int approxCharWidth = 25; //gained by guessing
            int maxCharsPerLine = screenWidthPixels / approxCharWidth;

            foreach (var line in WrapTextByWord(message.ToString(), maxCharsPerLine))
            {
                MemoryManip.APLog.Enqueue(line);
            }
        }

        private IEnumerable<string> WrapTextByWord(string text, int maxCharsPerLine)
        {
            var words = text.Split(' ');
            var line = new StringBuilder();

            foreach (var word in words)
            {
                if (line.Length + word.Length + 1 > maxCharsPerLine)
                {
                    yield return line.ToString();
                    line.Clear();
                }

                if (line.Length > 0)
                    line.Append(' ');

                line.Append(word);
            }

            if (line.Length > 0)
                yield return line.ToString();
        }


        async Task SendLocs()
        {
            long location = await sentLocations.DequeueAsync();
            SendLocation(location);
        }

        void SendLocation(long location)
        {
            if (!Connected)
            {
                Common.WriteLog($"Trying to send location {location} when there's no connection", "ArchipelagoClient::SendLocation");
                return;
            }
            Common.WriteLog(location, "ArchipelagoClient::SendLocation");
            _session.Locations.CompleteLocationChecksAsync(location);
            CheckVictory();
        }

        public bool IsLocationChecked(long location)
        {
            if (!Connected)
            {
                return false;
            }

            return _session.Locations.AllLocationsChecked.Contains(location);
        }

        
        public bool IsLocationCheckedLocally(long location)
        {
            if (!Connected)
            {
                throw new Exception("Not connected.");
            }

            var localChecks = _session.DataStorage[Scope.Slot, "localchecks"].To<List<long>>();

            bool ret = localChecks.Contains(location);
            if (!ret)
                _session.DataStorage[Scope.Slot, "localchecks"] += new[] { (long)location };

            return ret;
        }

        public bool SyncLocations(List<long> locations)
        {
            if (!Connected || locations == null || locations.Count == 0)
            {
                return false;
            }

            Common.WriteLog($"Sending location checks: {string.Join(", ", locations)}", "ArchipelagoClient::SyncLocations");
            _session.Locations.CompleteLocationChecksAsync(locations.ToArray());
            return true;
        }

        public void SendCompletion()
        {
            if (!Connected)
            {
                return;
            }

            _session.SetGoalAchieved();
        }

        public int GetCurrentPlayer()
        {
            if (!Connected)
            {
                return -1;
            }

            return _session.ConnectionInfo.Slot;
        }

        public void SendMessage(string message)
        {
            _session?.Say(message);
        }

        public async void Session_ItemReceived(IReceivedItemsHelper helper)
        {
            var index = helper.Index - 1;
            var item = helper.DequeueItem();
            var itemName = item.ItemName;
            itemName ??= item.ItemDisplayName;

            Common.WriteLog($"Received item #{index}: {item.ItemId} - {itemName}", "ArchipelagoClient::Session_ItemReceived");

            var player = item.Player;
            var playerName = player.Alias ?? player.Name ?? $"Player #{player.Slot}";

            _session.DataStorage[Scope.Slot, "index"].Initialize(0);

            var storedIndex = await _session.DataStorage[Scope.Slot, "index"].GetAsync<int>();
            if (storedIndex < index)
            {
                _session.DataStorage[Scope.Slot, "index"] = index;
                MemoryManip.itemsReceived.Enqueue(itemName);
            }
            else if (item.Flags == ItemFlags.Trap || NORESEND.Contains(item.ItemName))
            {
                Common.WriteLog($"Didn't enqueue {itemName} which is {item.Flags}", "ArchipelagoClient::Session_ItemReceived");
            }
            else
            {
                MemoryManip.itemsReceived.Enqueue(itemName);
            }
            
        }
        
        public async void SetShopNames(Dictionary<long, string> locations, FeLanguage language)
        {
            Common.WriteLog("Scouting", "ArchipelagoClient::ScoutShopLocationNoHint");
            await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, locations.Keys.ToArray()).ContinueWith(t => 
            {
                foreach (ItemInfo item in t.Result.Values)
                {
                    string ret = $"{item.Player}'s {item.ItemName}"; 
                    language.SetString(locations[item.LocationId].ToUpper(), ret);
                }
            });
        }

        public async void ScoutShopLocation(long[] locations)
        {
            if (shp == ShopHintPolicy.All)
                await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, locations);
            else if(shp == ShopHintPolicy.OnlyProg)
            {
                List<long> LocsToHint = new List<long>();
                await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, locations).ContinueWith(t =>
                {
                    foreach (ItemInfo item in t.Result.Values)
                    {
                        if (item.Flags.HasFlag(ItemFlags.Advancement))
                            LocsToHint.Add(item.LocationId);
                    }
                });

                await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, LocsToHint.ToArray());
            }
        }

        public async Task<T> GetDataStorage<T>(string type)
        {
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {
                    return await _session.DataStorage[Scope.Slot, type].GetAsync<T>();
                }
                catch (Exception ex)
                {
                    Common.WriteLog("GetDataStorage", $"Failed to get data for {type}. Retrying in 5 seconds");
                }
                await Task.Delay(5000);
            }
            // TODO: Change in-game strings
            Common.WriteLog("GetDataStorage", $"Failed to get data for {type}. Data is desynced, please restart.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
            return default(T);
        }

        public async Task SetDataStorage(string type, int amount)
        {
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {
                    _session.DataStorage[Scope.Slot, type] = amount;
                    return; 
                }
                catch (Exception ex)
                {
                    Common.WriteLog("SetDataStorage", $"Failed to set data for {type}. Retrying in 5 seconds");
                    await Task.Delay(5000); 
                }
            }
            // TODO: Change in-game strings
            Common.WriteLog("SetDataStorage", $"Failed to set data for {type}. Data is desynced, please restart.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public async Task IncrementDataStorage(string type)
        {
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {
                    int currentValue = await _session.DataStorage[Scope.Slot, type].GetAsync<int>();
                    _session.DataStorage[Scope.Slot, type] = currentValue + 1;
                    //Common.WriteLog($"Current Value for {type} is {currentValue}", "IncrementDataStorage");
                    return; 
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"Failed to increment data for {type}. Retrying in 5 seconds", "IncrementDataStorage");
                    attempts++;
                    await Task.Delay(5000);
                }
            }
            // TODO: Change in-game strings
            Common.WriteLog($"Failed to increment data for {type}. Data is desynced, please restart.", "IncrementDataStorage");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public async Task<List<long>> GetLocalChecks()
        {
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {       
                    return _session.DataStorage[Scope.Slot, "localchecks"].To<List<long>>();
                }
                catch (Exception ex)
                {
                    Common.WriteLog("IncrementDataStorage", $"Failed to get localchecks. Retrying in 5 seconds");
                    await Task.Delay(5000);
                }
            }
            // TODO: Change in-game strings
            Common.WriteLog("IncrementDataStorage", $"Failed to get localchecks. Data is desynced, please restart.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);

            return null;
        }

        public async void CheckVictory()
        {
            /*
            int missions = await _session.DataStorage[Scope.Slot, "missions"].GetAsync<int>();
            int bonus = await _session.DataStorage[Scope.Slot, "bonus"].GetAsync<int>();
            int wasps = await _session.DataStorage[Scope.Slot, "wasps"].GetAsync<int>();
            int cards = await _session.DataStorage[Scope.Slot, "cards"].GetAsync<int>();
            */
            var localChecks = await GetLocalChecks();

            int missions = 0;
            int bonus = 0;
            int wasps = 0;
            int cards = 0;
            int gags = 0;

            foreach (long id in localChecks)
            {
                string type, name;
                (type, name) = lt.getTypeAndNameByAPID(id);
                if (type == null && MemoryManip.cardIDs.Contains(id))
                {
                    type = "card";
                    name = $"card{id}";
                }

                if (name != null && !name.Contains("Talk to"))
                {
                    switch (type)
                    {
                        case "mission":
                            missions++;
                            break;
                        case "bonus missions":
                            bonus++;
                            break;
                        case "wasp":
                            wasps++;
                            break;
                        case "card":
                            cards++;
                            break;
                        case "gag":
                            gags++;
                            break;
                        default:
                            break;
                    }
                }
            }

            mm.UpdateProgress(missions, bonus, wasps, cards, gags, victory, waspPercent, cardPercent);

            Common.WriteLog($"Completed:\nMissions: {missions}\nBonus Missions: {bonus}\nWasps: {wasps}\nCards: {cards}", "ArchipelagoClient::CheckVictory");

            double wp = ((double)wasps / 140) * 100;
            Common.WriteLog($"Wasps: {wp}%", "ArchipelagoClient::CheckVictory");

            double cp = ((double)cards / 49) * 100;
            Common.WriteLog($"Cards: {cp}%", "ArchipelagoClient::CheckVictory");

            if (wp < waspPercent)
                return;
            if (cp < cardPercent)
                return;

            switch (victory)
            {
                case VICTORY.FinalMission:
                    if (IsLocationChecked(122361))
                        SendCompletion();
                    return;
                
                case VICTORY.AllStory:
                    if (missions >= 49)
                        SendCompletion();
                    return;

                case VICTORY.AllMissions:
                    if (missions >= 49 && bonus >= 28)
                        SendCompletion();
                    return;

                case VICTORY.WaspsCards:
                    SendCompletion();
                    return;
            }
        }
    }
}