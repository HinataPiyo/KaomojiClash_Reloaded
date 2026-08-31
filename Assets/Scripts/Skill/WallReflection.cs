using UnityEngine;

public interface IWallReflection
{
    int MaxLevel();
    float GetReflectionPower(int level);
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

    public override string GetDescription(int level)
    {
        return $"Wall Reflection Power: {GetReflectionPower(level) * 100}%";
    }
}