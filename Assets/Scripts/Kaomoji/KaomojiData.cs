using UnityEngine;

[CreateAssetMenu(fileName = "KaomojiData", menuName = "KaomojiClash_Reloaded/KaomojiData")]
public class KaomojiData : ScriptableObject
{
    [SerializeField] SymbolData symbol_Mouth;
    [SerializeField] SymbolData symbol_LeftEye;
    [SerializeField] SymbolData symbol_RightEye;
    [SerializeField] SymbolData symbol_LeftHand;
    [SerializeField] SymbolData symbol_RightHand;

    /// <summary>
    /// 指定されたSymbolTypに対応するSymbolDataを返す
    /// </summary>
    /// <param name="type">SymbolType</param>
    /// <returns>対応するSymbolData</returns>
    /// <exception cref="System.ArgumentException">無効なSymbolTypeが指定された場合にスローされる</exception>
    public SymbolData GetSymbolDataByType(SymbolType type)
    {
        return type switch
        {
            SymbolType.Mouth => symbol_Mouth,
            SymbolType.LeftEye => symbol_LeftEye,
            SymbolType.RightEye => symbol_RightEye,
            SymbolType.LeftHand => symbol_LeftHand,
            SymbolType.RightHand => symbol_RightHand,
            _ => throw new System.ArgumentException($"Invalid SymbolType: {type}"),
        };
    }
}