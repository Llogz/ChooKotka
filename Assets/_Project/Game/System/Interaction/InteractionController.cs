using System;
using Core.Services;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using InputSystem = Core.InputSystem;

namespace Game.System.Interaction
{
    public sealed class InteractionController : IStartable, IDisposable
    {
        private readonly InputSystem _input;
        private readonly InteractionDetector _detector;
        private readonly IInteractionContext _context;

        [Inject]
        public InteractionController(
            IInputController input,
            InteractionDetector detector,
            IInteractionContext context)
        {
            _input = input.GetInputSystem();
            _detector = detector;
            _context = context;
        }

        public void Start()
        {
            _input.Player.Enable();
            _input.Player.Use.performed += OnInteract;
        }

        public void Dispose()
        {
            _input.Player.Use.performed -= OnInteract;
            _input.Player.Disable();
        }

        private void OnInteract(InputAction.CallbackContext _)
        {
            var current = _detector.Current;

            if (current == null)
                return;

            if (current.CanInteract(_context))
            {
                current.Interact(_context);
            }
        }
    }
}