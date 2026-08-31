namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ArenaObjectCardUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI objectName;
        [SerializeField] Image icon;
        [SerializeField] GameObject newMark;
        Button button;
        ArenaObjectData data;

        IArenaObjectSelect arenaObjectSelect;

        void Awake()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(ButtonOnClick);
        }

        void Start()
        {
            arenaObjectSelect = ApiProvider.Get<IArenaObjectSelect>();
        }

        public void SetData(ArenaObjectData data, int level, bool isNew)
        {
            this.data = data;
            this.level.text = $"Level {level}";
            this.objectName.text = data.ObjectName;
            description.text = data.GetDescription(level);
            icon.sprite = data.Icon;
            newMark.SetActive(isNew);
        }

        void ButtonOnClick()
        {
            // ArenaObjectが選択されたときの処理をここに追加
            arenaObjectSelect.EndSelectArenaObject(data);
        }
    }
}