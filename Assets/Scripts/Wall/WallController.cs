namespace Wall
{

    using UnityEngine;

    public interface IWall
    {
        Vector2 GetWallCenter();
        Vector2 GetWallRange();
        Vector2 GetRandomPositionWithinWall();
    }

    public class WallController : MonoBehaviour, IWall
    {
        [SerializeField] Vector2 wallRange;
        public Vector2 GetWallCenter() => transform.position;
        public Vector2 GetWallRange() => wallRange;
        public Vector2 GetRandomPositionWithinWall()
        {
            float randomX = Random.Range(-wallRange.x / 2, wallRange.x / 2);
            float randomY = Random.Range(-wallRange.y / 2, wallRange.y / 2);
            return (Vector2)transform.position + new Vector2(randomX, randomY);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, wallRange);
        }
    }

}