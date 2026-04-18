using Core.Services;

namespace Game.Traps
{
    public abstract class TrapBehaviourView : UpdatableBehaviour
    {
        public abstract ITrapBehaviour CreateBehaviour();
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;
    }
}