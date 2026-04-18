using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.Services
{
    public interface IUpdatable
    {
        void GameUpdate(float dt);
        void GameFixedUpdate(float dt);
        void GameEarlyUpdate(float dt);
        void GameLateUpdate(float dt);
        /// <summary>
        /// WARNING!!! If object is being instantiated not from 
        /// Factory UpdateType should be initialized manually
        /// </summary>
        UpdateType UpdateType { get; }
        bool IsDestroyed { get; }
    }
    
    public abstract class UpdatableBehaviour : MonoBehaviour, IUpdatable
    { 
        public abstract UpdateType UpdateType { get; set; }
        public bool IsDestroyed { get; private set; }
        
        private IUpdateManagerService _updateManager;
        private UpdateForce _registeredUpdate;
        private bool _isRegistered;

        [Inject] public void Init(IUpdateManagerService updateManager)
        {
            _updateManager = updateManager;
            Register();
        }

        private void Register()
        {
            if (_updateManager == null || _isRegistered)
                return;

            _registeredUpdate = _updateManager.GetUpdate(UpdateType);
            _registeredUpdate.Register(this);
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered)
                return;

            _registeredUpdate?.Unregister(this);
            _registeredUpdate = null;
            _isRegistered = false;
        }

        public virtual void OnDestroy()
        {
            IsDestroyed = true;
            Unregister();
        }
        
        public virtual void OnEnable()
        {
            Register();
        }

        public virtual void OnDisable()
        {
            Unregister();
        }
        
        public async UniTask RunWhile(
            Func<UniTask> tick,
            Func<bool> condition,
            PlayerLoopTiming timing,
            ILifetime lifetime)
        {
            var token = lifetime.GetToken();

            while (!IsDestroyed && !token.IsCancellationRequested && condition())
            {
                try
                {
                    if (tick != null)
                        await tick();
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                await UniTask.Yield(timing);
            }
        }

        public virtual void GameUpdate(float dt)
        {
            
        }
        
        public virtual void GameFixedUpdate(float dt)
        {
            
        }

        public virtual void GameEarlyUpdate(float dt)
        {
            
        }
        
        public virtual void GameLateUpdate(float dt)
        {
            
        }
    }
}
