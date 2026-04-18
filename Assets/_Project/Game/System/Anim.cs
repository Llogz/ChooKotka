using System;
using Core.Services;
using UnityEngine;

namespace Game.System
{
    public class Anim : UpdatableBehaviour
    {
        [Header("For Controller")]
        
        [SerializeField] private string animName;
        public string AnimName => animName;
        
        [SerializeField] private int priority;
        public int Priority => priority;
        
        [Header("Setup")]
        [SerializeField] private SpriteRenderer spr;
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float timePerFrame;
        [SerializeField] private bool loop = true;
        
        private int _currentFrame = 0;
        private float _curTime = 0f;
        
        public override UpdateType UpdateType { get; set; }

        private void SwitchFrame()
        {
            _currentFrame++;
            if (_currentFrame >= frames.Length)
            {
                if (loop) _currentFrame = 0;
                else _currentFrame = frames.Length - 1;
            }
        }

        public void Reset()
        {
            _currentFrame = 0;
        }

        public override void GameUpdate(float dt)
        {
            spr.sprite = frames[_currentFrame];
            
            _curTime -= dt;
            if (_curTime <= 0)
            {
                SwitchFrame();
                _curTime = timePerFrame;
            }
        }
    }
}
