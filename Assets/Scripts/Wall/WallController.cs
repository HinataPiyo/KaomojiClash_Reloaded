namespace Wall
{

    using UnityEngine;

    public interface IWall
    {
        Transform GetWallTransform();
        Vector2 GetWallRange();
        Vector2 GetRandomPositionWithinWall();
        void InactivateWall();
        void ActivateWall();
    }

    public class WallController : MonoBehaviour, IWall
    {
        [SerializeField] Vector2 wallRange;
        public Transform GetWallTransform() => transform;
        public Vector2 GetWallRange() => wallRange;
        public Vector2 GetRandomPositionWithinWall()
        {
            float randomX = Random.Range(-wallRange.x / 2, wallRange.x / 2);
            float randomY = Random.Range(-wallRange.y / 2, wallRange.y / 2);
            return (Vector2)transform.position + new Vector2(randomX, randomY);
        }

        public void InactivateWall() => gameObject.SetActive(false);
        public void ActivateWall() => gameObject.SetActive(true);

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, wallRange);
        }
    }

}