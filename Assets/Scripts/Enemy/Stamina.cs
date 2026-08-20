namespace Enemy
{
    using Base;

    public interface IEnemyStamina
    {
        public event System.Action OnEnemyDeathEvent;
    }
    
    public class Stamina : StaminaBase, IEnemyStamina
    {
        public event System.Action OnEnemyDeathEvent;
        void Awake()
        {
            currentStamina = 10;
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            if (currentStamina < 0)
            {
                currentStamina = 0;
                Die();
            }
        }

        public override void Die()
        {
            Destroy(gameObject);
            OnEnemyDeathEvent?.Invoke();
        }
    }
}