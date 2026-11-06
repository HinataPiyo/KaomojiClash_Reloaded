namespace UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// AIリファクタリング: スキル配置・コスト占有・解放のルールとモデル更新を集約する。
    /// </summary>
    public sealed class SkillLoadoutService
    {
        const int MinimumSkillCost = 1;

        public bool CanAssignSkill(SymbolMouthData loadout, int startSlot, SkillData skill, int slotCapacity)
        {
            if (loadout == null || skill == null || slotCapacity <= 0)
                return false;

            int unlockedCount = GetUnlockedCount(loadout, slotCapacity);
            int skillCost = Mathf.Max(MinimumSkillCost, skill.Cost);
            if (startSlot < 0 || startSlot >= unlockedCount || startSlot + skillCost > unlockedCount)
                return false;

            bool[] occupiedSlots = GetOccupiedSlots(loadout, slotCapacity, startSlot);
            for (int i = 0; i < loadout.SkillDatas.Count && i < unlockedCount; i++)
            {
                if (i != startSlot && loadout.SkillDatas[i] == skill)
                    return false;
            }

            for (int i = startSlot; i < startSlot + skillCost; i++)
            {
                if (occupiedSlots[i])
                    return false;
            }

            return true;
        }

        public bool CanAssignSkillAnywhere(SymbolMouthData loadout, SkillData skill, int slotCapacity)
        {
            if (loadout == null)
                return false;

            int unlockedCount = GetUnlockedCount(loadout, slotCapacity);
            for (int i = 0; i < unlockedCount; i++)
            {
                if (CanAssignSkill(loadout, i, skill, slotCapacity))
                    return true;
            }

            return false;
        }

        public bool[] GetOccupiedSlots(SymbolMouthData loadout, int slotCapacity)
        {
            return GetOccupiedSlots(loadout, slotCapacity, -1);
        }

        public bool CanUnlockNextSlot(
            SymbolMouthData loadout,
            int slotIndex,
            int slotCapacity,
            float currentGold,
            float requiredAmount)
        {
            return loadout != null
                && slotIndex == loadout.SkillSlot_Unlocked
                && slotIndex >= 0
                && slotIndex < slotCapacity
                && slotIndex < SymbolMouthData.Max_SkillSlot_UnlockedCount
                && currentGold >= requiredAmount;
        }

        public bool TryUnlockNextSlot(
            SymbolMouthData loadout,
            int slotIndex,
            int slotCapacity,
            float currentGold,
            float requiredAmount)
        {
            if (!CanUnlockNextSlot(loadout, slotIndex, slotCapacity, currentGold, requiredAmount))
                return false;

            loadout.UnlockNextSkillSlot();
            return loadout.SkillSlot_Unlocked == slotIndex + 1;
        }

        bool[] GetOccupiedSlots(SymbolMouthData loadout, int slotCapacity, int excludedStartSlot)
        {
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            bool[] occupiedSlots = new bool[Mathf.Max(0, slotCapacity)];
            int unlockedCount = GetUnlockedCount(loadout, slotCapacity);
            for (int i = 0; i < loadout.SkillDatas.Count && i < unlockedCount; i++)
            {
                SkillData skill = loadout.SkillDatas[i];
                if (i == excludedStartSlot || skill == null)
                    continue;

                int cost = Mathf.Max(MinimumSkillCost, skill.Cost);
                int end = Mathf.Min(i + cost, unlockedCount);
                for (int slotIndex = i; slotIndex < end; slotIndex++)
                    occupiedSlots[slotIndex] = true;
            }

            return occupiedSlots;
        }

        int GetUnlockedCount(SymbolMouthData loadout, int slotCapacity)
        {
            return Mathf.Min(
                Mathf.Clamp(loadout.SkillSlot_Unlocked, 0, SymbolMouthData.Max_SkillSlot_UnlockedCount),
                Mathf.Max(0, slotCapacity));
        }

        public bool TryAssignSkill(SymbolMouthData loadout, int startSlot, SkillData skill, int slotCapacity)
        {
            if (!CanAssignSkill(loadout, startSlot, skill, slotCapacity))
                return false;

            loadout.AddSkillData(startSlot, skill);
            return true;
        }
    }
}
