using System;
using Core.Services;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using VContainer;

namespace Game.Menu
{
    public class LocaleSetButton : MonoBehaviour
    {
        [SerializeField] private GameButton button;
        [SerializeField] private Locale locale;

        private void SetLocale()
        {
            LocalizationSettings.SelectedLocale = locale;
        }

        private void OnEnable()
        {
            button.OnButtonPressed += SetLocale;
        }

        private void OnDisable()
        {
            button.OnButtonPressed -= SetLocale;
        }
    }
}