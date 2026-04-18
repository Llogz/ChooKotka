using System.Collections.Generic;
using VContainer;

namespace Core.Services
{
    public interface IInputController
    {
        InputSystem GetInputSystem();
    }

    public class InputControllerService : IInputController
    {
        [Inject] private SceneChangerService _sceneChangerService;
        
        private readonly List<InputSystem> _curInputs = new();
        
        public InputSystem GetInputSystem()
        {
            InputSystem input = new InputSystem();
            input.Enable();
            _curInputs.Add(input);
            _sceneChangerService.OnChangeCheck(OnSceneSwitch);
        
            return input;
        }

        private void OnSceneSwitch()
        {
            foreach (InputSystem i in _curInputs)
                i.Disable();
            _curInputs.Clear();
        }
    }

}