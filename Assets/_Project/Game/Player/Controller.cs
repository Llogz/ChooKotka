using Core;
using Core.Services;
using Game.System;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Game.Player
{
    public class Controller : UpdatableBehaviour
    { 
        public bool CanMove { get; set; } = true;
        public float AdditionalSpeed { get; set; } = 0f;
        public float AdditionalAcceleration { get; set; } = 0f;
        public float AdditionalDeceleration { get; set; } = 0f;

        [SerializeField] private ControllerAddition[] controllerAdditions;

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private GroundChecker groundChecker;
        
        [SerializeField] private float acceleration;
        [SerializeField] private float speed;
        [SerializeField] private float deceleration;

        [SerializeField, Range(0f, 1f)] private float controlsDeadZone = 0.1f;
        [SerializeField, Range(0f, 1f)] private float decelerationDeadZone = 0.05f;

        [Header("Animation")] 
        [SerializeField] private SpriteRenderer spr;
        [SerializeField] private AnimController animController;
        [SerializeField] private string idleAnim;
        [SerializeField] private float idleAnimTime = 0.1f;
        [SerializeField] private string runAnim;
        [SerializeField] private float runAnimTime = 0.1f;
        
        [Header("Sound")]
        [SerializeField] private AudioResource[] sounds;
        [SerializeField] private float soundsDelay;
        [SerializeField] private float volume;
        [SerializeField] private float soundMinDistance;
        [SerializeField] private float soundMaxDistance;
        
        private Vector2 _curSpeed = Vector2.zero;
        private Vector2 _controls = Vector2.zero;

        private bool _canMakeSound = true;
        private float _curSoundDelayMultiplier;

        private ILifetime _caLifetime;
        
        private InputSystem _input;
        private IAudioService _audio;
        private ITimerService _timer;
        private ILifetime _lifetime;
        [Inject] private void Init(
            IInputController inputController, 
            IAudioService audioService,
            ITimerService timer,
            ILifetime lifetime
            )
        {
            _input = inputController.GetInputSystem();
            _audio = audioService;
            _timer = timer;
            _lifetime = lifetime;
        }

        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        private void Move(float dt)
        {
            _controls = _input.Player.Move.ReadValue<Vector2>();

            _curSpeed += Vector2.Lerp(Vector2.zero, _controls, (acceleration + AdditionalAcceleration) * dt);

            if (_controls.x < controlsDeadZone && _controls.x > -controlsDeadZone)
            {
                float x = -_curSpeed.x;
                if (x > 0f) x = 1f;
                if (x < 0f) x = -1f;
                _curSpeed += Vector2.Lerp(Vector2.zero, new Vector2(x, 0f), dt * (deceleration + AdditionalDeceleration));

                if (_curSpeed.x < decelerationDeadZone && _curSpeed.x > -decelerationDeadZone)
                {
                    _curSpeed.x = 0f;
                }
            }

            Vector2 additive = Vector2.zero;
            Vector2 additiveM = new Vector2(1.0f, 1.0f);
            foreach (ControllerAddition ca in controllerAdditions)
            {
                additive += ca.AdditionalSpeed;
                additiveM *= ca.SpeedMultiplier;
            }
            _curSoundDelayMultiplier = additiveM.x;

            _curSpeed.x = Mathf.Clamp(_curSpeed.x, -(speed + AdditionalSpeed), speed + AdditionalSpeed);
            rg.linearVelocityX = _curSpeed.x * additiveM.x + additive.x;
        }
        
        private void Awake()
        {
            if (controllerAdditions.Length == 0) controllerAdditions = GetComponents<ControllerAddition>();
            
            _caLifetime = _lifetime.Child();
            foreach (ControllerAddition ca in controllerAdditions)
            {
                ca.UpdateType = UpdateType;
                ca.Lifetime = _caLifetime;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _caLifetime.Dispose();
        }

        public override void GameFixedUpdate(float dt)
        {
            foreach (ControllerAddition ca in controllerAdditions)
            {
                ca.enabled = true;
            }

            if (CanMove)
            {
                Move(dt);
            }
            else
            {
                rg.linearVelocityX = 0f;
            }
            
            if (rg.linearVelocityX > controlsDeadZone || rg.linearVelocityX < -controlsDeadZone)
            {
                spr.flipX = rg.linearVelocityX < 0f;
                animController.PullAnimation(runAnim, runAnimTime);

                if (_canMakeSound && groundChecker.IsTouchingGround)
                {
                    _audio.PlaySound(
                        sounds[Random.Range(0, sounds.Length - 1)], 
                        false, volume, transform.position, soundMinDistance, soundMinDistance
                        );
                    
                    _canMakeSound = false;
                    _timer.Delay(soundsDelay / _curSoundDelayMultiplier, () => _canMakeSound = true, UpdateType, _lifetime);
                }
            }
            else animController.PullAnimation(idleAnim, idleAnimTime);
        }
    }
}
