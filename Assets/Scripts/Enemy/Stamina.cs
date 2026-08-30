namespace Enemy
{
    using Base;

    public interface IEnemyStamina
    {
        public event System.Action OnEnemyDeathEvent;
    }
    
    public class Stamina : StaminaBase, IEnemyStamina, IInitializeStats
    {
        public event System.Action OnEnemyDeathEvent;

        EnemyStatsCalculator statsCalc;

        protected override void Awake()
        {
            base.Awake();
            statsCalc = GetComponent<EnemyStatsCalculator>();
        }

        public void InitializeStats()
        {
            currentStamina = statsCalc.GetStamina();
        }

        protected override void Start()
        {
            base.Start();
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
            worldUI.ShowClashObjectUI(transform.position);
            OnEnemyDeathEvent?.Invoke();
        }
    }
}