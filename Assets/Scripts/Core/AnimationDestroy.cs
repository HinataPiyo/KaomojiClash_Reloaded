using UnityEngine;

public class AnimationDestroy : MonoBehaviour
{
    [SerializeField] GameObject parent;

    public void DestroyParent()
    {
        Destroy(parent);
    }
}