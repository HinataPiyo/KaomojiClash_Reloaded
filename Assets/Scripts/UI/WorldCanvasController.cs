namespace UI
{
   using UnityEngine;

    public interface IDamageWorldUI
    {
        void SetPlayerToEnemyDamageText(Vector2 position, float damage);
        void SetEnemyToPlayerDamageText(Vector2 position, float damage);

        void ShowClashObjectUI(Vector2 position);
    }

    public interface IWaveStartWorldUI
    {
        void ShowContactObjectUI(Vector2 position);
    }

    public interface IPlayerHereArrow
    {
        void UpdatePlayerHereArrowPosition(Vector2 position);
    }

    public class WorldCanvasController : SceneSingleton<WorldCanvasController>, IDamageWorldUI, IPlayerHereArrow, IWaveStartWorldUI
    {
        [SerializeField] ApplyDamageText playerToEnemyDamageText;
        [SerializeField] ApplyDamageText enemyToPlayerDamageText;
        [SerializeField] PlayerStaminaUI playerStaminaUI;
        [SerializeField] Transform playerHereArrow;
        [SerializeField] GameObject contactObjectUI;
        [SerializeField] GameObject clashObjectUI;

        protected override void OnSingletonAwake()
        {
        }

        public void SetPlayerToEnemyDamageText(Vector2 position, float damage)
        {
            ApplyDamageText text = Instantiate(playerToEnemyDamageText, position, Quaternion.identity, transform);
            text.SetText(damage);
        }

        public void SetEnemyToPlayerDamageText(Vector2 position, float damage)
        {
            ApplyDamageText text = Instantiate(enemyToPlayerDamageText, position, Quaternion.identity, transform);
            text.SetText(damage);
        }

        public void UpdatePlayerHereArrowPosition(Vector2 position)
        {
            playerHereArrow.position = position;
        }

        public void ShowContactObjectUI(Vector2 position)
        {
            Instantiate(contactObjectUI, position, Quaternion.identity, transform);
        }

        public void ShowClashObjectUI(Vector2 position)
        {
            Instantiate(clashObjectUI, position, Quaternion.identity, transform);
        }
    }
}