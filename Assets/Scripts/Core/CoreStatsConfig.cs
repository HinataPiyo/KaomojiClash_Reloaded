using UnityEngine;

public class CoreStatsConfig : ScriptableObject
{
    [Header("コアステータス")]
    [field: SerializeField] public float Default_Stamina = 10f;
    [field: SerializeField] public float Default_Speed = 5f;
    [field: SerializeField] public float Default_Power = 1f;
    [field: SerializeField] public float Default_Guard = 1f;

    [Header("共通移動関連")]
    [field: SerializeField] public float MaxDraggingDistance = 3f;
}