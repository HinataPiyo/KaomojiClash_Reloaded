using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Flags]
public enum SymbolType { Mouth = 1 << 0, LeftEye = 1 << 1, RightEye = 1 << 2, LeftHand = 1 << 3, RightHand = 1 << 4 }
public enum StatusType { Stamina, Power, Speed, Guard, CriticalDamage, CriticalRate }

[CreateAssetMenu(fileName = "SymbolData", menuName = "KaomojiClash_Reloaded/SymbolData")]
public class SymbolData : ScriptableObject
{
    [SerializeField] SymbolType symbolType;
    [SerializeField] char symbol;
    [SerializeField] SkillData[] skillData;

    public SymbolType SymbolType => symbolType;
    public char Symbol => symbol;
    public SkillData[] SkillData => skillData;
}

public static class SymbolDataCollection
{
    public static Dictionary<SymbolType, List<SymbolData>> collections = new Dictionary<SymbolType, List<SymbolData>>();
    public static readonly string path = "Assets/Resources/SymbolDatas";
    public async static Task SymbolDataLoadAsync()
    {
        collections.Clear();
        SymbolData[] symbolDatas = Resources.LoadAll<SymbolData>("SymbolDatas");
        foreach (var data in symbolDatas)
        {
            if (!collections.ContainsKey(data.SymbolType))
            {
                collections[data.SymbolType] = new List<SymbolData>();
            }
            collections[data.SymbolType].Add(data);
        }
        await Task.Yield();
    }

    public static SymbolData GetRandomSymbolData(SymbolType type)
    {
        if (!collections.ContainsKey(type) || collections[type].Count == 0)
        {
            Debug.LogWarning($"No SymbolData found for SymbolType: {type}");
            return null;
        }
        var list = collections[type];
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}