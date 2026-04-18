using Core.Services;
using UnityEngine;
using VContainer;

namespace Game.Player
{
    public class CameraMovement : UpdatableBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed;
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool yLock = true;
        [SerializeField] private float shakeExpireSpeed;
        [SerializeField] private float minX = -999;
        [SerializeField] private float maxX = 999;
        [SerializeField] private float borderOffset = 7.25f;
        [SerializeField] private string shakeSaveKey = "shake";

        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        private ISettingsSaveService _settings;
        [Inject]
        private void Init(ISettingsSaveService settings)
        {
            _settings = settings;
        }

        private static float _shakePowerSet = 1f;
        private static Vector3 _curShakeOffset;
        public static void Shake(float power, bool z = false)
        {
            power *= _shakePowerSet;
            _curShakeOffset = new Vector3(
                Random.Range(-power, power),
                Random.Range(-power, power),
                z ?  Random.Range(-power, power) : 0
                );
        }

        private void OnSave()
        {
            _shakePowerSet = _settings.Load(shakeSaveKey);
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            _settings.OnSaved += OnSave;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _settings.OnSaved -= OnSave;
        }

        private float _startY = 0f;
        private void Start()
        {
            _startY = transform.position.y;
            Vector3 targetPos = target.position + offset;
            targetPos.y = yLock ? _startY : target.position.y;
            transform.position = targetPos + offset;
        }
        public override void GameFixedUpdate(float dt)
        {
            if (target == null) return;
            _curShakeOffset = Vector3.Lerp(_curShakeOffset, Vector3.zero, shakeExpireSpeed * dt);
            Vector3 targetPos = target.position + offset;
            targetPos.y = yLock ? _startY : targetPos.y;
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            transform.position = Vector3.Lerp(transform.position, targetPos + _curShakeOffset, speed * dt);
        }
    }
}