namespace Core.Services
{
    public abstract class FabricObject<TData> : UpdatableBehaviour
        where TData : FabricObjectData
    {
        public void Initialize(TData data)
        {
            UpdateType = data.UpdateType;
            OnInitialize(data);
        }

        protected abstract void OnInitialize(TData data);
    }

}