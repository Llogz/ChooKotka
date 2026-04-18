using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;

namespace Core.Services
{
    public interface IAudioService
    {
        void PlaySound(AudioResource resource, bool isMusic, float volume, Vector3 position, float minDistance, float maxDistance, bool loop = false);
    }

    public class AudioService : SceneService, IAudioService
    {
        [Inject] private IPoolService _poolService;
        [Inject] private IObjectResolver _objectResolver;

        [SerializeField] private GameObject soundPrefab;
        [SerializeField] private GameObject musicPrefab;
        private readonly List<AudioSource> _curSources = new();
        
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IAudioService>().AsSelf();
        }
        
        public void PlaySound(AudioResource resource, bool isMusic, float volume, Vector3 position, float minDistance, float maxDistance, bool loop = false)
        {
            var obj = _poolService.Get(isMusic ? musicPrefab : soundPrefab, position, Quaternion.identity);
            var source = obj.GetComponent<AudioSource>();

            source.resource =  resource;
            source.loop = loop;
            source.volume = volume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;

            source.Play();
            _curSources.Add(source);
            
            _objectResolver.InjectGameObject(obj);
        }

        private void Update()
        {
            foreach (var source in _curSources.ToList())
            {
                if (!source)
                {
                    _curSources.Remove(source);
                    continue;
                }

                if (!source.isPlaying)
                {
                    _curSources.Remove(source);
                    _poolService.Release(source.gameObject);
                }
            }
        }
    }
}