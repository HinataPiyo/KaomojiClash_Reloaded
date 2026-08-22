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

        protected override void Start()
        {
            base.Start();
            currentStamina = 10;
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            worldUI.SetPlayerToEnemyDamageText(transform.position, amount);
            if (currentStamina <= 0)
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