using System;
using Core;
using Core.Services;
using Game.Player;
using Game.System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Player
{
    public class ControllerDash : ControllerAddition
    {
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private float rgDeadZone = 0.1f;
        [SerializeField] private float dashForce;
        [SerializeField] private float dashForceExpireSpeed;
        [SerializeField] private float gravitySet;
        [SerializeField] private float minDashForceToGravitySet = 0.1f;
        [SerializeField] private float dashDelay = 1f;
        
        [Header("Animation")]
        [SerializeField] private AnimController animController;
        [SerializeField] private string dashAnim;
        [SerializeField] private float dashAnimTime;
        
        private float _curDashPower = 0f;
        
        private bool _canDash = true;
        private bool _dashBlock = false;
        
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
            _input.Player.Dash.performed += Dash;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _input.Player.Dash.performed -= Dash;
        }

        public override void GameUpdate(float dt)
        {
            if (rg.linearVelocityY == 0f) _canDash = true;
            _curDashPower = Mathf.Lerp(_curDashPower, 0f, Time.deltaTime * dashForceExpireSpeed);
            rg.linearVelocityY = Mathf.Abs(_curDashPower) > minDashForceToGravitySet ? gravitySet : rg.linearVelocityY;
            
            AdditionalSpeed = new Vector2(_curDashPower, 0f);
        }

        private void Dash(InputAction.CallbackContext context)
        {
            if (Block) return;
            if (_dashBlock || !_canDash) return;
            _dashBlock = true;
            _canDash = false;
            _timer.Delay(dashDelay, () => _dashBlock = false, UpdateType, Lifetime);
            if (rg.linearVelocityX > rgDeadZone) _curDashPower += dashForce + Addition;
            else if (rg.linearVelocityX < -rgDeadZone) _curDashPower -= dashForce + Addition;
            else return;
            animController.PullAnimation(dashAnim, dashAnimTime);
        }
    }
}