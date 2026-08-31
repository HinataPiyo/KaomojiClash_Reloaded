namespace UI
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ResultData
    {
        public int waveCount;
        public int killCount;
        public int getMoney;

        public List<ArenaObjectController.ArenaObjectSetData> acquiredArenaObjects;
    }

    public interface IResultUIHandler
    {
        void AddEnemyKillCount();
        void AddWaveCount();
        void AddGetMoney(int money);
        void ShowResult();
    }

    public class ResultUIHandler : MonoBehaviour, IResultUIHandler
    {
        [SerializeField] TextMeshProUGUI waveCountText;
        [SerializeField] TextMeshProUGUI killCountText;
        [SerializeField] TextMeshProUGUI GetMoneyText;
        [SerializeField] Transform acquiredArenaObjectsParent;
        [SerializeField] GameObject acquiredArenaObjectPrefab;

        [SerializeField] Button retryButton;
        [SerializeField] Button homeButton;

        public ResultData resultData = new ResultData();
        [SerializeField] Canvas canvas;

        IArenaObjectSelect arenaObjectSelect;

        public void AddEnemyKillCount() => resultData.killCount++;
        public void AddWaveCount() => resultData.waveCount++;
        public void AddGetMoney(int money) => resultData.getMoney += money;

        void Awake()
        {
            ApiProvider.Register<IResultUIHandler>(this);

            retryButton.onClick.AddListener(() =>
            {
                ApiProvider.Get<ISceneChange>().ChangeScene(SceneName.Battle);
            });

            homeButton.onClick.AddListener(() =>
            {
                ApiProvider.Get<ISceneChange>().ChangeScene(SceneName.Home);
            });

            Wave.WaveController.OnWaitingForNextWave += AddWaveCount;
            Wave.WaveController.OnWaveFailed += ShowResult;
            Wave.WaveController.OnWaveTimeUp += ShowResult;

            canvas.enabled = false;
        }

        void Start()
        {
            arenaObjectSelect = ApiProvider.Get<IArenaObjectSelect>();
        }

        void OnDestroy()
        {
            Wave.WaveController.OnWaitingForNextWave -= AddWaveCount;
            Wave.WaveController.OnWaveFailed -= ShowResult;
            Wave.WaveController.OnWaveTimeUp -= ShowResult;
        }

        public void ShowResult()
        {
            waveCountText.text = $"Wave: {resultData.waveCount}";
            killCountText.text = $"Kill: {resultData.killCount}";
            GetMoneyText.text = $"Get Money: {resultData.getMoney}";
            Clear();
            for(int i = 0; i < arenaObjectSelect.GetArenaObjectSetDatas().Count; i++)
            {
                ResultArenaObjectIcon icon = Instantiate(acquiredArenaObjectPrefab, acquiredArenaObjectsParent).GetComponent<ResultArenaObjectIcon>();
                icon.SetIcon(arenaObjectSelect.GetArenaObjectSetDatas()[i].Data.Icon, arenaObjectSelect.GetArenaObjectSetDatas()[i].Logic.Level);
                
            }
            canvas.enabled = true;
        }

        void Clear()
        {
            resultData.waveCount = 0;
            resultData.killCount = 0;
            resultData.getMoney = 0;

            foreach (Transform child in acquiredArenaObjectsParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}