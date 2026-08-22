namespace UI
{
   using UnityEngine;

    public interface IWorldUI
    {
        void SetPlayerToEnemyDamageText(Vector2 position, float damage);
        void SetEnemyToPlayerDamageText(Vector2 position, float damage);
    }

    public interface IPlayerHereArrow
    {
        void UpdatePlayerHereArrowPosition(Vector2 position);
    }

    public class WorldCanvasController : MonoBehaviour, IWorldUI, IPlayerHereArrow
    {
        [SerializeField] ApplyDamageText playerToEnemyDamageText;
        [SerializeField] ApplyDamageText enemyToPlayerDamageText;
        [SerializeField] PlayerStaminaUI playerStaminaUI;
        [SerializeField] Transform playerHereArrow;

        void Awake()
        {
            ApiProvider.Register<IWorldUI>(this);
            ApiProvider.Register<IPlayerHereArrow>(this);
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
    }

}