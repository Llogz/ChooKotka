using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.SavingSystem
{
    public interface IDataSubject
    {
        public List<IDataObserver> Observers { get; }

        public void AddObserver(IDataObserver dataObserver);
        public void RemoveObserver(IDataObserver dataObserver);
        public DictionaryWrapperJson GetGameData();

        public void SetGameData(DictionaryWrapperJson gameData);
    }
    
    public class GameDataProvider : IDataSubject
    {
        private List<IDataObserver> _observers = new();

        public List<IDataObserver> Observers => new(_observers);

        public void AddObserver(IDataObserver dataObserver)
        {
            if (_observers.Contains(dataObserver))
                throw new Exception("Observer already added.");

            _observers.Add(dataObserver);
        }

        public void RemoveObserver(IDataObserver dataObserver)
        {
            if (_observers.Contains(dataObserver) == false)
                throw new Exception("Observer not found.");

            _observers.Remove(dataObserver);
        }

        public DictionaryWrapperJson GetGameData()
        {
            DictionaryWrapperJson gameData = new DictionaryWrapperJson();

            foreach (var observer in _observers)
            {
                gameData.AddRange(observer.GetGameData());
            }

            return gameData;
        }

        public void SetGameData(DictionaryWrapperJson gameData)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.SetGameData(gameData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка при обновлении observer {observer.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
}