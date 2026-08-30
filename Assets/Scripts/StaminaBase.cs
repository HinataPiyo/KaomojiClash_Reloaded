namespace Base
{
    using UnityEngine;
    using UI;

    public interface IAttackable
    {
        void TakeDamage(float amount);
    }

    public interface IInvincible
    {
        void ChangeInvincible(bool value);
        bool IsInvincible { get; }
    }

    public abstract class StaminaBase : MonoBehaviour, IAttackable, IInvincible
    {
        public bool IsInvincible { get; private set; } = false;
        private int originalLayer;

        protected virtual void Awake() 
        {
            originalLayer = gameObject.layer;
        }

        public void ChangeInvincible(bool value) 
        {
            int invincibleLayer = LayerMask.NameToLayer("Invincible");
            if (invincibleLayer == -1)
            {
                Debug.LogWarning("Invincible layer is not defined in TagManager. Using original layer.");
                invincibleLayer = originalLayer;
            }

            gameObject.layer = value ? invincibleLayer : originalLayer;
            Debug.Log($"<color=yellow>ChangeInvincible: {gameObject.name} is now {(value ? "Invincible" : "Vulnerable")}</color>");
            IsInvincible = value;
        }
        protected float currentStamina;
        protected IDamageWorldUI worldUI;
        

        protected virtual void Start()
        {
            worldUI = ApiProvider.Get<IDamageWorldUI>();
        }

        /// <summary>
        /// スタミナを減らす
        /// 実装は継承先で行う
        /// </summary>
        public abstract void TakeDamage(float amount);

        public abstract void Die();
    }

}