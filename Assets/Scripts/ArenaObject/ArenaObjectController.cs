using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public interface IArenaObjectSelect
{
    void CreateArenaObjects(ArenaObjectData data);
    IEnumerator ArenaObjectSelectRoutine(int playerLevelUpCount);
    List<ArenaObjectController.ArenaObjectSetData> GetArenaObjectSetDatas();
    void EndSelectArenaObject(ArenaObjectData data);
    void SetEditDone(bool done);
}

public class ArenaObjectController : MonoBehaviour, IArenaObjectSelect
{
    const int MAX_ARENAOBJECT_SELECTION_COUNT = 3;
    [SerializeField] List<ArenaObjectSetData> arenaObjectSetDatas = new List<ArenaObjectSetData>();
    bool isEndSelectArenaObject = false;
    bool isEditDone = false;        // ArenaObjectの編集が完了したかどうかを示すフラグ
    bool isNewSpawned = false;      // 新しく配置するオブジェクトが生成されたかどうか

    IStage stage;
    IArenaObjectSelectUI arenaObjectSelectUI;
    IArenaObjectEditModeUI arenaObjectEditModeUI;
    IResultUIHandler resultUIHandler;

    [System.Serializable]
    public class ArenaObjectSetData
    {
        [SerializeField] ArenaObjectData data;

        public ArenaObjectData Data => data;
        public ArenaObjectLogic Logic { get; private set; }

        public ArenaObjectSetData(ArenaObjectData data, ArenaObjectLogic logic)
        {
            this.data = data;
            Logic = logic;
        }
    }

    public List<ArenaObjectSetData> GetArenaObjectSetDatas() => arenaObjectSetDatas;

    void Awake()
    {
        ApiProvider.Register<IArenaObjectSelect>(this);
    }

    void Start()
    {
        stage = ApiProvider.Get<IStage>();
        arenaObjectSelectUI = ApiProvider.Get<IArenaObjectSelectUI>();
        arenaObjectEditModeUI = ApiProvider.Get<IArenaObjectEditModeUI>();
        resultUIHandler = ApiProvider.Get<IResultUIHandler>();
    }

    /// <summary>
    /// プレイヤーのレベルアップ回数分、ArenaObjectを選択する
    /// </summary>
    public IEnumerator ArenaObjectSelectRoutine(int playerLevelUpCount)
    {
        for (int i = 0; i < playerLevelUpCount; i++)
        {
            isEndSelectArenaObject = false;
            isEditDone = false;
            isNewSpawned = false;
            ShowArenaObjectSelectUI();

            // ここでDataが選択されるまで待つ処理を入れる
            yield return new WaitUntil(() => isEndSelectArenaObject);

            arenaObjectEditModeUI.Show();

            // ArenaObjectの編集が完了するまで待つ
            yield return new WaitUntil(() => isEditDone);
        }

        if(playerLevelUpCount == 0)
        {
            isEndSelectArenaObject = false;
            isEditDone = false;
            isNewSpawned = false;
            
            arenaObjectEditModeUI.Show();

            // ArenaObjectの編集が完了するまで待つ
            yield return new WaitUntil(() => isEditDone);
        }

        yield return null;
    }

    /// <summary>
    /// ArenaObjectSelectUIを表示する
    /// </summary>
    public void ShowArenaObjectSelectUI()
    {
        List<ArenaObjectSelectUI.Entry> selectList = GetRandomDataSelect();
        arenaObjectSelectUI.SetData(selectList);
    }

    /// <summary>
    /// ArenaObjectをランダムに抽選し、UIに表示するためのデータを取得（重複排除）
    /// </summary>
    /// <returns></returns>
    public List<ArenaObjectSelectUI.Entry> GetRandomDataSelect()
    {
        List<ArenaObjectSelectUI.Entry> entries = new List<ArenaObjectSelectUI.Entry>();

        // ArenaObjectDataCollectionが存在しない場合は空のリストを返す
        if (ArenaObjectDataCollection.collections == null || ArenaObjectDataCollection.collections.Length == 0) return entries;

        // 重複なしでランダムに選ぶために、利用可能なコレクションのリストを作成 (最大レベルに達しているものは除外)
        List<ArenaObjectData> availablePool = new List<ArenaObjectData>();
        foreach (var collectionData in ArenaObjectDataCollection.collections)
        {
            var existingSetData = arenaObjectSetDatas.Find(setData => setData.Data == collectionData && setData.Logic != null);
            if (existingSetData != null && existingSetData.Logic.Level >= ArenaObjectData.MaxLevel) continue;
            availablePool.Add(collectionData);
        }

        // 選択する数は、利用可能なプールの数と最大選択数の小さい方にする
        int selectionCount = Mathf.Min(MAX_ARENAOBJECT_SELECTION_COUNT, availablePool.Count);

        for (int i = 0; i < selectionCount; i++)
        {
            // 残りのプールからランダムに1つ選択
            int randomIndex = Random.Range(0, availablePool.Count);
            ArenaObjectData selectedData = availablePool[randomIndex];
            availablePool.RemoveAt(randomIndex); // 重複を防ぐためにプールから削除

            // 既にプレイヤーが所持しているかどうかを確認
            var existingSetData = arenaObjectSetDatas.Find(setData => setData.Data == selectedData && setData.Logic != null);
            ArenaObjectLogic existingLogic = existingSetData?.Logic;

            // 既に存在するArenaObjectのLogicがある場合は次のレベルを、ない場合は1を渡す
            entries.Add(new ArenaObjectSelectUI.Entry(selectedData, existingLogic != null ? existingLogic.Level + 1 : 1));
        }

        return entries;
    }

    /// <summary>
    /// ArenaObjectが選択されたときの処理
    /// </summary>
    /// <param name="data">UIで選択されたArenaObjectのデータ</param>
    public void EndSelectArenaObject(ArenaObjectData data)
    {
        var existingSetData = arenaObjectSetDatas.Find(setData => setData.Data == data && setData.Logic != null);
        if (existingSetData != null)
        {
            // 既に存在するArenaObjectのLogicがある場合はそのレベルを上げる
            existingSetData.Logic.IncrimentLevel();
            isNewSpawned = false;
        }
        else
        {
            // 新しく生成する
            CreateArenaObjects(data);
            isNewSpawned = true;
        }

        arenaObjectSelectUI.Hide();
        isEndSelectArenaObject = true;
    }

    public void SetEditDone(bool done)
    {
        isEditDone = done;
    }

    /// <summary>
    /// ArenaObjectを生成する
    /// </summary>
    /// <param name="data">生成するArenaObjectのデータ</param>
    public void CreateArenaObjects(ArenaObjectData data)
    {
        GameObject arenaObject = Instantiate(data.EffectPrefab, stage.GetCurrentWall().GetWallTransform());
        ArenaObjectLogic logic = arenaObject.GetComponent<ArenaObjectLogic>();
        arenaObjectSetDatas.Add(new ArenaObjectSetData(data, logic));
    }

}