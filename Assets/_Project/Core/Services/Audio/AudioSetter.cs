using System;
using Core.Services;
using UnityEngine;
using VContainer;

namespace Core.Services
{
    public class AudioSetter : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private string saveKey;

        private float _initialVolume = 1f;
        private bool _hasStarted = false;
        
        private ISettingsSaveService _saveService;
        [Inject]
        private void Init(ISettingsSaveService saveService)
        {
            _saveService = saveService;
            
            _hasStarted = true;
            _saveService.OnSaved += Load;
            
            _initialVolume = audioSource.volume;
            Load();
        }

        private void Load()
        {
            audioSource.volume = _initialVolume * _saveService.Load(saveKey);
        }

        private void OnEnable()
        {
            if (_hasStarted) _saveService.OnSaved += Load;
        }

        private void OnDisable()
        {
            _saveService.OnSaved -= Load;
        }
    }
}
