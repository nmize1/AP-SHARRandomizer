using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using SHARRandomizer.Classes;
using SHARMemory.SHAR.Classes;
using Newtonsoft.Json.Linq;


namespace SHARRandomizer
{
    public class ArchipelagoClient
    {
        List<string> NORESEND = new List<string>() { "Wrench", "10 Coins", "Hit N Run Reset"};
        private const string MinArchipelagoVersion = "0.5.0";
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
                            Common.WriteLog($"Error creating session: {e}", "ArchipelagoClient::TryConnect");
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
                    Common.WriteLog("AP connection failed: " + string.Join("\n", loginFailure.Errors), "ArchipelagoClient::TryConnect");
                    _session = null;
                    Common.WriteLog("Reattempting connecting in 5 seconds.", "ArchipelagoClient::TryConnect");
                    await Task.Delay(5000);
                }
            } while (loginResult is LoginFailure);

            var login = loginResult as LoginSuccessful;
            Common.WriteLog($"Successfully connected to {URI} as {SLOTNAME}", "ArchipelagoClient::TryConnect");
            Common.WriteLog("Slot Data:", "ArchipelagoClient::TryConnect");
            foreach (var kvp in login.SlotData)
            {
                Common.WriteLog($"  {kvp.Key}: {kvp.Value}", "ArchipelagoClient::TryConnect");
            }
            SaveName = $"{SLOTNAME}{login.Slot}-{login.SlotData["id"]}";
            victory = (VICTORY)int.Parse(login.SlotData["goal"].ToString());
            waspPercent = Convert.ToInt32(login.SlotData["EnableWaspPercent"]) == 1 ? Convert.ToInt32(login.SlotData["wasppercent"]) : 0;
            cardPercent = Convert.ToInt32(login.SlotData["EnableCardPercent"]) == 1 ? Convert.ToInt32(login.SlotData["cardpercent"]) : 0;

            JArray costsArray = (JArray)login.SlotData["costs"];
            ShopCosts = costsArray.ToObject<List<int>>();

            MemoryManip.APCONNECTED = true;
            while (true)
            {
                await SendLocs();
            }
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
        }

        async Task SendLocs()
        {
            long location = await sentLocations.DequeueAsync();
            SendLocation(location);
        }

        public void SendLocation(long location)
        {
            if (!Connected)
            {
                Common.WriteLog($"Trying to send location {location} when there's no connection", "ArchipelagoClient::SendLocation");
                return;
            }
            Common.WriteLog(location, "ArchipelagoClient::SendLocation");
            _session.Locations.CompleteLocationChecksAsync(location);
            CheckVictory(location);
        }

        public bool IsLocationChecked(long location)
        {
            if (!Connected)
            {
                return false;
            }

            return _session.Locations.AllLocationsChecked.Contains(location);
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
        
        public async void ScoutShopLocationNoHint(Dictionary<long, string> locations, FeLanguage language)
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

        public void ScoutShopLocation(long[] locations)
        {
            _session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, locations);
        }

        /* Maybe move summation loop to on connect then just do 1 ++ each check. More efficient. Also, maybe check game stats instead of locations? */
        void CheckVictory(long location)
        {
            int missions = 0;
            int bonus = 0;
            int wasps = 0;
            int cards = 0;

            foreach (var loc in _session.Locations.AllLocationsChecked)
            { 
                (string type, _) = lt.getTypeAndNameByAPID(loc);

                switch (type)
                {
                    case "mission":
                        missions++;
                        break;
                    case "bonus_mission":
                        bonus++;
                        break;
                    case "wasp":
                        wasps++;
                        break;
                    case "card":
                        cards++;
                        break;
                }
            }

            double wp = ((double)wasps / 140) * 100;
            Common.WriteLog($"Wasps: {wp}", "ArchipelagoClient::CheckVictory");
            
            double cp = ((double)cards / 49) * 100;
            Common.WriteLog($"Cards: {cp}", "ArchipelagoClient::CheckVictory");

            if (wp < waspPercent)
                return;
            if (cp < cardPercent)
                return;
            
            //Common.WriteLog($"GOAL: {victory.ToString()}. WASPS: {wp} / {waspPercent} ({wasps}). CARDS: {cp} / {cardPercent} ({cards}). MISSIONS: {missions} / 49. BONUS MISSIONS: {bonus} / 28.");
            if (victory == VICTORY.FinalMission)
            {
                if (IsLocationChecked(122361))
                {
                    SendCompletion();
                    return;
                }
            }
            else
            {
                switch (victory)
                {
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
}