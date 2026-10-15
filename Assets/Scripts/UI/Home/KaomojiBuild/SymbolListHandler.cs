namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [RequireComponent(typeof(SymbolTypeChangeHandler))]
    public class SymbolListHandler : MonoBehaviour
    {
        [SerializeField] SymbolIconButton icon_Prefab;
        [SerializeField] Transform content_Transform;
        SymbolTypeChangeHandler typeChangeHandler;
        public event System.Action<SymbolData> OnSelectedSymbolChanged;

        void Awake()
        {
            typeChangeHandler = GetComponent<SymbolTypeChangeHandler>();
            typeChangeHandler.OnSymbolTypeChanged += CreateSymbolList;
        }

        void Start()
        {
            // イベントの登録が終わってから初期化を行う
            CreateSymbolList(typeChangeHandler.FirstSymbolType());
        }

        void OnDestroy()
        {
            typeChangeHandler.OnSymbolTypeChanged -= CreateSymbolList;
        }

        /// <summary>
        /// 指定されたSymbolTypeに基づいて、SymbolDataのリストを生成し、アイコンを表示する
        /// </summary>
        public void CreateSymbolList(SymbolType type)
        {
            foreach (Transform child in content_Transform)
            {
                Destroy(child.gameObject);
            }

            // TypeごとのSymbolDataを取得して、アイコンを生成する
            if(!SymbolDataCollection.collections.ContainsKey(type)) return;
            List<SymbolData> symbolDataList = SymbolDataCollection.collections[type];
            foreach (SymbolData symbolData in symbolDataList)
            {
                SymbolIconButton icon = Instantiate(icon_Prefab, content_Transform);
                icon.Initialize(symbolData, HandleSymbolSelected);
            }
        }

        void HandleSymbolSelected(SymbolData symbolData)
        {
            OnSelectedSymbolChanged?.Invoke(symbolData);
        }
    }
}