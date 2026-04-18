using System;
using VContainer;
using VContainer.Unity;

namespace Core.Services.SavingSystem
{
    /// <summary>
    /// THIS IS AN EXAMPLE AND HAS TO BE REMOVED LATER
    /// </summary>
    public class SavingExample : DataParser, IDataObserver, IInitializable
    {
        private double _money = 600;

        private IDataSubject _gameDataProvider;
        private const string GameDataKey = "Money";

        public double Money => _money;

        public event Action OnMoneyChanged;

        [Inject]
        public void Construct(IDataSubject gameDataProvider)
        {
            _gameDataProvider = gameDataProvider;
        }

        public void Initialize()
        {
            _gameDataProvider.AddObserver(this);
            OnMoneyChanged?.Invoke();
        }

        public void AddMoney(double amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be greater than 0.");

            _money += amount;
            OnMoneyChanged?.Invoke();
        }

        public bool TryRemoveMoney(double amount)
        {
            if (_money < amount)
                return false;

            _money -= amount;

            OnMoneyChanged?.Invoke();

            return true;
        }

        public DictionaryWrapperJson GetGameData()
        {
            DictionaryWrapperJson gameData = new DictionaryWrapperJson();
            gameData.AddRawData(GameDataKey, _money.ToString("F2"));

            return gameData;
        }

        public void SetGameData(DictionaryWrapperJson gameData)
        {
            _money = GetDoublePositiveData(gameData, GameDataKey);

            OnMoneyChanged?.Invoke();
        }
    }
}