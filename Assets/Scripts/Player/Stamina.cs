namespace Player
{
    using Base;

    public class Stamina : StaminaBase
    {
        ICamera cam;
        IHitStop hitStop;

        protected override void Start()
        {
            base.Start();
            cam = ApiProvider.Get<ICamera>();
            hitStop = ApiProvider.Get<IHitStop>();
            currentStamina = 5;
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
            cam.SetCameraState(CameraState.PlayerDeath);
            hitStop.PlayerDeathHitStopEffect();
        }
    }
}