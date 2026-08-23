
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaObjectData", menuName = "KaomojiClash_Reloaded/ArenaObjectData")]
public abstract class ArenaObjectData : ScriptableObject
{
    public const int MaxLevel = 5;          // ５回強化が可能
    [SerializeField] string objectName;
    [SerializeField] Sprite icon;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] Vector2 size;

    public string ObjectName => objectName;
    public Sprite Icon => icon;
    public GameObject EffectPrefab => effectPrefab;
    public Vector2 Size => size;

    public abstract string GetDescription(int level);
}

public static class ArenaObjectDataCollection
{
    public static ArenaObjectData[] collections;
    public static readonly string path = "Assets/Resources/ArenaObjectDatas";

    public async static Task ArenaObjectDataLoadAsync()
    {
        collections = Resources.LoadAll<ArenaObjectData>("ArenaObjectDatas");
        await Task.Yield();
    }
}