namespace Enemy
{
    using Base;
    using UnityEngine;
    
    public class Sutamina : SutaminaBase
    {
        void Awake()
        {
            currentSutamina = 10;
        }

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