using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using SHARMemory.SHAR.Classes;
using SHARRandomizer.Classes;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;


namespace SHARRandomizer
{
    public class ArchipelagoClient
    {
        public MemoryManip? mm;
        readonly LocationTranslations lt = LocationTranslations.LoadFromJson("Configs/Vanilla.json");
        //readonly RewardTranslations rt = RewardTranslations.LoadFromJson("Configs/Rewards.json");
        //readonly UITranslations uit = UITranslations.LoadFromJson("Configs/UITranslations.json");

        private Process _trackerProcess;
        private StreamWriter _trackerStdin;
        private readonly object _outputLock = new();
        private readonly List<string> _outputBuffer = new();
        private readonly TaskCompletionSource _trackerReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public event Action<List<string>> LocationsInLogicUpdated;
        private static readonly Random _rng = new();
        List<string> NORESEND = new List<string>() { "Wrench", "10 Coins", "Hit N Run Reset", "Hit N Run", "Reset Car", "Duff Trap", "Eject", "Launch", "Traffic Trap" };
        private const string MinArchipelagoVersion = "0.5.0"; //update to .6.0 soon
        public static AwaitableQueue<long> sentLocations = new AwaitableQueue<long>();

        public bool Connected => _session?.Socket.Connected ?? false;
        private bool _attemptingConnection;

        public event System.Action? ConnectionSucceeded;
        public event System.Action? ConnectionFailed;

        private CancellationTokenSource _cts = new();
        public CancellationToken Token => _cts.Token;

        private ArchipelagoSession? _session;

        public string URI = "";
        public string SLOTNAME = "";
        public string PASSWORD = "";
        public string appath = "";

        public static string? SaveName;
        public static List<int>? ShopCosts;

        public enum VICTORY
        {
            AllMissions = 0,
            AllStory = 1,
            FinalMission = 2,
            Cars = 3
        }

        public VICTORY victory = VICTORY.FinalMission;
        public int cardPercent = 0;
        public int waspPercent = 0;

        Queue<(long itemID, long locationID, int player)> ighints = new();

        public enum ShopHintPolicy
        {
            All = 0,
            OnlyProg = 1,
            None = 2
        }
        public ShopHintPolicy shp = ShopHintPolicy.All;
        public bool ehp = false;

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
            if (_session == null)
                throw new Exception($"Error setting up session. {nameof(_session)} was null.");

            _session.Socket.ErrorReceived += Session_ErrorReceived;
            _session.Socket.SocketClosed += Session_SocketClosed;
            _session.Items.ItemReceived += Session_ItemReceived;
            _session.MessageLog.OnMessageReceived += Session_OnMessageReceived;
        }

        private async void TryConnect()
        {
            while (!Token.IsCancellationRequested)
            {
                LoginResult loginResult;
                _attemptingConnection = true;

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
                                Common.WriteLog($"Error creating session: \"{e.Message}\".", "ArchipelagoClient::TryConnect");
                                loginResult = new LoginFailure("Session creation failed.");
                                _cts.Cancel();
                                ConnectionFailed?.Invoke();
                                return;
                            }
                        }

