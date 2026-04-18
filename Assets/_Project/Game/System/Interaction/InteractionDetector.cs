using System;
using UnityEngine;

namespace Game.System.Interaction
{
    public sealed class InteractionDetector : MonoBehaviour
    {
        public IInteractable Current { get; private set; }

        public event Action<IInteractable> OnChanged;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();

            if (interactable == null)
                return;

            Current = interactable;
            OnChanged?.Invoke(Current);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();

            if (interactable == null)
                return;

            if (Current == interactable)
            {
                Current = null;
                OnChanged?.Invoke(null);
            }
        }
    }
}