namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public enum PanelName
    { Home, KaomojiBuild }

    public enum NextOrBack
    { Next, Back }
    public class PanelChangeButton : MonoBehaviour
    {
        [System.Serializable]
        public class Entry
        {
            public PanelName panelName;
            public Canvas canvas;
        }

        [SerializeField] NextOrBack thisNextOrBack;

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

            if(thisNextOrBack == NextOrBack.Next)
            {
                ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.ButtonClick_00);
            }
            else if(thisNextOrBack == NextOrBack.Back)
            {
                ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.Cancel_00);
            }
        }
    }
}