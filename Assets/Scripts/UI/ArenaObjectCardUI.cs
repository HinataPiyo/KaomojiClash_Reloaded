namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ArenaObjectCardUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] Image icon;
        [SerializeField] GameObject newMark;
        Button button;
        ArenaObjectData data;
        

        IArenaObjectEdit arenaObjectEdit;

        void Awake()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(ButtonOnClick);
        }

        void Start()
        {
            arenaObjectEdit = ApiProvider.Get<IArenaObjectEdit>();
        }

        public void SetData(ArenaObjectData data, int level, bool isNew)
        {
            this.data = data;
            this.level.text = $"Level {level}";
            description.text = data.GetDescription(level);
            icon.sprite = data.Icon;
            newMark.SetActive(isNew);
        }

        void ButtonOnClick()
        {
            // ArenaObjectが選択されたときの処理をここに追加
            arenaObjectEdit.EndSelectArenaObject(data);
        }
    }
}