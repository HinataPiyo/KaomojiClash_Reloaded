using UnityEngine;

public interface IWallReflection
{
    int MaxLevel();
    float GetReflectionPower(int level);
    string[] GetDescription(int level);
}

[CreateAssetMenu(fileName = "WallReflection", menuName = "KaomojiClash_Reloaded/Skill/WallReflection")]
public class WallReflection : SkillData, IWallReflection
{
    [SerializeField] float[] reflectionPower;

    public override int MaxLevel() => reflectionPower.Length;
    public float GetReflectionPower(int level)
    {
        if (level < 1 || level > MaxLevel())
        {
            Debug.LogWarning($"Invalid level: {level}. Level must be between 1 and {MaxLevel()}.");
            return 0f;
        }

        return reflectionPower[level - 1];
    }

    public override string[] GetDescription(int level)
    {
        string[] descriptions = new string[MaxLevel()];
        for (int i = 0; i < MaxLevel(); i++)
        {
            descriptions[i] = $"現在のスピードの{GetReflectionPower(i + 1) * 100}%の力で壁を反射する。";
        }
        return descriptions;
    }
}