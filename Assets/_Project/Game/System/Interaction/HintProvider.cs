using UnityEngine;

namespace Game.System.Interaction
{
    public class HintProvider : MonoBehaviour, IInteractionHintProvider
    {
        [SerializeField] private string _text = "Press E";
        [SerializeField] private bool _show = true;

        public bool ShouldShowHint => _show;

        public string GetHintText() => _text;
    }
}