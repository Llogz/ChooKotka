using UnityEngine;

namespace Core.Services
{
    public abstract class FabricObjectData
    {
        public UpdateType UpdateType { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation  { get; }
        public Transform Parent  { get; }

        public FabricObjectData(UpdateType updateType, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            UpdateType = updateType;
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }
    }
}