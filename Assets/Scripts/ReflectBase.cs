using UnityEngine;
using Wall;

public abstract class ReflectBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected IStage stage;
    IHitStop hitStop;
    ICamera cam;
    const float REFRECT_SPEED_BORDER = 1.5f;        // 反射可能な速度の閾値
    [SerializeField] protected float speedThreshold = 0.92f;        // ダメージを与えるための速度の閾値（相手より自分のほうが速い場合のみダメージを与える）

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        stage = ApiProvider.Get<IStage>();
        hitStop = ApiProvider.Get<IHitStop>();
        cam = ApiProvider.Get<ICamera>();
    }

    protected abstract void OnCollisionEnter2D(Collision2D col);
    protected void Reflection(Collision2D col)
    {
        // 現在の速度
        Vector2 v = rb.linearVelocity;

        // 衝突点の法線（最も信頼度が高い）
        Vector2 n = col.contacts[0].normal;

        // 反射ベクトルを計算
        Vector2 reflected = Vector2.Reflect(v, n);

        // 速度の大きさは維持したまま向きだけ変更
        rb.linearVelocity = reflected.normalized * v.magnitude ;

        // エフェクトを生成（反射方向に合わせて回転）
        Vector2 dir = reflected.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        hitStop.HitStopEffect();
        cam.SetCameraState(CameraState.Reflect);
        cam.ShakeCamera(1f, 0.2f, 0.1f);
    }

    protected bool CanReflection()
    {
        return rb.linearVelocity.sqrMagnitude >= REFRECT_SPEED_BORDER;
    }

    protected abstract bool CanApplyDamage(Rigidbody2D otherRb);

    /// <summary>
    /// 壁に衝突した場合の反射処理
    /// 速度に関係なく壁の中心に向かって反射する
    /// </summary>
    protected void WallReflection(float userSpeed, float reflectionPower)
    {
        // 壁の中心に向かって反射する
        IWall wall = stage.GetCurrentWall();
        rb.AddForce((wall.GetWallCenter() - (Vector2)transform.position).normalized * userSpeed * reflectionPower, ForceMode2D.Impulse);
    }

    protected abstract float ApplyDamageCalculation();
}
