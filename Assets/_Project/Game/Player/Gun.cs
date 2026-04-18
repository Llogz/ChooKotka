using System;
using Core;
using Core.Services;
using Game.System;
using UnityEngine;
using VContainer;

namespace Game.Player
{
    public class Gun : UpdatableBehaviour
    {
        public Transform Target { get; set; }
        
        [SerializeField] private bool pinToMouse = true;
    
        [SerializeField] private float rotationOffset;
        [SerializeField] private float rotationOffsetByX;
        [SerializeField] private float[] rotationLocks;
    
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
    
        [Header("Animation")]
        [SerializeField] private AnimController animController;
        [SerializeField] private string shootAnim;
        [SerializeField] private float shootAnimTime;

        public override UpdateType UpdateType { get; set; }

        private IFabricService _fabric;
        private InputSystem _input;
        [Inject]
        private void Init(IFabricService fabric, IInputController inputController)
        {
            _fabric = fabric;
            _input = inputController.GetInputSystem();
        }
        
        private float FindClosestRotation(float rotation) //by chatgpt
        {
            float closest = rotationLocks[0];
            float minDiff = 360f;

            foreach (float lockAngle in rotationLocks)
            {
                float diff = Mathf.Abs(Mathf.DeltaAngle(rotation, lockAngle));

                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = lockAngle;
                }
            }

            return closest;
        }
    
        public void Shoot(float power)
        {
            _fabric.Create<Bullet, BulletData>(
                new BulletData(power, UpdateType, bulletSpawnPoint.position, bulletSpawnPoint.rotation), 
                bulletPrefab.gameObject, false
                );
            if (animController != null) animController.PullAnimation(shootAnim, shootAnimTime);
        }

        public override void GameUpdate(float dt)
        {
            if (!Target && !pinToMouse) return;
        
            Vector2 dir = new Vector2(transform.position.x, transform.position.y) - (pinToMouse ? ProjMath.MousePosition(_input) : Target.position);
            float rotation = ProjMath.RotateTowardsPosition(dir.normalized);
        
            Vector3 targetPos = !Target ? ProjMath.MousePosition(_input) : Target.position;
        
            if (rotationLocks.Length > 0)
                rotation = FindClosestRotation(rotation);
            transform.eulerAngles = new Vector3(0f, 0f, rotation + rotationOffset + (targetPos.x > transform.position.x ? rotationOffsetByX : -rotationOffsetByX));
        }
    }
}