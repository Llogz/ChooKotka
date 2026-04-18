using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.Services
{
    public interface IUpdateManagerService
    {
        UpdateForce GetUpdate(UpdateType updateType);
    }
    
    public enum UpdateType
    {
        Game,
        UI
    }
    
    public class UpdateManagerService : SceneService, IUpdateManagerService
    {
        [Inject] private ILifetime _lifetime;
        
        [SerializeField] private UpdateType attachPhysicsUpdateType = UpdateType.Game;
        [SerializeField] private PlayerLoopTiming attachPhysicsUpdateTiming = PlayerLoopTiming.FixedUpdate;
        
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IUpdateManagerService>().AsSelf();
        }
        
        private readonly Dictionary<UpdateType, UpdateForce> _updates = new();
        public UpdateManagerService()
        {
            foreach (var t in (UpdateType[])Enum.GetValues(typeof(UpdateType)))
                _updates.Add(t, new UpdateForce());
        }

        private async void Start()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate); // So all updates won't start before any Start method
            
            foreach (var updateKey in _updates.Keys)
            {
                _updates[updateKey].StartUpdateLoop(PlayerLoopTiming.EarlyUpdate, _lifetime);
                _updates[updateKey].StartUpdateLoop(PlayerLoopTiming.PreLateUpdate, _lifetime);
                _updates[updateKey].StartUpdateLoop(PlayerLoopTiming.Update, _lifetime);
                _updates[updateKey].StartUpdateLoop(PlayerLoopTiming.FixedUpdate, _lifetime);
                
                if (attachPhysicsUpdateType == updateKey) 
                    _updates[updateKey].StartPhysicsLoop(attachPhysicsUpdateTiming, _lifetime);
            }
        }

        public UpdateForce GetUpdate(UpdateType updateType) => _updates[updateType];
    }
}