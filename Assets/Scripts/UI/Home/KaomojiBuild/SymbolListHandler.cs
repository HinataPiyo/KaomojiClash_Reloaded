namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    

    /// <summary>
    /// 記号のリストを管理する
    /// </summary>
    [RequireComponent(typeof(SymbolTypeChangeHandler))]
    public class SymbolListHandler : SceneSingleton<SymbolListHandler>
    {
        [SerializeField] Canvas symbolCollectionCanvas;
        [SerializeField] SymbolIconButton icon_Prefab;
        [SerializeField] Transform content_Transform;
        SymbolTypeChangeHandler typeChangeHandler;
        public event System.Action<SymbolData> OnSelectedSymbolChanged;     // 選択時の通知

        protected override void OnSingletonAwake()
        {
            typeChangeHandler = GetComponent<SymbolTypeChangeHandler>();
            typeChangeHandler.OnSymbolTypeChanged += CreateSymbolList;

            ShowCanvas(false);
        }

        /// <summary>
        /// 表示切替
        /// </summary>
        public void ShowCanvas(bool show)
        {
            symbolCollectionCanvas.enabled = show;
        }

        void Start()
        {
            // イベントの登録が終わってから初期化を行う
            CreateSymbolList(typeChangeHandler.FirstSymbolType());
        }

        protected override void OnSingletonDestroy()
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
                // [不具合修正] ボタン生成時の購読者一覧を固定せず、押下時点の購読者へ通知する。
                icon.Initialize(symbolData, NotifySelectedSymbolChanged);
            }
        }

        /// <summary>
        /// c#の特性上、イベントはnullチェックが必要なため、別途関数を作成して通知する
        /// </summary>
        /// <param name="symbolData"></param>
        void NotifySelectedSymbolChanged(SymbolData symbolData)
        {
            OnSelectedSymbolChanged?.Invoke(symbolData);
        }
    }
}