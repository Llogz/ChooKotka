using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Services
{
    public interface ITimerService
    {
        UniTask Delay(float time, Action callback, UpdateType updateType, ILifetime lifetime);
        void DelayFireAndForget(float time, Action callback, UpdateType updateType, ILifetime lifetime);
    }
    
    public class TimerService : SceneService, ITimerService
    {
        [Inject] private IUpdateManagerService _updateManager;
        
        private readonly List<Timer> _timers = new();
        
        public override void Configure(VContainer.IContainerBuilder builder)
        {
            builder.RegisterComponent(this).As<ITimerService>();
        }

        public UniTask Delay(float time, Action callback, UpdateType updateType, ILifetime lifetime)
        {
            var tcs = new UniTaskCompletionSource();
            var timer = new Timer(time, callback, tcs, updateType, lifetime.GetToken());
            _timers.Add(timer);
            return tcs.Task;
        }

        public void DelayFireAndForget(float time, Action callback, UpdateType updateType, ILifetime lifetime)
        {
            var timer = new Timer(time, callback, null, updateType, lifetime.GetToken());
            _timers.Add(timer);
        }

        public void UpdateTimers(float dt, UpdateType type)
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                if (_timers[i].UpdateType != type)
                    continue;

                if (_timers[i].Tick(dt))
                    _timers.RemoveAt(i);
            }
        }

        private void Awake()
        {
            foreach (UpdateType type in Enum.GetValues(typeof(UpdateType)))
            {
                var updater = gameObject.AddComponent<TimerUpdater>();
                updater.Setup(this, type);
                updater.Init(_updateManager);
            }
        }
    }
}
