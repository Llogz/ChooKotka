using UnityEngine;

namespace Game.Menu
{

    public class Menu : MonoBehaviour
    {
        public string Name => menuName;
        [SerializeField] private string menuName;
        
        public bool IsOpen => isOpen;
        [SerializeField] private bool isOpen;
        
        [SerializeField] private float speed;
        [SerializeField] private Vector3 openPosition;
        [SerializeField] private Vector3 closePosition;

        public void SetState(bool state)
        {
            isOpen = state;
        }

        private void Update()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, IsOpen ? openPosition : closePosition, Time.deltaTime * speed);
        }
    }

}
