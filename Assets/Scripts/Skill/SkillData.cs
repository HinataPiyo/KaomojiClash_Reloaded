using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "KaomojiClash_Reloaded/SkillData")]
public abstract class SkillData : ScriptableObject
{
    [SerializeField] string skillName;

    public abstract int MaxLevel();
    public abstract string[] GetDescription(int level);

    public static List<SkillWithLevel> GetAllSkillsWithLevelsFromKaomojiData(KaomojiData data)
    {
        var skillsWithLevels = new List<SkillWithLevel>();

        // すべてのSymbolTypeをループして、対応するSymbolDataからスキルを取得
        foreach (SymbolType type in System.Enum.GetValues(typeof(SymbolType)))
        {
            if (type == SymbolType.MAX) continue;       // MAXは無視する

            SymbolData symbolData = data.GetSymbolDataByType(type);      // 対応するSymbolDataを取得
            if (symbolData != null)
            {
                if(symbolData.SkillData == null || symbolData.SkillData.Length <= 0) continue;      // スキルがない場合はスキップ

                // SymbolDataからスキルを取得し、Listに追加
                foreach (var skill in symbolData.SkillData)
                {
                    // すでにListに存在する場合はスキルレベルを更新しない
                    var existingSkill = skillsWithLevels.Find(s => s.Skill == skill);
                    if (existingSkill == null)
                    {
                        skillsWithLevels.Add(new SkillWithLevel(skill, 1));        // 初期レベルを1に設定
                    }
                    // すでに存在する場合はレベルをインクリメント(ただし、MaxLevelを超えないようにする)
                    else if (existingSkill.Level < skill.MaxLevel())
                    {
                        skillsWithLevels[skillsWithLevels.IndexOf(existingSkill)] = new SkillWithLevel(skill, existingSkill.Level + 1);
                    }
                }
            }
        }

        return skillsWithLevels;
    }
}

[System.Serializable]
public class SkillWithLevel
{
    public SkillData Skill { get; private set; }
    public int Level { get; private set; }

    public SkillWithLevel(SkillData skill, int level)
    {
        Skill = skill;
        Level = level;
    }
}