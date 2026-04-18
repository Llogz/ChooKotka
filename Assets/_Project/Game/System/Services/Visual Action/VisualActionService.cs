using Core.Services;
using Game.Player;
using VContainer;

namespace Game.System.Services
{
    public class VisualActionService : SceneService, IVisualActionsHandler
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IVisualActionsHandler>();
        }

        /// <summary>
        /// There can be anything that is related to scene, no limits
        /// </summary>
        /// <param name="action"></param>
        public void PullAction(VisualAction action)
        {
            CameraMovement.Shake(action.shakePower);
        }
    }
}
