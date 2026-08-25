namespace ArenaObject.Bumper
{
    using System;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "Bumper", menuName = "KaomojiClash_Reloaded/ArenaObject/Bumper")]
    public class Data : ArenaObjectData
    {
        // Bumperに必要なパラメータを追加していく
        [SerializeField] float reflectionPower = 1.1f;
        [SerializeField] float multiplier = 0.2f;

        public float GetReflectionPower(int level)
        {
            return reflectionPower + multiplier * level;
        }

        public override string GetDescription(int level)
        {
            return $"Bumper Level {level}: Reflection Power = {GetReflectionPower(level) * 100f:0}%";
        }
    }
}