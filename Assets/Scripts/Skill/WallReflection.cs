using UnityEngine;

public interface IWallReflection
{
    float GetReflectionPower();
}

[CreateAssetMenu(fileName = "WallReflection", menuName = "KaomojiClash_Reloaded/Skill/WallReflection")]
public class WallReflection : SkillData, IWallReflection
{
    [SerializeField] float reflectionPower = 1.5f;

    public float GetReflectionPower()
    {
        return reflectionPower;
    }

    public override string GetDescription()
    {
        return $"Wall Reflection ";
    }
}