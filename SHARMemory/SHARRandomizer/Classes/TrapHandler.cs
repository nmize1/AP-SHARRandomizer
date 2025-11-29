using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SHARMemory.Memory;
using SHARMemory.SHAR;
using SHARMemory.SHAR.Classes;

namespace SHARRandomizer.Classes
{
    public class TrapHandler
    {
        public TrapHandler(Memory memory, TrapWatcher watcher)
        {
            _ = new LaunchTrap(memory, watcher);
            _ = new DuffTrap(memory, watcher);
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

        void AddTime(TimeSpan time);
    }

    public abstract class TimeTrapBase : ITrap
    {
        public abstract string Name { get; }

        private DateTime _expireAt;
        private CancellationTokenSource _cancellationTokenSource;
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

                await Task.Delay(wait);
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
        public void AddTime(TimeSpan t) { /*pass*/ }

        private Memory _memory;

        public LaunchTrap(Memory memory, TrapWatcher watcher)
        {
            _memory = memory;
            watcher.TrapTriggered += OnTrapTriggered;
        }

        private async void OnTrapTriggered(object? sender, TrapEventArgs e)
        {
            if (!(e.TrapName == Name))
                return;

            var car = _memory.Singletons.CharacterManager?.Player?.Car;

            while ((car = _memory.Globals.GameplayManager?.CurrentVehicle) == null)
                await Task.Delay(100);

            car.Launch(Random.Shared.Next(20, 101), Random.Shared.Next(-20, 51));
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
            if (e.TrapName == Name)
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
