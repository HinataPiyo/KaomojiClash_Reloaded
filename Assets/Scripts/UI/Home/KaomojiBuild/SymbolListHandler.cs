namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SymbolListHandler : MonoBehaviour
    {
        [SerializeField] SymbolIconButton icon_Prefab;
        [SerializeField] Transform content_Transform;

        void Awake()
        {
            SymbolTypeChangeHandler.OnSymbolTypeChanged += CreateSymbolList;
        }

        void OnDestroy()
        {
            SymbolTypeChangeHandler.OnSymbolTypeChanged -= CreateSymbolList;
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
                icon.Initialize(symbolData);
            }
        }
    }
}