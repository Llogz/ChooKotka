using Core;
using Game.Menu;
using Game.System.Interaction;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Lifetime = VContainer.Lifetime;

namespace Game
{
    public class GameRoot : Root
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private InteractionDetector _detector;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            if (_detector != null)
            {
                builder.RegisterInstance(_detector);

                builder.Register<IInteractionContext>(resolver =>
                        new PlayerInteractionContext(_player),
                    Lifetime.Singleton
                );

                builder.RegisterEntryPoint<InteractionController>();
            }
            else Debug.LogWarning("No Interaction detector was selected!");
        }
    }
}