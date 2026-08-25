namespace Base
{
    using UnityEngine;
    using UI;

    public interface IAttackable
    {
        void TakeDamage(float amount);
    }

    public abstract class StaminaBase : MonoBehaviour, IAttackable
    {
        protected float currentStamina;
        protected IWorldUI worldUI;

        protected virtual void Awake() { }
        

        protected virtual void Start()
        {
            worldUI = ApiProvider.Get<IWorldUI>();
        }

        /// <summary>
        /// スタミナを減らす
        /// 実装は継承先で行う
        /// </summary>
        public abstract void TakeDamage(float amount);

        public abstract void Die();
    }

}