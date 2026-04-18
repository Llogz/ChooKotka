using Core;
using Core.Services;
using UnityEngine;

namespace Game.Player
{
    public class ControllerAddition : UpdatableBehaviour
    {
        public int Addition { get; set; } = 0;
        public bool Block { get; set; } = false;
        public Vector2 AdditionalSpeed { get; set; } = Vector2.zero;
        public Vector2 SpeedMultiplier { get; set; } = new Vector2(1f, 1f);
        public ILifetime Lifetime { get; set; }
        public override UpdateType UpdateType { get; set; }
    }
}