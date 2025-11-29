using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace SHARRandomizer.Classes
{
    public class TrapHandler
    {
        public TrapHandler(Memory memory, TrapWatcher watcher)
        {
            _ = new LaunchTrap(memory, watcher);
            _ = new HitNRunTrap(memory, watcher);
            _ = new Eject(memory, watcher);

            _ = new DuffTrap(memory, watcher);
            _ = new TrafficTrap(memory, watcher);
        }
    }

    public class TrapEventArgs : EventArgs
    {
        public string TrapName { get; }

        public TrapEventArgs(string trapName)
        {
            TrapName = trapName;
        }
    }

    public interface ITrap
    {
        string Name { get; }
    }

    public abstract class TimeTrapBase : ITrap
    {
        public abstract string Name { get; }

        private DateTime _expireAt;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _lock = new();

        protected abstract Task OnTrapStartAsync();
        protected abstract Task OnTrapStopAsync();

        public void AddTime(TimeSpan amount)
        {
            var now = DateTime.UtcNow;

            lock (_lock)
            {
                _expireAt = (_cancellationTokenSource == null) ? now + amount : _expireAt + amount;

                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _ = RunAsync(_cancellationTokenSource.Token);
                }
            }
        }

        private async Task RunAsync(CancellationToken token)
        {
            await OnTrapStartAsync();

            while (DateTime.UtcNow < _expireAt)
            {
                var wait = _expireAt - DateTime.UtcNow;
                if(wait > TimeSpan.FromSeconds(1))
                    wait = TimeSpan.FromSeconds(1);

                await Task.Delay(wait, token);
            }

            await OnTrapStopAsync();

            lock (_lock)
            {
                _cancellationTokenSource = null;
            }
        }
    }

    public class TrapWatcher
    {
        public event EventHandler<TrapEventArgs>? TrapTriggered;

        public void OnTrapDetected(string trapName)
        {
            TrapTriggered?.Invoke(this, new TrapEventArgs(trapName));
        }
    }

    public class LaunchTrap : ITrap
    {
        public string Name => "Launch";

        private Memory _memory;

        public LaunchTrap(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private async void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            var car = _memory.Singletons.CharacterManager?.Player?.Car;

            while ((car = _memory.Globals.GameplayManager?.CurrentVehicle) == null)
                await Task.Delay(100);

            car.Launch(Random.Shared.Next(20, 101), Random.Shared.Next(-20, 51));
        }
    }

    public class HitNRunTrap : ITrap
    {
        public string Name => "Hit N Run";

        private Memory _memory;

        public HitNRunTrap(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            _memory.Singletons.HitNRunManager.CurrHitAndRun = 100f;
        }
    }

    public class Eject : ITrap
    {
        public string Name => "Eject";
        
        private Memory _memory;

        public Eject(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private async void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            Vehicle car;

            while ((car = _memory.Globals.GameplayManager?.CurrentVehicle) == null)
                await Task.Delay(100);

            while (car != null)
            {
                car.Stop();
                if (_memory.Singletons.CharacterManager?.Player?.Controller is CharacterController charcontroller)
                    charcontroller.Intention = CharacterController.Intentions.GetOutCar;
                await Task.Delay(100);
                car = _memory.Singletons.CharacterManager?.Player?.Car;
            }
        }
    }

    public class TrafficTrap : ITrap
    {
        public string Name => "Traffic Trap";

        private Memory _memory;

        public TrafficTrap(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private async void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            var hnr = _memory.Singletons.HitNRunManager;
            int vdc = hnr.VehicleDestroyedCoins;
            int trafficgroup = 1;


            for (int i = 0; i < 2; i++)
            {
                /* Disable coin drops, destroy all traffic for dramatic effect, reenable coin drops */
                hnr.VehicleDestroyedCoins = trafficgroup == 1 ? 0 : vdc;
                float curhnr = _memory.Singletons.HitNRunManager.CurrHitAndRun;
                foreach (TrafficVehicle v in _memory.Globals.TrafficManager.Vehicles.ToArray())
                {
                    if (v == null) continue;
                    try
                    {
                        v.Vehicle.VehicleDestroyed = true;
                    }
                    catch
                    {
                        Common.WriteLog($"Attempted to destroy null traffic.", "HandleTraps");
                    }
                    _memory.Singletons.HitNRunManager.CurrHitAndRun = 0.0f;
                    await Task.Delay(50);
                }
                _memory.Singletons.HitNRunManager.CurrHitAndRun = curhnr;
                hnr.VehicleDestroyedCoins = vdc;

                /* Switch traffic cars */
                _memory.Globals.TrafficManager.CurrTrafficModelGroup = trafficgroup;

                /* time to swerve */
                _memory.Globals.TrafficAIMinSecondsBetweenLaneChanges = trafficgroup == 1 ? 0 : 5;

                /* if we went back to default, break out */
                if (trafficgroup == 0)
                {
                    break;
                }

                /* sit back and relax */
                var end = DateTime.UtcNow.AddSeconds(60);

                while (DateTime.UtcNow < end)
                {
                    await Task.Delay(100);
                }

                /* set traffic back to original group and repeat the switch */
                trafficgroup = 0;
            }
        }
    }

    public class DuffTrap : TimeTrapBase
    {
        public override string Name => "Duff Trap";

        private Memory _memory;

        public DuffTrap(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            AddTime(TimeSpan.FromSeconds(30));
        }

        protected override Task OnTrapStartAsync()
        {
            var controller = _memory.Singletons.InputManager.ControllerArray[0];

            Common.WriteLog("Drunk", "DuffTrap");
            //car
            controller.SwapButtons(InputManager.Buttons.SteerLeft, InputManager.Buttons.SteerRight);
            controller.SwapButtons(InputManager.Buttons.Accelerate, InputManager.Buttons.Reverse);

            //walk
            controller.SwapButtons(InputManager.Buttons.MoveLeft, InputManager.Buttons.MoveRight);
            controller.SwapButtons(InputManager.Buttons.MoveUp, InputManager.Buttons.MoveDown);
            controller.SwapButtons(InputManager.Buttons.Attack, InputManager.Buttons.Jump);

            return Task.CompletedTask;
        }

        protected override Task OnTrapStopAsync()
        {
            var controller = _memory.Singletons.InputManager.ControllerArray[0];

            Common.WriteLog("Sober", "DuffTrap");
            //car
            controller.EnableButton(InputManager.Buttons.SteerLeft);
            controller.EnableButton(InputManager.Buttons.SteerRight);
            controller.EnableButton(InputManager.Buttons.Accelerate);
            controller.EnableButton(InputManager.Buttons.Reverse);

            //walk
            controller.EnableButton(InputManager.Buttons.MoveLeft);
            controller.EnableButton(InputManager.Buttons.MoveRight);
            controller.EnableButton(InputManager.Buttons.MoveUp);
            controller.EnableButton(InputManager.Buttons.MoveDown);
            controller.EnableButton(InputManager.Buttons.Attack);
            controller.EnableButton(InputManager.Buttons.Jump);

            return Task.CompletedTask;
        }
    }
}
