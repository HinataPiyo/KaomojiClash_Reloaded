using System.Collections.Generic;
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

    public void SetSymbolDataByType(SymbolData data)
    {
        switch (data.SymbolType)
        {
            case SymbolType.Mouth:
                symbol_Mouth = data;
                break;
            case SymbolType.LeftEye:
                symbol_LeftEye = data;
                break;
            case SymbolType.RightEye:
                symbol_RightEye = data;
                break;
            case SymbolType.LeftHand:
                symbol_LeftHand = data;
                break;
            case SymbolType.RightHand:
                symbol_RightHand = data;
                break;
            default:
                throw new System.ArgumentException($"Invalid SymbolType: {data.SymbolType}");
        }
    }

    /// <summary>
    /// すべてのSymbolDataからスキルとそのレベルを取得する
    /// </summary>
    public Dictionary<SkillData, int> GetAllSkillsWithLevels()
    {
        var skillsWithLevels = new Dictionary<SkillData, int>();

        // すべてのSymbolTypeをループして、対応するSymbolDataからスキルを取得
        foreach (SymbolType type in System.Enum.GetValues(typeof(SymbolType)))
        {
            if (type == SymbolType.MAX) continue;       // MAXは無視する

            SymbolData symbolData = GetSymbolDataByType(type);      // 対応するSymbolDataを取得
            if (symbolData != null)
            {
                // SymbolDataからスキルを取得し、Dictionaryに追加
                foreach (var skill in symbolData.SkillData)
                {
                    // すでにDictionaryに存在する場合はスキルレベルを更新しない
                    if (!skillsWithLevels.ContainsKey(skill))
                    {
                        skillsWithLevels[skill] = 1;        // 初期レベルを1に設定
                    }
                    // すでに存在する場合はレベルをインクリメント(ただし、MaxLevelを超えないようにする)
                    else if (skillsWithLevels[skill] < skill.MaxLevel())
                    {
                        skillsWithLevels[skill]++;          // すでに存在する場合はレベルをインクリメント
                    }
                }
            }
        }

        return skillsWithLevels;
    }

    public (float, float) GetStatusValue(StatusType type)
    {
        SymbolMouthData mouthData = symbol_Mouth as SymbolMouthData;
        if(mouthData == null) return (0f, 0f); // symbol_MouthがSymbolMouthDataでない場合は(0, 0)を返す
        switch (type)
        {
            case StatusType.Speed:
                return (mouthData.CoreStats.Speed, CoreStats.Max_Speed);
            case StatusType.Power:
                return (mouthData.CoreStats.Power, CoreStats.Max_Power);
            case StatusType.Stamina:
                return (mouthData.CoreStats.Stamina, CoreStats.Max_Stamina);
            case StatusType.Guard:
                return (mouthData.CoreStats.Guard, CoreStats.Max_Guard);
            case StatusType.CriticalDamage:
                return (0f, 1f); // 仮の値
            case StatusType.CriticalRate:
                return (0f, 1f); // 仮の値
            default:
                throw new System.ArgumentException($"Invalid StatusType: {type}");
        }
    }
}