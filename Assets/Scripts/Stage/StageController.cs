using UnityEngine;
using Wall;

public interface IStage
{
    Vector2 EncountPosition(int waveCount);
    void CheckCreateWall(Vector2 centerPosition);
    public IWall GetCurrentWall();
}

public class StageController : MonoBehaviour, IStage
{
    [SerializeField] float encountInterval = 5f;        // 敵の出現位置の間隔
    [SerializeField] WallController wall_Prefab;

    WallController currentWall;

    public IWall GetCurrentWall() => currentWall;

    void Awake()
    {
        ApiProvider.Register<IStage>(this);
    }

    /// <summary>
    /// 敵の出現位置を返す
    /// </summary>
    /// <param name="waveCount">現在のウェーブ数</param>
    /// <returns>敵の出現位置</returns>
    public Vector2 EncountPosition(int waveCount)
    {
        float x = encountInterval * waveCount;
        return new Vector2(x, 0f);
    }

    public void CheckCreateWall(Vector2 centerPosition)
    {
        if (currentWall != null)
        {
            currentWall.ActivateWall();
            return;
        }

        currentWall = Instantiate(wall_Prefab, centerPosition, Quaternion.identity);
    }
}