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
                            Console.WriteLine(e);
                        }
                    }

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
                    Console.WriteLine("Reattempting connecting in 5 seconds.");
                    await Task.Delay(5000);
                }
            } while (loginResult is LoginFailure);

            var login = loginResult as LoginSuccessful;
            Console.WriteLine($"Successfully connected to {URI} as {SLOTNAME}");
            Console.WriteLine("Slot Data:");
            foreach (var kvp in login.SlotData)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
            MemoryManip.UUID = login.SlotData["id"].ToString();
            _session.DataStorage["index"].Initialize(0);
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
            Console.WriteLine(location);
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

        public async void Session_ItemReceived(IReceivedItemsHelper helper)
        {
            var index = helper.Index - 1;
            var item = helper.DequeueItem();
            var itemName = item.ItemName;
            itemName ??= item.ItemDisplayName;

            Console.WriteLine($"Received item #{index}: {item.ItemId} - {itemName}");
            
            var player = item.Player;
            var playerName = player.Alias ?? player.Name ?? $"Player #{player.Slot}";

            var storedIndex = await _session.DataStorage["index"].GetAsync<int>();
            if (storedIndex < index)
            {
                _session.DataStorage["index"] = index;
                MemoryManip.itemsReceived.Enqueue(itemName);
            }
            else if (item.Flags != ItemFlags.Trap && !NORESEND.Contains(item.ItemName))
            {
                MemoryManip.itemsReceived.Enqueue(itemName);
            }
            else
            {
                Console.WriteLine($"Didn't enqueue {itemName} which is {item.Flags}");
            }
            
        }
    }
}