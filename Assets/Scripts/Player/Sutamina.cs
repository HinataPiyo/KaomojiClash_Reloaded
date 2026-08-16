namespace Player
{
    using UnityEngine;
    using Base;

    public class Sutamina : SutaminaBase
    {
        public override void TakeDamage(float amount)
        {
            currentSutamina -= amount;
            if (currentSutamina < 0)
            {
                currentSutamina = 0;
                Die();
            }
        }

        public override void Die()
        {
            Destroy(gameObject);
        }
    }
}