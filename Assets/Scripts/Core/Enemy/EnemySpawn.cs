namespace Enemy
{
    using UnityEngine;
    using Wall;

    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] GameObject enemyPrefab;

        IWall wall;

        void Start()
        {
            wall = ApiProvider.Get<IWall>();
        }

        public void SpawnEnemy()
        {
            Vector2 spawnPosition = wall.GetRandomPositionWithinWall();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            
        }
    }
}