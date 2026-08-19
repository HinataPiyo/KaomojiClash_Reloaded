namespace Wall
{

    using UnityEngine;

    public interface IWall
    {
        Vector2 GetWallCenter();
    }

    public class WallController : MonoBehaviour, IWall
    {
        public Vector2 GetWallCenter() => transform.position;
    }

}