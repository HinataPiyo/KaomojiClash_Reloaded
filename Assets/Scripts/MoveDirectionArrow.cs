using TMPro;
using UnityEngine;

public interface IMoveDirectionArrow
{
    void SetArrowDirection(Vector2 direction);
    void UpdateScale(float distance);
    void SetVisible(bool visible);
}

public class MoveDirectionArrow : MonoBehaviour, IMoveDirectionArrow
{
    [SerializeField] TextMeshPro arrow;
    static readonly float maxArrowLength = 3f;
    static readonly float minArrowLength = 0.1f;

    public void SetArrowDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void UpdateScale(float distance)
    {
        float clampedDistance = Mathf.Clamp(distance, minArrowLength, maxArrowLength);
        arrow.transform.localScale = new Vector3(-clampedDistance, 1, 1);
    }

    public void SetVisible(bool visible)
    {
        if (arrow != null)
        {
            arrow.gameObject.SetActive(visible);
        }
    }
}