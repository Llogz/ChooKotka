using System;
using Core;
using Core.Services;
using Game.System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Player
{
    public class ControllerJump : ControllerAddition
    {
        private int _curAdditionalJumps = 0;
        
        private bool _hasJumped = false;
        private bool _jumpBlock = false;

        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private GroundChecker groundChecker;
        
        [SerializeField] private int jumps = 1;
        [SerializeField] private float jumpForce;

        [SerializeField] private float jumpBlockDelay;

        [Header("Animation")]
        [SerializeField] private AnimController animController;
        [SerializeField] private string jumpAnim;
        [SerializeField] private float jumpAnimTime;

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
            _input.Player.Jump.performed += Jump;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _input.Player.Jump.performed -= Jump;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (Block) return;
            if (!groundChecker.IsTouchingGround & _curAdditionalJumps <= 0 || _jumpBlock) return;
            
            _jumpBlock = true;
            _timer.Delay(jumpBlockDelay, () => _jumpBlock = false, UpdateType, Lifetime);
            _curAdditionalJumps--;
            if (groundChecker.IsTouchingGround) _curAdditionalJumps = jumps + Addition;
            rg.linearVelocityY = jumpForce;
            
            animController.PullAnimation(jumpAnim, jumpAnimTime);
            animController.ResetAnimation();
            _hasJumped = true;
        }

        public override void GameUpdate(float dt)
        {
            if (!_hasJumped) return;
            _hasJumped = !(groundChecker.IsTouchingGround & !_jumpBlock);
            if (!_hasJumped)
            {
                animController.ResetController();
                return;
            }
            animController.PullAnimation(jumpAnim, jumpAnimTime);
        }
    }

}