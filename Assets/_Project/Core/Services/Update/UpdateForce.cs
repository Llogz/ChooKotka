using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Services
{
    public class UpdateForce
    {
        public float TimeScale { get; set; } = 1f;
        public bool Running { get; set; } = true;

        private readonly List<IUpdatable> _updatables = new();

        public void Register(IUpdatable u)
        { 
            if (!_updatables.Contains(u))
                _updatables.Add(u);
        }

        public void Unregister(IUpdatable u)
        {
            while (_updatables.Remove(u))
            {
            }
        }

        public void StartUpdateLoop(PlayerLoopTiming timing, ILifetime lifetime)
        {
            UpdateLoop(timing, lifetime.GetToken()).AttachExternalCancellation(lifetime.GetToken()).Forget();
        }

        public void StartPhysicsLoop(PlayerLoopTiming timing, ILifetime lifetime)
        {
            PhysicsUpdateLoop(timing, lifetime.GetToken()).Forget();
        }

        private async UniTask PhysicsUpdateLoop(PlayerLoopTiming timing, CancellationToken token)
        {
            Physics.simulationMode = SimulationMode.Script;
            Physics2D.simulationMode = SimulationMode2D.Script;
            
            while (Running)
            {
                await UniTask.Yield(timing, token);

                if (token.IsCancellationRequested)
                    break;

                float dt = Time.deltaTime * TimeScale;
                
                Physics.Simulate(dt);
                Physics2D.Simulate(dt);
            }
        }

        private async UniTask UpdateLoop(PlayerLoopTiming timing, CancellationToken token)
        {
            while (Running)
            {
                await UniTask.Yield(timing, token);

                if (token.IsCancellationRequested)
                    break;

                float dt = Time.deltaTime * TimeScale;

                for (int i = 0; i < _updatables.Count; i++)
                {
                    var updatable = _updatables[i];
                    var behaviour = updatable as Object;
                    if (behaviour == null || updatable.IsDestroyed)
                    {
                        _updatables.RemoveAt(i);
                        i--;
                        continue;
                    }
                    
                    switch (timing)
                    {
                        case PlayerLoopTiming.EarlyUpdate:
                            updatable.GameEarlyUpdate(dt);
                            break;
                        case PlayerLoopTiming.FixedUpdate:
                            updatable.GameFixedUpdate(dt);
                            break;
                        case PlayerLoopTiming.Update:
                            updatable.GameUpdate(dt);
                            break;
                        case PlayerLoopTiming.PreLateUpdate:
                            updatable.GameLateUpdate(dt);
                            break;
                    }
                }
            }
        }
    }

}
