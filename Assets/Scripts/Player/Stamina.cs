namespace Player
{
    using Base;
    using UI;

    public class Stamina : StaminaBase
    {
        ICamera cam;
        IHitStop hitStop;
        IPlayerStaminaUI playerStaminaUI;

        protected override void Start()
        {
            base.Start();
            cam = ApiProvider.Get<ICamera>();
            hitStop = ApiProvider.Get<IHitStop>();
            playerStaminaUI = ApiProvider.Get<IPlayerStaminaUI>();
            currentStamina = 1000;
            playerStaminaUI.UpdateStaminaUI(currentStamina, 5);
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            worldUI.SetEnemyToPlayerDamageText(transform.position, amount);
            playerStaminaUI.UpdateStaminaUI(currentStamina, 5);
            if (currentStamina <= 0)
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