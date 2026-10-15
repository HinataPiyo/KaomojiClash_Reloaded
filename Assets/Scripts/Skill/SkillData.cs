using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "KaomojiClash_Reloaded/SkillData")]
public abstract class SkillData : ScriptableObject
{
    [SerializeField] string skillName;
    [SerializeField] int cost;

    public string SkillName => skillName;
    public int Cost => cost;
    public abstract string GetDescription();
}