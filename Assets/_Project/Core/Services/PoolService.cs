using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Core.Services
{
    public interface IPoolService
    {
        GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null);
        void Release(GameObject instance);
    }

    public class PooledObject : MonoBehaviour
    {
        public ObjectPool<GameObject> Pool { get; set; }
    }
    
    public class PoolService : SceneService, IPoolService
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IPoolService>();
        }

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 100;
        
        private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab);
                _pools[prefab] = pool;
            }

            var go = pool.Get();
            go.transform.SetPositionAndRotation(pos, rot);
            go.transform.SetParent(parent);
            go.SetActive(true);
            return go;
        }

        public void Release(GameObject instance)
        {
            instance.SetActive(false);

            var pooled = instance.GetComponent<PooledObject>();
            pooled.Pool.Release(instance);
        }

        private ObjectPool<GameObject> CreatePool(GameObject prefab)
        {
            ObjectPool<GameObject> pool = null;

            pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var go = Instantiate(prefab);
                    var po = go.AddComponent<PooledObject>();
                    po.Pool = pool;
                    return go;
                },
                actionOnGet: go => go.SetActive(true),
                actionOnRelease: go => go.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            return pool;
        }
    }
}