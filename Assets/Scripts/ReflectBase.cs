using UnityEngine;

public abstract class ReflectBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    const float REFRECT_SPEED_BORDER = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    protected bool CanReflection()
    {
        return rb.linearVelocity.sqrMagnitude >= REFRECT_SPEED_BORDER;
    }

    protected abstract bool CanApplyDamage(Rigidbody2D otherRb);
}
