using Core.Services;
using UnityEngine;
using VContainer;

namespace Game.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Menu[] menus;

        public void Open(string menuName)
        {
            foreach (var t in menus)
            {
                if (t.Name == menuName)
                {
                    t.SetState(true);
                }
                else if (t.IsOpen)
                {
                    t.SetState(false);
                }
            }
        }
    }
}
