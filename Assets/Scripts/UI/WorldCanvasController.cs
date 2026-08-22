namespace UI
{
   using UnityEngine;

    public interface IWorldUI
    {
        void SetPlayerToEnemyDamageText(Vector2 position, float damage);
        void SetEnemyToPlayerDamageText(Vector2 position, float damage);
    }

    public class WorldCanvasController : MonoBehaviour, IWorldUI
    {
        [SerializeField] ApplyDamageText playerToEnemyDamageText;
        [SerializeField] ApplyDamageText enemyToPlayerDamageText;

        void Awake()
        {
            ApiProvider.Register<IWorldUI>(this);
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
    }

}