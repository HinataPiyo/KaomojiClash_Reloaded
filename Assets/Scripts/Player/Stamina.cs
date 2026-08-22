namespace Player
{
    using Base;

    public class Stamina : StaminaBase
    {
        protected override void Start()
        {
            base.Start();
            currentStamina = 1000;
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            worldUI.SetEnemyToPlayerDamageText(transform.position, amount);
            if (currentStamina < 0)
            {
                currentStamina = 0;
                Die();
            }
        }

        public override void Die()
        {
            Destroy(gameObject);
        }
    }
}