                        loginResult = _session.TryConnectAndLogin(
                            "Simpsons Hit and Run",
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
                        ConnectionFailed?.Invoke();
                        _cts.Cancel();
                        return;
                    }
                } while (loginResult is LoginFailure);

                var login = (LoginSuccessful)loginResult;
                Common.WriteLog($"Successfully connected to {URI} as {SLOTNAME}", "ArchipelagoClient::TryConnect");
                Common.WriteLog("Slot Data:", "ArchipelagoClient::TryConnect");
                foreach (var kvp in login.SlotData)
                {
                    Common.WriteLog($"  {kvp.Key}: {kvp.Value}", "ArchipelagoClient::TryConnect");
                }
                try
                {
                    //SaveName = $"{SLOTNAME}{login.Slot}-{login.SlotData["id"]}";
                    victory = (VICTORY)int.Parse(login.SlotData["Itchy_And_Scratchy_Ticket_Requirement"].ToString()!);
                    waspPercent = Convert.ToInt32(login.SlotData["Wasp_Percent"]);
                    waspPercent = Convert.ToInt32(login.SlotData["Card_Percent"]);
                    MemoryManip.maxCoins = Convert.ToInt32(login.SlotData["Max_Shop_Price"]);
                    MemoryManip.coinScale = Convert.ToInt32(login.SlotData["Shop_Scale_Modifier"]);
                    MemoryManip.gagfinder = (login.SlotData["Shuffle_Gagfinder"] as JArray)?.Count > 0; ;
                    MemoryManip.checkeredflag = (login.SlotData["Shuffle_Checkered_Flags"] as JArray)?.Count > 0; ;
                    MemoryManip.cardIDs = ((JArray)login.SlotData["card_locations"]).ToObject<List<long>>()!;
                    JArray costsArray = (JArray)login.SlotData["costs"];
                    ShopCosts = costsArray.ToObject<List<int>>()!;
                    shp = (ShopHintPolicy)Convert.ToInt32(login.SlotData["Shop_Hint_Policy"].ToString()!);
                    ehp = Convert.ToBoolean(login.SlotData["Extra_Hint_Policy"]);
                    MemoryManip.VerifyID = (string)login.SlotData["VerifyID"];
                    var ingameHints = login.SlotData["ingamehints"];

                    if (ingameHints is JValue jv && jv.Value is string s && s == "No hints")
                    {
                        Common.WriteLog("No ingame hints provided.", "ArchipelagoClient::TryConnect");
                    }
                    else if (ingameHints is JObject obj)
                    {
                        foreach (var kv in (JObject)login.SlotData["ingamehints"])
                        {
                            long itemID = long.Parse(kv.Key);
                            var loc = (JArray)kv.Value;

                            long locID = loc[0].Value<long>();
                            int player = loc[1].Value<int>();

                            ighints.Enqueue((itemID, locID, player));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.WriteLog(ex.ToString(), "test");
                    ex.ToString();
                }

                _session!.DataStorage[Scope.Slot, "missions"].Initialize(0);
                _session.DataStorage[Scope.Slot, "bonus"].Initialize(0);
                _session.DataStorage[Scope.Slot, "wasps"].Initialize(0);
                _session.DataStorage[Scope.Slot, "cards"].Initialize(0);
                _session.DataStorage[Scope.Slot, "gags"].Initialize(0);
                _session.DataStorage[Scope.Slot, "shops"].Initialize(0);
                _session.DataStorage[Scope.Slot, "hnr"].Initialize(0);
                _session.DataStorage[Scope.Slot, "wrench"].Initialize(0);
                _session.DataStorage[Scope.Slot, "coins"].Initialize(0);
                _session.DataStorage[Scope.Slot, "localchecks"].Initialize(new[] { (long)1 });

                MemoryManip.APCONNECTED = true;
                ConnectionSucceeded?.Invoke();
                while (true)
                {
                    await SendLocs();
                }
            }
        }

        public async void Disconnect()
        {
            if (!Connected)
            {
                return;
            }

            _attemptingConnection = false;
            await _session?.Socket.DisconnectAsync();
            _session = null;
            _cts.Cancel();
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
            /*
            if (location == 122361)
                await SetDataStorage("finalmission", 1);
            */
            await SendLocation(location);
        }

        async Task SendLocation(long location)
        {
            if (!Connected)
            {
                Common.WriteLog($"Trying to send location {location} when there's no connection", "ArchipelagoClient::SendLocation");
                return;
            }
            if (_session == null)
            {
                Common.WriteLog($"Trying to send location {location} when session is null", "ArchipelagoClient::SendLocation");
                return;
            }
            Common.WriteLog(location, "ArchipelagoClient::SendLocation");
            await _session.Locations.CompleteLocationChecksAsync(location);
        }

        public List<long> GetCheckedLocations()
        {
            if (!Connected)
                return new List<long>();
            if (_session == null)
                return new List<long>();
            
            return _session.Locations.AllLocationsChecked.ToList<long>();

        }

        public bool IsLocationChecked(long location)
        {
            if (!Connected)
            {
                return false;
            }
            if (_session == null)
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
            if (_session == null)
            {
                throw new Exception("Session null.");
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
            if (_session == null)
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
            if (_session == null)
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
            if (_session == null)
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
            if (_session == null)
            {
                throw new Exception("Session null.");
            }

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
            else if (NORESEND.Contains(item.ItemName))
            {
                Common.WriteLog($"Didn't enqueue {itemName} which is {item.Flags}", "ArchipelagoClient::Session_ItemReceived");
            }
            else
            {
                MemoryManip.itemsReceived.Enqueue(itemName);
            }
        }

        public async void SetShopNames(Dictionary<long, string> locations, FeLanguage? language)
        {
            Common.WriteLog("Scouting", "ArchipelagoClient::ScoutShopLocationNoHint");
            if (_session == null)
            {
                throw new Exception("Session null.");
            }
            var results = await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, [.. locations.Keys]);
            foreach (var item in results.Values)
            {
                string ret = $"{item.Player}'s {item.ItemName}";
                language?.SetString(locations[item.LocationId].ToUpper(), ret);
            }
        }

        public async void ScoutShopLocation(long[] locations)
        {
            if (_session == null)
            {
                throw new Exception("Session null.");
            }
            if (shp == ShopHintPolicy.All)
                await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, locations);
            else if (shp == ShopHintPolicy.OnlyProg)
            {
                var results = await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, locations);
                var values = results.Values;
                List<long> LocsToHint = new(values.Count);
                foreach (var item in values)
                    if (item.Flags.HasFlag(ItemFlags.Advancement))
                        LocsToHint.Add(item.LocationId);

                await _session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, LocsToHint.ToArray());
            }
            else if (shp == ShopHintPolicy.None) { /* pass */ }
        }
        public string ExtraHint()
        {
            if (ighints.Count == 0)
                return GetJoke();

            var receivedItemIds = _session?.Items.AllItemsReceived.Select(i => i.ItemId).ToHashSet();

            while (ighints.Count > 0)
            {
                var (itemID, locID, player) = ighints.Dequeue();

                if (!receivedItemIds.Contains(itemID))
                    continue;

                _session?.Hints.CreateHints(player, HintStatus.Unspecified, locID);

                var p = _session.Players.GetPlayerInfo(player);
                var location = _session.Locations.GetLocationNameFromId(locID, p.Game);
                var item = _session.Items.GetItemName(itemID);

                return $"Your {item} is at {location} in {p}'s world.";
            }

            return GetJoke();
        }

        private string GetJoke() =>
            _jokes[_rng.Next(_jokes.Count)];

        /* Exception on declaring things randomly in the middle cuz its only relevant here */
        private static readonly List<string> _jokes =
        [
            "Milhouse says he saw your hint behind the Kwik-E-Mart, but Milhouse says a lot of things.",
            "Your hint is in the Aurora Borealis. Localized entirely within your kitchen. At this time of year.",
            "Apu put your hint back on the hot dog roller even though it fell on the floor. He says it's still good.",
            "Worst. Hint. Ever.",
            "Now lemme tell ya, your hint was over on the banana tree... OH that reminds me of the time I was court-martialed...",
            "Now listen here, your hint.... well, that takes me back to the time I went huntin' for clues...",
            "Uh.. your hint? Yeaaahh, about that... I had it right here next to my donut...",
            "Dude, your hint totally bailed...",
            "The hint machine has been unplugged to prevent Lenny and Carl from fighting...",
            "You want the new hint from the B-Sharps? Sorry they broke up. Again."
        ];

        public async Task<T> GetDataStorage<T>(string type)
        {
            if (_session == null)
            {
                Common.WriteLog($"Failed to get data for {type}. Session is null, please restart.", "GetDataStorage");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
                return default(T);
            }
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {
                    return await _session.DataStorage[Scope.Slot, type].GetAsync<T>();
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"Failed to get data for {type}: \"{ex.Message}\". Retrying in 5 seconds", "GetDataStorage");
                }
                await Task.Delay(5000);
            }
            // TODO: Change in-game strings
            Common.WriteLog($"Failed to get data for {type}. Data is desynced, please restart.", "GetDataStorage");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
            return default(T);
        }

        public async Task SetDataStorage(string type, int amount)
        {
            if (_session == null)
            {
                Common.WriteLog($"Failed to set data for {type}. Session is null, please restart.", "SetDataStorage");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
                return;
            }
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
                    Common.WriteLog($"Failed to set data for {type}: \"{ex.Message}\". Retrying in 5 seconds", "SetDataStorage");
                    await Task.Delay(5000);
                }
            }
            Common.WriteLog($"Failed to set data for {type}. Data is desynced, please restart.", "SetDataStorage");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public async Task IncrementDataStorage(string type)
        {
            if (_session == null)
            {
                Common.WriteLog($"Failed to increment data for {type}. Session is null, please restart.", "IncrementDataStorage");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
                return;
            }
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
                    Common.WriteLog($"Failed to increment data for {type}: \"{ex.Message}\". Retrying in 5 seconds", "IncrementDataStorage");
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
            if (_session == null)
            {
                Common.WriteLog($"Failed to get localchecks. Session is null, please restart.", "GetLocalChecks");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
                return null;
            }
            var attempts = 0;
            while (attempts++ < 5)
            {
                try
                {
                    return _session.DataStorage[Scope.Slot, "localchecks"].To<List<long>>();
                }
                catch (Exception ex)
                {
                    Common.WriteLog($"Failed to get localchecks: \"{ex.Message}\". Retrying in 5 seconds", "GetLocalChecks");
                    await Task.Delay(5000);
                }
            }
            // TODO: Change in-game strings
            Common.WriteLog($"Failed to get localchecks. Data is desynced, please restart.", "GetLocalChecks");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);

            return null;
        }



        /* Old victory check, will remove eventually
        public async Task CheckVictory()
        {            
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
                if (type == null && (MemoryManip.cardIDs != null && MemoryManip.cardIDs.Contains(id)))
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

            mm?.UpdateProgress(missions, bonus, wasps, cards, gags, victory, waspPercent, cardPercent);

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
                    int fmcheck = await GetDataStorage<int>("finalmission");
                    if (fmcheck == 1)
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
        */
    }
}