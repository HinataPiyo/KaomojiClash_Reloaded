namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 部位変更のUIを管理するクラス
    /// </summary>
    public class SymbolTypeChangeHandler : MonoBehaviour
    {
        [SerializeField] GameObject button_Prefab;
        [SerializeField] Transform buttonParent_Transform;

        public static event System.Action<SymbolType> OnSymbolTypeChanged;

        void Awake()
        {
            CreateSymbolTypeButtons();
        }

        void Start()
        {
            CreateSymbolTypeButton(SymbolType.Mouth); // 初期状態としてMouthを選択
            OnSymbolTypeChanged += (type) => ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.ButtonClick_00);
        }

        /// <summary>
        /// SymbolTypeのボタンを生成する
        /// </summary>
        public void CreateSymbolTypeButtons()
        {
            foreach (Transform child in buttonParent_Transform)
            {
                Destroy(child.gameObject);
            }

            foreach (SymbolType type in System.Enum.GetValues(typeof(SymbolType)))
            {
                if (type == SymbolType.MAX) continue; // MAXは除外

                CreateSymbolTypeButton(type);
            }
        }

        void CreateSymbolTypeButton(SymbolType type)
        {
            GameObject buttonObj = Instantiate(button_Prefab, buttonParent_Transform);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = type.ToString();
            button.onClick.AddListener(() => OnSymbolTypeChanged?.Invoke(type));
        }
    }
}