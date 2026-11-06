namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public enum PanelName
    { Home, KaomojiBuild, CoreStats_Upgrade, Skill_Set, Critical_Upgrade }

    public enum NextOrBack
    { Next, Back }
    public class PanelChangeButton : MonoBehaviour
    {
        public static PanelName CurrentPanel;
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
            CurrentPanel = targetEntry.panelName;

            if(thisNextOrBack == NextOrBack.Next)
            {
                AudioManager.Instance.PlaySE(SEAudioName.ButtonClick_00);
            }
            else if(thisNextOrBack == NextOrBack.Back)
            {
                AudioManager.Instance.PlaySE(SEAudioName.Cancel_00);
            }

            SymbolListHandler.Instance.ShowCanvas(IsShowSymbolCollection());
        }

        bool IsShowSymbolCollection()
        {
            return targetEntry.panelName == PanelName.KaomojiBuild
            || targetEntry.panelName == PanelName.CoreStats_Upgrade
            || targetEntry.panelName == PanelName.Skill_Set;
        }
    }
}