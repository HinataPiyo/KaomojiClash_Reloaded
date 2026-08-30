namespace Player
{
    using Base;
    using UI;
    using UnityEngine;
    using Wave;

    public class Stamina : StaminaBase, IInitializeStats
    {
        ICamera cam;
        IHitStop hitStop;
        IPlayerStaminaUI playerStaminaUI;
        IBattleState battleState;

        PlayerStatsCalculator statsCalc;

        protected override void Awake()
        {
            base.Awake();
            statsCalc = GetComponent<PlayerStatsCalculator>();
        }

        public void InitializeStats()
        {
            currentStamina = statsCalc.GetStamina();
        }

        protected override void Start()
        {
            base.Start();
            cam = ApiProvider.Get<ICamera>();
            hitStop = ApiProvider.Get<IHitStop>();

            playerStaminaUI = ApiProvider.Get<IPlayerStaminaUI>();
            battleState = ApiProvider.Get<IBattleState>();

            playerStaminaUI.UpdateStaminaUI(currentStamina, statsCalc.GetStamina());
        }

        public override void TakeDamage(float amount)
        {
            currentStamina -= amount;
            worldUI.SetEnemyToPlayerDamageText(transform.position, amount);
            playerStaminaUI.UpdateStaminaUI(currentStamina, Mathf.CeilToInt(statsCalc.GetStamina()));
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
            worldUI.ShowClashObjectUI(transform.position);
            hitStop.PlayerDeathHitStopEffect();
            
            battleState.ChangeBattleState(BattleState.WaveFailed);
        }
    }
}