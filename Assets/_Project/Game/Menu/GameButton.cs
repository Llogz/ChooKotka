using System;
using Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Menu
{
    public enum GameButtonAction
    {
        Nothing,
        StartScene,
        ChangeMenu,
        ExitGame
    }

    public sealed class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action OnButtonPressed { get; set; }

        [SerializeField] private GameButtonAction action;
        [SerializeField] private string index;
        [SerializeField] private MenuManager menuManager;

        private bool _isMouseOn = false;

        private InputSystem _inputSystem;
        private ISceneChangerService _sceneChanger;
        [Inject] public void Init(IInputController controller, ISceneChangerService sceneChanger)
        {
            _inputSystem = controller.GetInputSystem();
            _sceneChanger = sceneChanger;
        }

        public void OnMouseEnter()
        {
            _isMouseOn = true;
        }

        public void OnMouseExit()
        {
            _isMouseOn = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExit();
        }

        private bool _hasStarted = false;
        public void Start()
        {
            _inputSystem.UI.Click.performed += OnClick;
            _hasStarted = true;
        }

        public void OnEnable()
        {
            if (!_hasStarted) _inputSystem.UI.Click.performed += OnClick;
        }

        public void OnDisable()
        {
            _inputSystem.UI.Click.performed -= OnClick;
        }

        private bool _isHolding = false;
        public void OnClick(InputAction.CallbackContext context)
        {
            _isHolding = !_isHolding;
            if (_isHolding) return;
            
            if (!_isMouseOn) return;

            switch (action)
            {
                case GameButtonAction.StartScene:
                    _sceneChanger.ChangeScene(index); break;
                case GameButtonAction.ChangeMenu:
                    menuManager.Open(index); break;
                case GameButtonAction.ExitGame:
                    Application.Quit(); break;
            }

            OnButtonPressed?.Invoke();
        }
    }
}