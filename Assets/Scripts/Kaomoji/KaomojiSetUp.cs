using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IKaomojiSetUp
{
    void SetUp(KaomojiData data, int exp = 0);
    List<SkillWithLevel> SkillsWithLevels { get; }
}

public interface IKaomojiStats
{
    bool TryGetSkill<T>(out T skill, out int level) where T : class;
    CoreStats TryGetCoreStats();
}

public class KaomojiSetUp : MonoBehaviour, IKaomojiSetUp, IKaomojiStats
{
    [SerializeField] TextMeshPro kaomojiBody;
    static readonly char default_FaceLineLeft = '(';
    static readonly char default_FaceLineRight = ')';

    KaomojiData data;

    int hasExp = 0;

    public List<SkillWithLevel> SkillsWithLevels { get; private set; } = new List<SkillWithLevel>();

    public void SetUp(KaomojiData data, int exp = 0)
    {
        string kaomoji = GetKaomojiCoupling(data);
        SkillsWithLevels = SkillData.GetAllSkillsWithLevelsFromKaomojiData(data);
        kaomojiBody.text = kaomoji;
        this.data = data;
        hasExp = exp;

        IStatsCalculator statsCalculator = GetComponentInParent<IStatsCalculator>();
        statsCalculator?.SetUp(this);
    }

    /// <summary>
    /// 指定されたKaomojiDataから顔文字を生成して返す
    /// </summary>
    public static string GetKaomojiCoupling(KaomojiData data)
    {
        if (data == null) return string.Empty;

        SymbolData leftHand = data.GetSymbolDataByType(SymbolType.LeftHand);
        SymbolData leftEye = data.GetSymbolDataByType(SymbolType.LeftEye);
        SymbolData mouth = data.GetSymbolDataByType(SymbolType.Mouth);
        SymbolData rightEye = data.GetSymbolDataByType(SymbolType.RightEye);
        SymbolData rightHand = data.GetSymbolDataByType(SymbolType.RightHand);

        int length = 0;
        if (leftHand != null) length++;
        if (leftEye != null) length++;
        if (mouth != null) length++;
        if (rightEye != null) length++;
        if (rightHand != null) length++;

        if (length == 0) return string.Empty;

        length += 2;    // 両端の括弧分を追加

        char[] buffer = new char[length];
        int index = 0;

        if (leftHand != null) buffer[index++] = leftHand.Symbol;
        buffer[index++] = default_FaceLineLeft;         // 左側の括弧を追加
        if (leftEye != null) buffer[index++] = leftEye.Symbol;
        if (mouth != null) buffer[index++] = mouth.Symbol;
        if (rightEye != null) buffer[index++] = rightEye.Symbol;
        buffer[index++] = default_FaceLineRight;        // 右側の括弧を追加
        if (rightHand != null) buffer[index++] = rightHand.Symbol;

        return new string(buffer);
    }

    /// <summary>
    /// 指定された特定のスキルクラス型、またはインターフェース (T) を持つ最初のスキル情報を、安全かつキャスト不要な参照として取得します。
    /// </summary>
    public bool TryGetSkill<T>(out T skill, out int level) where T : class
    {
        var found = SkillsWithLevels.Find(s => s.Skill is T);
        if (found != null)
        {
            skill = found.Skill as T;
            level = found.Level;
            return true;
        }

        skill = null;
        level = 0;
        return false;
    }

    /// <summary>
    /// KaomojiDataのMouthTypeに関連するCoreStatsを取得する。
    /// もしMouthTypeが存在しない場合、またはCoreStatsが定義されていない場合はnullを返す。
    /// </summary>
    public CoreStats TryGetCoreStats()
    {
        if (data == null) return null;

        SymbolData mouthData = data.GetSymbolDataByType(SymbolType.Mouth);
        if (mouthData is SymbolMouthData symbolMouthData)
        {
            return symbolMouthData.CoreStats;
        }

        return null;
    }
}