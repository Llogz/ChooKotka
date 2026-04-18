using Core;
using Core.Services;
using Game.System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using InputSystem = Core.InputSystem;

namespace Game.Player
{
    public class ControllerAttack : ControllerAddition
    {
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private GroundChecker _groundChecker;
        [SerializeField, Range(0f, 1f)] private float controlsDeadZone = 0.1f;
        [SerializeField] private float speedMultiplier = 0.2f;
        [SerializeField] private int combo;
        [SerializeField] private int curCombo = 0;
        [SerializeField] private Collider2D leftAttack;
        [SerializeField] private Collider2D rightAttack;
        
        [Header("Delays")]
        [SerializeField] private float attackDelay;
        [SerializeField] private float comboDelay;
        [SerializeField] private float attackTime;
        
        [Header("Animations")]
        [SerializeField] private AnimController animController;
        [SerializeField] private string[] attackAnims;
        [SerializeField] private float attackAnimTime;
        private int _curAttackAnim = 0;
        
        private float _attackDir = 1f;
        
        private bool _isAttacking = false;
        private bool _canAttack = true;
        private bool _isCombo = false;
        
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
            _input.Player.Attack.performed += Attack;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _input.Player.Attack.performed -= Attack;
        }

        public override void GameUpdate(float dt)
        {
            if (rg.linearVelocityX > controlsDeadZone || rg.linearVelocityX < -controlsDeadZone) _attackDir = Mathf.Sign(rg.linearVelocityX);
            SpeedMultiplier = new Vector2(_isAttacking ? speedMultiplier : 1f, 1f);
        }

        private void Attack(InputAction.CallbackContext context)
        {
            if (!_canAttack || Block || !_groundChecker.IsTouchingGround) return;
            if (_isCombo)
            {
                curCombo++;
                if (curCombo >= combo + Addition)
                {
                    curCombo--;
                    return;
                }
            }
            else
            {
                _isCombo = true;
                _timer.Delay(comboDelay, () =>
                {
                    _isCombo = false;
                    curCombo = 0;
                }, UpdateType, Lifetime);
            }

            _isAttacking = true;
            _canAttack = false;
            _timer.Delay(attackDelay, () => _canAttack = true, UpdateType, Lifetime);
            _timer.Delay(attackTime, () =>
            {
                _isAttacking = false;
                leftAttack.enabled = (false);
                rightAttack.enabled = (false);
            }, UpdateType, Lifetime);
            leftAttack.enabled = (_attackDir < 0f);
            rightAttack.enabled = (_attackDir > 0f);
            
            animController.PullAnimation(attackAnims[_curAttackAnim], attackAnimTime);
            _curAttackAnim++;
            if (_curAttackAnim >= attackAnims.Length) _curAttackAnim = 0;
        }
    }
}