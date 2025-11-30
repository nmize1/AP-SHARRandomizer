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
        public List<ITrap> Traps { get; } = new();

        public TrapHandler(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
        {
            Traps.Add(new LaunchTrap(memory, memoryManip, watcher));
            Traps.Add(new HitNRunTrap(memory, memoryManip, watcher));
            Traps.Add(new Eject(memory, memoryManip, watcher));

            Traps.Add(new DuffTrap(memory, memoryManip, watcher));
            Traps.Add(new TrafficTrap(memory, memoryManip, watcher));
        }

        public void NotifyControllerRemap()
        {
            foreach (var trap in Traps)
            {
                foreach (var c in trap.Components)
                {
                    if (c is IControlRemappedComponent remap)
                    {
                        remap.OnControllerRemap();
                    }
                }
            }
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
        IEnumerable<object> Components => Enumerable.Empty<object>();
    }

    public interface ITimerComponent
    {
        void AddTime(TimeSpan amount);
        TimeSpan? TimeRemaining { get; }
    }

    public class TimerComponent : ITimerComponent
    {
        private Func<Task> _onTrapStartAsync;
        private Func<Task> _onTrapStopAsync;

        private DateTime _expireAt;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _lock = new();

        public TimerComponent(Func<Task> onTrapStartAsync, Func<Task> onTrapStopAsync)
        {
            _onTrapStartAsync = onTrapStartAsync;
            _onTrapStopAsync = onTrapStopAsync;
        }

        public TimeSpan? TimeRemaining => _cancellationTokenSource == null ? null : _expireAt - DateTime.UtcNow;

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
            await _onTrapStartAsync();

            while (DateTime.UtcNow < _expireAt)
            {
                var wait = _expireAt - DateTime.UtcNow;
                if (wait > TimeSpan.FromSeconds(1))
                    wait = TimeSpan.FromSeconds(1);

                await Task.Delay(wait, token);
            }

            await _onTrapStopAsync();

            lock (_lock)
            {
                _cancellationTokenSource = null;
            }
        }
    }

    public interface IControlRemappedComponent
    {
        void OnControllerRemap();
    }

    public class ControlRemappedComponent : IControlRemappedComponent
    {
        private readonly System.Action _onRemap;

        public ControlRemappedComponent(System.Action onRemap)
        {
            _onRemap = onRemap;
        }

        public void OnControllerRemap() => _onRemap();
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

        public LaunchTrap(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
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

        public HitNRunTrap(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
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

        public Eject(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
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
        private readonly TimerComponent _timer;

        private List<object> _components = new();
        public IEnumerable<object> Components => _components;

        public TrafficTrap(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
        {
            _memory = memory;

            _timer = new TimerComponent(onTrapStartAsync, onTrapStopAsync);
            _components.Add(_timer);

            watcher.TrapTriggered += OnTrapTriggered;
        }

        private void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            _timer.AddTime(TimeSpan.FromSeconds(60));
        }

        private async Task onTrapStartAsync()
        { 
            var hnr = _memory.Singletons.HitNRunManager;
            int vdc = hnr.VehicleDestroyedCoins;
            int trafficgroup = 1;

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
        }

        private Task onTrapStopAsync()
        {
            var hnr = _memory.Singletons.HitNRunManager;

            _memory.Globals.TrafficManager.CurrTrafficModelGroup = 0;
            _memory.Globals.TrafficAIMinSecondsBetweenLaneChanges = 5;

            return Task.CompletedTask;
        }
    }

    public class DuffTrap : ITrap
    {
        public string Name => "Duff Trap";

        private Memory _memory;
        private MemoryManip _memoryManip;

        private TimerComponent _timer;
        private ControlRemappedComponent _remap;

        private List<object> _components = new();
        public IEnumerable<object> Components => _components;

        public DuffTrap(Memory memory, MemoryManip memoryManip, TrapWatcher watcher)
        {
            _memory = memory;
            _memoryManip = memoryManip;
            _timer = new TimerComponent(OnTrapStartAsync, OnTrapStopAsync);
            _remap = new ControlRemappedComponent(OnRemap);
            _components.Add(_remap);
            _components.Add(_timer);
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (e.TrapName != Name)
                return;

            _timer.AddTime(TimeSpan.FromSeconds(30));
        }

        private Task OnTrapStartAsync()
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

            _memoryManip.CheckAvailableMoves(_memory, _memoryManip.CURRENTLEVEL);

            return Task.CompletedTask;
        }

        private Task OnTrapStopAsync()
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

        private void OnRemap()
        {
            if (_timer.TimeRemaining != null)
            {
                /* Reset the buttons and then swap them again to undo remapping shenanigans */
                var controller = _memory.Singletons.InputManager.ControllerArray[0];
                
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

                //car
                controller.SwapButtons(InputManager.Buttons.SteerLeft, InputManager.Buttons.SteerRight);
                controller.SwapButtons(InputManager.Buttons.Accelerate, InputManager.Buttons.Reverse);

                //walk
                controller.SwapButtons(InputManager.Buttons.MoveLeft, InputManager.Buttons.MoveRight);
                controller.SwapButtons(InputManager.Buttons.MoveUp, InputManager.Buttons.MoveDown);
                controller.SwapButtons(InputManager.Buttons.Attack, InputManager.Buttons.Jump);
            }
        }
    }
}
