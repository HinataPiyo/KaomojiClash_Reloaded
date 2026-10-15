namespace Player
{
    public static class Funds
    {
        public static float CurrentGold { get; private set; } = 1000f;  
        public static void AddGold(float amount)
        {
            CurrentGold += amount;
        }

        /// <summary>
        /// 指定した金額を消費できるか確認し、消費可能であれば金額を減らすメソッド
        /// </summary>
        /// <param name="amount">消費する金額</param>
        /// <returns>消費に成功した場合はtrue、失敗した場合はfalse</returns>
        public static bool SpendGold(float amount)
        {
            if (CurrentGold >= amount)
            {
                // CurrentGold -= amount;
                UnityEngine.Debug.Log($"Spent {amount} gold.");
                return true;
            }
            
            UnityEngine.Debug.Log($"Failed to spend {amount} gold. Not enough funds.");
            return false;
        }
    }
}