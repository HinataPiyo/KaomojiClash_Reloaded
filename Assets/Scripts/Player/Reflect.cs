namespace Player
{
    using Base;
    using UnityEngine;

    public class Reflect : ReflectBase
    {
        protected override void OnCollisionEnter2D(Collision2D col)
        {
            // 敵と衝突した
            if (col.collider.CompareTag("Enemy"))
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
                    attackable?.TakeDamage(1f);
                }
            }
            // else if (col.collider.CompareTag("Wall"))
            // {
            //     ApplaySkillTagEffects();     // 反射に関連するスキルタグの効果を適用
            // }
        }

        protected override bool CanApplyDamage(Rigidbody2D otherRb)
        {
            // 相手より自分のほうが速い場合のみダメージを与える
            return otherRb != null && rb.linearVelocity.sqrMagnitude > otherRb.linearVelocity.sqrMagnitude * 0.92f;
        }
    }
}