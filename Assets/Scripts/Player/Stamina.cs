namespace Player
{
    using UnityEngine;
    using Base;

    public class Stamina : StaminaBase
    {
        void Start()
        {
            currentStamina = 1000;
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
        }
    }
}