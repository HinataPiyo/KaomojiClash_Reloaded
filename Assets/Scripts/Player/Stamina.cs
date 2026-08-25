namespace Player
{
    using Base;
    using UI;

    public class Stamina : StaminaBase
    {
        ICamera cam;
        IHitStop hitStop;
        IPlayerStaminaUI playerStaminaUI;

        PlayerStatsCalculator statsCalc;

        protected override void Awake()
        {
            base.Awake();
            statsCalc = GetComponent<PlayerStatsCalculator>();
        }

        protected override void Start()
        {
            base.Start();
            cam = ApiProvider.Get<ICamera>();
            hitStop = ApiProvider.Get<IHitStop>();
            playerStaminaUI = ApiProvider.Get<IPlayerStaminaUI>();

            currentStamina = statsCalc.GetStamina();
            playerStaminaUI.UpdateStaminaUI(currentStamina, statsCalc.GetStamina());
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            worldUI.SetEnemyToPlayerDamageText(transform.position, amount);
            playerStaminaUI.UpdateStaminaUI(currentStamina, statsCalc.GetStamina());
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