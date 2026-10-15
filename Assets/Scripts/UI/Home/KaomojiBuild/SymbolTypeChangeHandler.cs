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
        [SerializeField] SymbolType defaultSymbolType;

        public static event System.Action<SymbolType> OnSymbolTypeChanged;

        void Start()
        {
            CreateSymbolTypeButtons(); // 初期状態としてボタンを生成
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
                if((type & defaultSymbolType) != 0)
                {
                    CreateSymbolTypeButton(type);
                }
            }
        }

        void CreateSymbolTypeButton(SymbolType type)
        {
            GameObject buttonObj = Instantiate(button_Prefab, buttonParent_Transform);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = type.ToString();
            button.onClick.AddListener(() => OnSymbolTypeChanged?.Invoke(type));
            Debug.Log($"Created button for SymbolType: {type}");
        }
    }
}