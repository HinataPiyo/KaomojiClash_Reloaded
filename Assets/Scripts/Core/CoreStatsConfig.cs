using UnityEngine;

public class CoreStatsConfig : ScriptableObject
{
    [Header("コアステータス")]
    [field: SerializeField] public float Default_Stamina = 10f;
    [field: SerializeField] public float Default_Speed = 5f;
    [field: SerializeField] public float Default_Power = 1f;
    [field: SerializeField] public float Default_Defense = 1f;
}