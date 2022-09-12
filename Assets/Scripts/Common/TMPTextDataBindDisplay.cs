using AxGrid.Base;
using TMPro;
using UnityEngine;

namespace AxGrid.Tools.Binders
{

    /// <summary>
    /// Хелпер привязки полей Display для отображения надписей
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTextDataBindDisplay : MonoBehaviourExt
    {
        [Header("Форматироване")]
        [Tooltip("Smart.Format(format, model)")]
        public string format = "{Display.InSlot.Button.}";

        private TextMeshProUGUI uiText;

        [OnAwake]
        void SetUIText()
        {
            uiText = GetComponent<TextMeshProUGUI>();
        }

        [OnStart]
        public void Bind()
        {
            Settings.Model.EventManager.AddAction("OnDisplayChanged", Changed);
            Settings.Model.EventManager.AddAction("OnDisplayChanged", Changed);
            Changed();
        }

        [OnDestroy]
        public void UnBind()
        {
            Settings.Model.EventManager.RemoveAction("OnDisplayChanged", Changed);
            Settings.Model.EventManager.RemoveAction("OnDisplayChanged", Changed);
        }

        protected void Changed()
        {
            string s = Text.Text.Get(format, Settings.Model);
            if (!string.IsNullOrEmpty(s))
                uiText.text = s;
        }

        public void SetKey(string key)
        {
            SetUIText();
            format = key;
            uiText.text = key;
            Changed();
        }
    }
}