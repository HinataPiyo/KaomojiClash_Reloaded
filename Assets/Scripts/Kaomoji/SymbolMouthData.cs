using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolMouthData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolMouthData")]
public class SymbolMouthData : SymbolData
{
    [Header("MouthTypeのステータス")]
    [SerializeField] List<SkillData> skillDatas;
    [SerializeField] int skillSlot_UnlockedCount;
    public static readonly int Max_SkillSlot_UnlockedCount = 5;

    public List<SkillData> SkillDatas => skillDatas;
    public int SkillSlot_Unlocked => skillSlot_UnlockedCount;
}