using System.Collections.Generic;
using UnityEngine;

public interface IArenaObjectEdit
{
    void CreateArenaObjects(ArenaObjectData data);
}

public class ArenaObjectController : MonoBehaviour, IArenaObjectEdit
{
    [SerializeField] List<ArenaObjectSetData> arenaObjectSetDatas = new List<ArenaObjectSetData>();

    IStage stage;

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
    }

    public void CreateArenaObjects(ArenaObjectData data)
    {
        GameObject arenaObject = Instantiate(data.EffectPrefab, stage.GetCurrentWall().GetWallTransform());
        ArenaObjectLogic logic = arenaObject.GetComponent<ArenaObjectLogic>();
        arenaObjectSetDatas.Add(new ArenaObjectSetData(data, logic));
    }

}