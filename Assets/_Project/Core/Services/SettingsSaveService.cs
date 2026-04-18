using System;
using UnityEngine;

namespace Core.Services
{
    public interface ISettingsSaveService
    {
        void Save(string key, float value);
        float Load(string key);
        bool HasKey(string key);
        Action OnSaved { get; set; }
    }
    
    public class SettingsSaveService : ISettingsSaveService
    {
        public void Save(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
            OnSaved?.Invoke();
        }

        public float Load(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public Action OnSaved { get; set; }
    }
}