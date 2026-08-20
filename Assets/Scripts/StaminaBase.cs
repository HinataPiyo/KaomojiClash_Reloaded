namespace Base
{
    using UnityEngine;

    public interface IAttackable
    {
        void TakeDamage(float amount);
    }

    public abstract class StaminaBase : MonoBehaviour, IAttackable
    {
        protected float currentStamina;

        /// <summary>
        /// スタミナを減らす
        /// 実装は継承先で行う
        /// </summary>
        public abstract void TakeDamage(float amount);

        public abstract void Die();
    }

}