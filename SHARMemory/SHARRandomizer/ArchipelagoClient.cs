using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using SHARRandomizer.Classes;

namespace SHARRandomizer
{
    public class ArchipelagoClient
    {
        private const string MinArchipelagoVersion = "0.5.0";
        public static AwaitableQueue<long> sentLocations = new AwaitableQueue<long>();

        public bool Connected => _session?.Socket.Connected ?? false;
        private bool _attemptingConnection;

        private ArchipelagoSession _session;
        private bool _ignoreLocations;

        private string URI = "archipelago.gg:49597";
        private string SLOTNAME = "CaesiusSHAR";
        private string PASSWORD = "";
/*
        private Dictionary<string, Dictionary<string, object>> location_table;
        private Dictionary<string, Dictionary<string, object>> item_table;
        private List<string> location_names;
        private List<string> item_names;
        private Dictionary<string, int> location_names_to_id;
        private Dictionary<string, int> item_names_to_id;
        private string game;

        public Dictionary<string, object> GetLocationByName(string name)
        {
            if (location_table.TryGetValue(name, out Dictionary<string, object> location))
            {
                return location;
            }
            else
            {
                return new Dictionary<string, object> { { "name", name } };
            }
        }

        public Dictionary<string, object> GetLocationById(int id)
        {
            string name = location_names[id];
            return GetLocationByName(name);
        }

        public Dictionary<string, object> GetItemByName(string name)
        {
            if (item_table.TryGetValue(name, out Dictionary<string, object> item))
            {
                return item;
            }
            else
            {
                return new Dictionary<string, object> { { "name", name } };
            }
        }
        public Dictionary<string, object> GetItemById(int id)
        {
            string name = item_names[id];
            return GetItemByName(name);
        }

        public void UpdateIds(Dictionary<string, object> dataPackage)
        {
            location_names_to_id = dataPackage["location_name_to_id"] as Dictionary<string, int>;
            item_names_to_id = dataPackage["item_name_to_id"] as Dictionary<string, int>;
        }

        public void UpdateDataPackage(Dictionary<string, object> dataPackage)
        {
            var games = dataPackage["games"] as Dictionary<string, object>;
            foreach (var kvp in games)
            {
                // If the key matches the current game, update the ids.
                if (kvp.Key == game)
                {
                    UpdateIds(kvp.Value as Dictionary<string, object>);
                }
            }
        }
*/
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
                Console.WriteLine(e);
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

            try
            {
                loginResult = _session.TryConnectAndLogin(
                    "SimpsonsHitAndRun",
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
                Console.WriteLine("AP connection failed: " + string.Join("\n", loginFailure.Errors));
                _session = null;
                return;
            }

            var login = loginResult as LoginSuccessful;
            Console.WriteLine($"Successfully connected to {URI} as {SLOTNAME}");
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
            Console.WriteLine("Connection to Archipelago lost: " + reason);
            Disconnect();
        }

        public void Session_ErrorReceived(Exception e, string message)
        {
            Console.WriteLine(message);
            if (e != null)
            {
                Console.WriteLine(e.ToString());
            }

            Disconnect();
        }

        public void Session_OnMessageReceived(LogMessage message)
        {
            Console.WriteLine(message);
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
                Console.WriteLine($"Trying to send location {location} when there's no connection");
                return;
            }

            _session.Locations.CompleteLocationChecksAsync(location);
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

            Console.WriteLine($"Sending location checks: {string.Join(", ", locations)}");
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

        public void Session_ItemReceived(IReceivedItemsHelper helper)
        {
            var index = helper.Index - 1;
            var item = helper.DequeueItem();
            var itemName = item.ItemName;
            itemName ??= item.ItemDisplayName;

            Console.WriteLine($"Received item #{index}: {item.ItemId} - {itemName}");
            var player = item.Player;
            var playerName = player.Alias ?? player.Name ?? $"Player #{player.Slot}";

            MemoryManip.itemsReceived.Enqueue(itemName);
        }
    }
}