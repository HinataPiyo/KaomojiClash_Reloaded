using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public interface IArenaObjectEdit
{
    void CreateArenaObjects(ArenaObjectData data);
    IEnumerator ArenaObjectSelectRoutine(int playerLevelUpCount);
    void EndSelectArenaObject(ArenaObjectData data);
}

public class ArenaObjectController : MonoBehaviour, IArenaObjectEdit
{
    const int MAX_ARENAOBJECT_SELECTION_COUNT = 3;
    [SerializeField] List<ArenaObjectSetData> arenaObjectSetDatas = new List<ArenaObjectSetData>();
    bool isEndSelectArenaObject = false;

    IStage stage;
    IArenaObjectSelectUI arenaObjectSelectUI;

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

    void Awake()
    {
        ApiProvider.Register<IArenaObjectEdit>(this);
    }

    void Start()
    {
        stage = ApiProvider.Get<IStage>();
        arenaObjectSelectUI = ApiProvider.Get<IArenaObjectSelectUI>();
    }

    // ! 次回、ArenaObjectをランダムに抽選し、UIに表示　ボタン選択後ArenaObjectを生成、ArenaObjectEditモードに遷移

    public IEnumerator ArenaObjectSelectRoutine(int playerLevelUpCount)
    {
        for (int i = 0; i < playerLevelUpCount; i++)
        {
            isEndSelectArenaObject = false;
            ShowArenaObjectSelectUI();

            // ここでDataが選択されるまで待つ処理を入れる
            yield return new WaitUntil(() => isEndSelectArenaObject);
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
    /// ArenaObjectをランダムに抽選し、UIに表示するためのデータを取得
    /// </summary>
    /// <returns></returns>
    public List<ArenaObjectSelectUI.Entry> GetRandomDataSelect()
    {
        List<ArenaObjectSelectUI.Entry> entries = new List<ArenaObjectSelectUI.Entry>();
        List<ArenaObjectSetData> tempList = new List<ArenaObjectSetData>(arenaObjectSetDatas);
        for (int i = 0; i < MAX_ARENAOBJECT_SELECTION_COUNT && tempList.Count > 0; i++)
        {
            int index = Random.Range(0, tempList.Count);
            ArenaObjectSetData selectedData = tempList[index];
            tempList.RemoveAt(index);
            entries.Add(new ArenaObjectSelectUI.Entry(selectedData.Data, selectedData.Logic.Level));
        }
        return entries;
    }

    public void EndSelectArenaObject(ArenaObjectData data)
    {
        CreateArenaObjects(data);
        arenaObjectSelectUI.Hide();
        isEndSelectArenaObject = true;
    }

    public void CreateArenaObjects(ArenaObjectData data)
    {
        GameObject arenaObject = Instantiate(data.EffectPrefab, stage.GetCurrentWall().GetWallTransform());
        ArenaObjectLogic logic = arenaObject.GetComponent<ArenaObjectLogic>();
        arenaObjectSetDatas.Add(new ArenaObjectSetData(data, logic));
    }

}