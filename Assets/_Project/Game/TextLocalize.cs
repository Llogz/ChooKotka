using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game
{
    public class TextLocalize : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        [SerializeField] private string key;
        [SerializeField] private LocalizedStringTable table;

        private void Translate(Locale locale)
        {
            if (table == null) return;
            if (table.GetTable() == null) return;
            if (table.GetTable().GetEntry(key) == null) return;
            text.text = table.GetTable().GetEntry(key).Value;
        }

        private void Start()
        {
            Translate(null);
        }

        private void Awake()
        {
            LocalizationSettings.SelectedLocaleChanged += Translate;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= Translate;
        }
    }
}
