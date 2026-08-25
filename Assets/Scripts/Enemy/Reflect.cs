namespace Enemy
{
    using Base;
    using UnityEngine;

    public class Reflect : ReflectBase
    {
        EnemyStatsCalculator statsCalc;

        protected override void Awake()
        {
            base.Awake();
            statsCalc = GetComponent<EnemyStatsCalculator>();
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            // プレイヤーと衝突した
            if (col.collider.CompareTag("Player"))
            {
                if (!CanReflection())       // 反射できない状態なら何もしない
                {
                    Debug.Log("速度が小さいため反射しません");
                    return;
                }

                Reflection(col);        // 反射

                Rigidbody2D otherRb = col.collider.GetComponent<Rigidbody2D>();

                // 相手より自分のほうが速い場合のみダメージを与える
                if (CanApplyDamage(otherRb))
                {
                    IAttackable attackable = col.collider.GetComponent<IAttackable>();
                    attackable?.TakeDamage(statsCalc.ApplyDamageCalculation());
                }
            }
        }

        protected override bool CanApplyDamage(Rigidbody2D otherRb)
        {
            // 相手より自分のほうが速い場合のみダメージを与える
            return otherRb != null && rb.linearVelocity.sqrMagnitude > otherRb.linearVelocity.sqrMagnitude * speedThreshold;
        }
    }
}