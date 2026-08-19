using UnityEngine;

public enum SymbolTyp { Mouth, LeftEye, RightEye, LeftHand, RightHand, MAX }

[CreateAssetMenu(fileName = "SymbolData", menuName = "KaomojiClash_Reloaded/SymbolData")]
public class SymbolData : ScriptableObject
{
    [SerializeField] SymbolTyp symbolType;
    [SerializeField] string symbolName;
    [SerializeField] char symbol;

    public SymbolTyp SymbolType => symbolType;
    public string SymbolName => symbolName;
    public char Symbol => symbol;
}