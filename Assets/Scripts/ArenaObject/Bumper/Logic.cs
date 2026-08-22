namespace ArenaObject.Bumper
{
    using UnityEngine;
    
    public class Logic : ArenaObjectLogic<Data>
    {
        Animator anim;

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            // 自身ととの位置を取得する
            // 現在の速度
            Rigidbody2D rb = col.rigidbody;
            Vector2 v = rb.linearVelocity;

            // 衝突点の法線（最も信頼度が高い）
            Vector2 n = col.contacts[0].normal;

            // 反射ベクトルを計算
            Vector2 reflected = Vector2.Reflect(v, n);

            // 速度の大きさは維持したまま向きだけ変更
            rb.linearVelocity = reflected.normalized * v.magnitude * Data.GetReflectionPower(Level);

            anim.SetTrigger("Reflect");
        }
    }
}