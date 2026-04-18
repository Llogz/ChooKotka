using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu
{
    public class SettingView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Slider slider;
        [SerializeField] private Toggle toggle;
        
        public TextMeshProUGUI Title => title;
        public Slider Slider => slider;
        public Toggle Toggle => toggle;
    }
}
