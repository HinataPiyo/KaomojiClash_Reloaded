namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public enum PanelName
    { Home, KaomojiBuild }
    public class PanelChangeButton : MonoBehaviour
    {
        [System.Serializable]
        public class Entry
        {
            public PanelName panelName;
            public Canvas canvas;
        }

        [SerializeField] Entry thisEntry;
        [SerializeField] Entry targetEntry;

        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnPanelChangeButtonPressed);
        }

        public void OnPanelChangeButtonPressed()
        {
            thisEntry.canvas.enabled = false;
            targetEntry.canvas.enabled = true;
        }
    }
}