using System;
using System.Collections.Generic;
using Core.Services;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using VContainer;

namespace Game.Menu
{
    public enum SettingType
    {
        Slider,
        Toggle
    }

    [Serializable]
    public struct SettingData
    {
        public LocalizedStringTable nameTable;
        public string nameKey;
        
        public string saveKey;

        public SettingType type;
        public float value;
        public float minValue;
        public float maxValue;
    }

    public class SettingsMenuManager : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private SettingData[] settings;
        [SerializeField] private SettingView settingViewPrefab;

        private readonly Dictionary<string, SettingView> _views = new();

        private ISettingsSaveService _saveService;
        [Inject]
        private void Init(ISettingsSaveService saveService)
        {
            _saveService = saveService;
        }
        
        private void Awake()
        {
            Setup();
            Load();
        }

        private void Translate(Locale locale)
        {
            foreach (var setting in settings)
            {
                _views[setting.saveKey].Title.text = setting.nameTable.GetTable().GetEntry(setting.nameKey).Value;
            }
        }
        
        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += Translate;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= Translate;
        }

        private void Setup()
        {
            foreach (var setting in settings)
            {
                var view = Instantiate(settingViewPrefab, root);
                view.Title.text = setting.nameTable.GetTable().GetEntry(setting.nameKey).Value;

                _views[setting.saveKey] = view;

                switch (setting.type)
                {
                    case SettingType.Slider:
                        view.Slider.gameObject.SetActive(true);
                        view.Toggle.gameObject.SetActive(false);

                        view.Slider.minValue = setting.minValue;
                        view.Slider.maxValue = setting.maxValue;

                        view.Slider.onValueChanged.AddListener(value =>
                        {
                            _saveService.Save(setting.saveKey, value);
                        });
                        break;

                    case SettingType.Toggle:
                        view.Slider.gameObject.SetActive(false);
                        view.Toggle.gameObject.SetActive(true);

                        view.Toggle.onValueChanged.AddListener(value =>
                        {
                            _saveService.Save(setting.saveKey, value ? 1 : 0);
                        });
                        break;
                }
            }
        }

        private void Load()
        {
            foreach (var setting in settings)
            {
                var view = _views[setting.saveKey];

                switch (setting.type)
                {
                    case SettingType.Slider:
                        float sliderValue = _saveService.HasKey(setting.saveKey)
                            ? _saveService.Load(setting.saveKey)
                            : setting.value;

                        view.Slider.SetValueWithoutNotify(sliderValue);
                        _saveService.Save(setting.saveKey, sliderValue);
                        break;

                    case SettingType.Toggle:
                        bool toggleValue = _saveService.HasKey(setting.saveKey)
                            ? _saveService.Load(setting.saveKey) == 1f
                            : setting.value > 0.5f;

                        view.Toggle.SetIsOnWithoutNotify(toggleValue);
                        _saveService.Save(setting.saveKey, toggleValue ? 1 : 0);
                        break;
                }
            }
        }
    }
}
