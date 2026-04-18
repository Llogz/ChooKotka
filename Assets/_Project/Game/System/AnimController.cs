using Core.Services;
using UnityEngine;

namespace Game.System
{
    public class AnimController : UpdatableBehaviour
    {
        [SerializeField] private Anim[] animators;
        private float[] _animatorTimers;
        
        private Anim _curAnim;
        private int _lastAnimId = 0;
        
        public override UpdateType UpdateType { get; set; }
    
        public void PullAnimation(string key, float time)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                if (animators[i].AnimName == key)
                {
                    _animatorTimers[i] = time;
                }
            }
        }

        public void ResetAnimation()
        {
            if (_curAnim != null) _curAnim.Reset();
        }

        public void ResetController()
        {
            Awake();
            _curAnim = null;
        }

        private void Awake()
        {
            if (animators.Length == 0) animators = GetComponents<Anim>();
            _animatorTimers = new float[animators.Length];
            for (int i = 0; i < animators.Length; i++) _animatorTimers[i] = 0f;
        }
        
        public override void GameUpdate(float dt)
        {
            if (!_curAnim) _curAnim = animators[0];
        
            for (int i = 0; i < animators.Length; i++)
            {
                _animatorTimers[i] -= dt;
                if (_animatorTimers[i] > 0f && animators[i].Priority > _curAnim.Priority | _animatorTimers[_lastAnimId] <= 0f)
                {
                    _lastAnimId = i;
                    _curAnim = animators[i];
                }
            }
        
            foreach (var anim in animators)
            {
                if (anim != _curAnim)
                {
                    anim.enabled = false;
                    anim.Reset();
                }
            }
            _curAnim.enabled = true;
        }
    }
}
