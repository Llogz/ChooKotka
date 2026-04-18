using UnityEngine;

namespace Game.System.Interaction
{
    public sealed class InteractionHintLogger : MonoBehaviour
    {
        [SerializeField] private InteractionDetector _detector;

        private void OnEnable()
        {
            _detector.OnChanged += OnChanged;
        }

        private void OnDisable()
        {
            _detector.OnChanged -= OnChanged;
        }

        private void OnChanged(IInteractable interactable)
        {
            if (interactable == null)
            {
                Debug.Log("Hide hint");
                return;
            }

            var component = (interactable as Component);
            var hint = component.GetComponent<IInteractionHintProvider>();

            if (hint != null && hint.ShouldShowHint)
            {
                Debug.Log(hint.GetHintText());
            }
        }
    }
}