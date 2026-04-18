using UnityEngine;
using VContainer;

namespace Core.Services
{
    public abstract class SceneService : MonoBehaviour
    {
        public abstract void Configure(IContainerBuilder builder);
    }
}