using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolMouthData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolMouthData")]
public class SymbolMouthData : SymbolData
{
    [Header("MouthTypeのステータス")]
    [SerializeField] List<SkillData> skillDatas = new List<SkillData>();
    [SerializeField] int skillSlot_UnlockedCount;
    public static readonly int Max_SkillSlot_UnlockedCount = 5;

    public List<SkillData> SkillDatas => skillDatas;
    public int SkillSlot_Unlocked => skillSlot_UnlockedCount;

    public void UnlockNextSkillSlot()
    {
        if (skillSlot_UnlockedCount < Max_SkillSlot_UnlockedCount)
            skillSlot_UnlockedCount++;
    }

    /// <summary>
    /// 指定したスロットにスキルデータを追加または更新する
    /// </summary>
    /// <param name="slotIndex">追加または更新するスロットのインデックス</param>
    /// <param name="skillData">追加または更新するスキルデータ</param>
    public void AddSkillData(int slotIndex, SkillData skillData)
    {
        if (slotIndex < 0 || slotIndex >= Max_SkillSlot_UnlockedCount)
            return;

        if (slotIndex < skillDatas.Count)
            skillDatas[slotIndex] = skillData;
        else
            skillDatas.Add(skillData);
    }
}