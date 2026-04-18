using Core.Services;
using Game.Player;
using Game.System.Services;
using UnityEngine;
using VContainer;

namespace Game.System
{
    public class BulletData : FabricObjectData
    {
        public float Power { get; set; }

        public BulletData(float power, 
            UpdateType updateType, Vector3 position, Quaternion rotation, Transform parent = null) 
            : base(updateType, position, rotation, parent)
        {
            Power = power;
        }
    }
    
    public class Bullet : FabricObject<BulletData>
    {
        public float Power { get; set; } = 20f;
        protected override void OnInitialize(BulletData data)
        {
            Power = data.Power;
        }
        
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private bool ignoreController;
        [SerializeField] private int damage;
        [SerializeField] private float damageBySpeedMultiplier = 1f;
        private float _additionalDamage = 0;
        [SerializeField] private float reflectSpeedMultiplier = 0.5f;
        [SerializeField] private float forceAdditionalAngle;
        [SerializeField] private float additionalRotate;

        [SerializeField] private VisualAction visualActionOnDamage;
        
        private bool _pin = false;
        private Vector3 _pinPos;
        private float _pinRotation;
        
        public override UpdateType UpdateType { get; set; }
        
        private IVisualActionsHandler _visualActionsHandler;
        [Inject]
        private void Init(IVisualActionsHandler visualActionsHandler)
        {
            _visualActionsHandler = visualActionsHandler;
        }

        private void Start()
        {
            Vector3 dir = ProjMath.MoveTowardsAngle(360f - transform.eulerAngles.z - forceAdditionalAngle);
            rg.linearVelocity = dir.normalized * Power;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponentInChildren<Controller>() & ignoreController 
                || collision.collider.isTrigger) return;
            
            if (collision.gameObject.TryGetComponent(out HealthController hc) && !_pin)
            {
                hc.ApplyDamage(damage + Mathf.RoundToInt(_additionalDamage));
                _visualActionsHandler.PullAction(visualActionOnDamage);
                Destroy(gameObject);
            }

            if (collision.gameObject.TryGetComponent(out Ground ground) && !_pin)
            {
                rg.linearVelocity = Vector2.zero;
                rg.gravityScale = 0f;
                rg.freezeRotation = true;
                
                _pin = true;
                _pinPos = transform.localPosition;
                _pinRotation = transform.localEulerAngles.z;
            }
        }
        public override void GameFixedUpdate(float dt)
        {
            if (_pin)
            {
                transform.localPosition = _pinPos;
                transform.localEulerAngles = new Vector3(0f, 0f, _pinRotation);
            }
            else
            {
                transform.eulerAngles = new Vector3(0f, 0f,
                    ProjMath.RotateTowardsPosition(rg.linearVelocity.normalized) + additionalRotate);
                
                _additionalDamage = Mathf.Max(_additionalDamage, Vector2.Distance(rg.linearVelocity, Vector2.zero) * damageBySpeedMultiplier);
            }
        }
    }
}