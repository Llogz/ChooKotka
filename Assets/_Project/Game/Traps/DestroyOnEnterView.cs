
namespace Game.Traps
{
    public class DestroyOnEnterView : TrapBehaviourView
    {
        public override ITrapBehaviour CreateBehaviour()
        {
            return new DestroyOnEnterBehaviour(gameObject);
        }
    }
}
