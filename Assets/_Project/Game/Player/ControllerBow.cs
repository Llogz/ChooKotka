using Core;
using Core.Services;
using Game.System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Player
{
    public class ControllerBow : ControllerAddition
    {
        public int Arrows { get; set; } = 10;
        
        [SerializeField] private ControllerAddition[] controllersBlock;
        [SerializeField] private Gun gun;
        [SerializeField] private float shootPower = 20f;
        [SerializeField] private float timeToLoadSet;
        [SerializeField] private float startTimeToLoad;
        [SerializeField] private float preLoadTime;
        [SerializeField] private float loadSpeedMultiplier;

        [Header("Animations")]
        [SerializeField] private SpriteRenderer spr;
        [SerializeField] private AnimController animController;
        [SerializeField] private string holdAnim;
        [SerializeField] private float holdAnimTime = 0.1f;
        [SerializeField] private string shootAnim;
        [SerializeField] private float shootAnimTime = 0.1f;
        
        private float _curLoadTime = 0f;
        
        private bool _isHoldingWhileLoad = false;
        private bool _isHolding = false;
        
        private bool _isLoading = false;
        
        private ITimerService _timer;
        private InputSystem _input;
        [Inject] private void Init(
            IInputController inputController,
            ITimerService timer
        )
        {
            _input = inputController.GetInputSystem();
            _timer = timer;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _input.Player.Shoot.performed += Shoot;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _input.Player.Shoot.performed -= Shoot;
        }

        public override void GameUpdate(float dt)
        {
            SpeedMultiplier = new Vector2(_isHolding ? loadSpeedMultiplier : 1.0f, 1f);

            foreach (var controller in controllersBlock)
            {
                controller.Block = _isHolding;
            }
            
            if (_isHolding)
            {
                spr.flipX = _input.UI.MousePosition.ReadValue<Vector2>().x < Screen.width / 2f;
                animController.PullAnimation(holdAnim, holdAnimTime);
                _curLoadTime += dt;
                _curLoadTime = Mathf.Clamp(_curLoadTime, 0f, timeToLoadSet);
            }
        }

        private void Shoot(InputAction.CallbackContext context)
        {
            if (!gun || !gun.gameObject.activeInHierarchy) return;

            Controller playerController = GetComponent<Controller>();
            if (playerController != null && !playerController.CanMove) return;

            _isHoldingWhileLoad = !_isHoldingWhileLoad;
            if (_isLoading || Arrows <= 0) return;

            _isHolding = !_isHolding;
            if (!_isHoldingWhileLoad && _isHolding)
            {
                _isHoldingWhileLoad = false;
                _isHolding = false;
                return;
            }
            if (!_isHolding)
            {
                gun.Shoot(shootPower * (1f / timeToLoadSet * (_curLoadTime + startTimeToLoad)));
                _curLoadTime = 0f;
                _timer.Delay(preLoadTime, () => _isLoading = false, UpdateType, Lifetime);
                _isLoading = true;
                
                Arrows--;
                animController.PullAnimation(shootAnim, shootAnimTime);
            }
        }
    }
}