namespace UI
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 部位変更のUIを管理するクラス
    /// </summary>
    public class SymbolTypeChangeHandler : SceneSingleton<SymbolTypeChangeHandler>
    {
        [SerializeField] GameObject button_Prefab;
        [SerializeField] Transform buttonParent_Transform;
        [SerializeField] SymbolType defaultSymbolType;

        [Serializable]
        public class CanvasBySymbolType
        {
            public SymbolType Type;
            public Canvas canvas;
        }

        public event Action<SymbolType> OnSymbolTypeChanged;
        [SerializeField] CanvasBySymbolType[] canvasByType;

        void Start()
        {
            CreateSymbolTypeButtons(); // 初期状態としてボタンを生成
            OnSymbolTypeChanged += (type) =>
            {
                AudioManager.Instance.PlaySE(SEAudioName.ButtonClick_00);
                foreach (var c in canvasByType)
                {
                    // 単純な比較: Canvas に登録された Type が current type に含まれているか
                    c.canvas.enabled = (type & c.Type) != 0;  // 1つでもビットが一致すれば表示
                }
            };
        }

        /// <summary>
        /// デフォルトの記号タイプに基づいて、最初の有効な記号タイプを取得します。
        /// </summary>
        /// <returns>最初の有効な記号タイプ</returns>
        public SymbolType FirstSymbolType()
        {
            foreach (SymbolType type in Enum.GetValues(typeof(SymbolType)))
            {
                if ((type & defaultSymbolType) != 0)
                {
                    return type;
                }
            }
            return default;
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

            foreach (SymbolType type in Enum.GetValues(typeof(SymbolType)))
            {
                if ((type & defaultSymbolType) != 0)
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