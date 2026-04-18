
namespace Core.Services.SavingSystem
{
    public interface IDataObserver
    {
        public DictionaryWrapperJson GetGameData();

        public void SetGameData(DictionaryWrapperJson gameData);
    }
}