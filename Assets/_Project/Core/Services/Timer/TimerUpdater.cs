namespace Core.Services
{
    public class TimerUpdater : UpdatableBehaviour
    {
        public override UpdateType UpdateType { get; set; }
        
        private TimerService _service;

        public void Setup(TimerService service, UpdateType type)
        {
            _service = service;
            UpdateType = type;
        }

        public override void GameUpdate(float dt)
        {
            _service.UpdateTimers(dt, UpdateType);
        }
    }
}