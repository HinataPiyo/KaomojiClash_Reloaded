namespace ArenaObject.Bumper
{
    using UnityEngine;
    
    public class Logic : ArenaObjectLogic<Data>
    {
        Animator anim;

        void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            // 自身ととの位置を取得する
            // 現在の速度
            Rigidbody2D rb = col.rigidbody;
            Vector2 direction = (col.transform.position - transform.position).normalized;

            //! PlayerのSpeedを暫定として入れる
            rb.AddForce(direction * 5 * Data.GetReflectionPower(Level), ForceMode2D.Impulse); // 反射ベクトルを加える

            anim.SetTrigger("Reflect");
        }
    }
}