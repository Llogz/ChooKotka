using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Services
{
    public interface IFabricService
    {
        T Create<T, TData>(TData data, GameObject prefab, bool usePool)
            where T : FabricObject<TData>
            where TData : FabricObjectData;
    }

    public class FabricService : IFabricService
    {
        private readonly IObjectResolver _resolver;
        public FabricService(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T Create<T, TData>(TData data, GameObject prefab, bool usePool)
            where T : FabricObject<TData>
            where TData : FabricObjectData
        {
            GameObject go;
            if (_resolver.TryResolve<IPoolService>(out var pool) && usePool) 
                 go = pool.Get(prefab, data.Position, data.Rotation, data.Parent);
            else go = Object.Instantiate(prefab, data.Position, data.Rotation, data.Parent);

            var instance = go.GetComponent<T>();
            if (instance == null)
            {
                Debug.LogError($"Prefab {prefab.name} does not contain {typeof(T).Name}");
                return null;
            }

            instance.Initialize(data);
            _resolver.InjectGameObject(go);
            
            return instance;
        }
    }
}
