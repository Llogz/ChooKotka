namespace Game.System.Interaction
{
    public interface IInteractionHintProvider
    {
        bool ShouldShowHint { get; }
        string GetHintText();
    }
}