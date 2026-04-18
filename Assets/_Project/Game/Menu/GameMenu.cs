using System;
using Core.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Menu
{
    public class GameMenu : UpdatableBehaviour
    {
        [Header("References")]
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private Menu blankMenu;
        [SerializeField] private Menu defaultMenu;
        [SerializeField] private GameButton openButton;

        [Header("Update")]
        [SerializeField] private UpdateType[] updateTypesSet;
        [SerializeField] private float timeScaleSet = 0.0f;
        private float[] _lastTimeSet;

        [Header("Visuals")]
        [SerializeField] private Image bg;
        [SerializeField] private Color bgClosedColor;
        [SerializeField] private Color bgOpenedColor;
        [SerializeField] private float bgSpeed;

        [Header("Other")]
        [SerializeField] private bool switchByKeyboard = true;

        public override UpdateType UpdateType { get; set; } = UpdateType.UI;
        
        private InputSystem _inputSystem;
        private IUpdateManagerService _updateManager;

        [Inject]
        private void Init(IInputController input, IUpdateManagerService updateManager)
        {
            _inputSystem = input.GetInputSystem();
            _updateManager = updateManager;
        }

        private bool _isMenuOpen = false;

        private void Awake()
        {
            UpdateType = UpdateType.UI;

            _lastTimeSet = new float[updateTypesSet.Length];
            for (int i = 0; i < updateTypesSet.Length; i++)
                _lastTimeSet[i] = _updateManager.GetUpdate(updateTypesSet[i]).TimeScale;

            CloseMenu();
        }

        public override void GameUpdate(float dt)
        {
            bg.color = Color.Lerp(
                bg.color,
                _isMenuOpen ? bgOpenedColor : bgClosedColor,
                dt * bgSpeed
            );
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (switchByKeyboard) _inputSystem.UI.Menu.performed += OnMenuPressed;
            if (openButton != null) openButton.OnButtonPressed += ToggleMenu;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (switchByKeyboard) _inputSystem.UI.Menu.performed -= OnMenuPressed;
            if (openButton != null) openButton.OnButtonPressed -= ToggleMenu;
        }

        private void OnMenuPressed(InputAction.CallbackContext context)
        {
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if (_isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }

        private void OpenMenu()
        {
            _isMenuOpen = true;

            menuManager.Open(defaultMenu.Name);

            for (int i = 0; i < updateTypesSet.Length; i++)
                _updateManager.GetUpdate(updateTypesSet[i]).TimeScale = timeScaleSet;
        }

        private void CloseMenu()
        {
            _isMenuOpen = false;

            menuManager.Open(blankMenu.Name);

            for (int i = 0; i < updateTypesSet.Length; i++)
                _updateManager.GetUpdate(updateTypesSet[i]).TimeScale = _lastTimeSet[i];
        }
    }
}
