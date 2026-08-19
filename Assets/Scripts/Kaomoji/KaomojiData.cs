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
    /// <param name="type">SymbolTyp</param>
    /// <returns>対応するSymbolData</returns>
    /// <exception cref="System.ArgumentException">無効なSymbolTypが指定された場合にスローされる</exception>
    public SymbolData GetSymbolDataByType(SymbolTyp type)
    {
        return type switch
        {
            SymbolTyp.Mouth => symbol_Mouth,
            SymbolTyp.LeftEye => symbol_LeftEye,
            SymbolTyp.RightEye => symbol_RightEye,
            SymbolTyp.LeftHand => symbol_LeftHand,
            SymbolTyp.RightHand => symbol_RightHand,
            _ => throw new System.ArgumentException($"Invalid SymbolTyp: {type}"),
        };
    }
}