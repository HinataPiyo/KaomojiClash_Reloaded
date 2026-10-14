namespace Player
{
    public static class Funds
    {
        public static float CurrentGold { get; private set; }
        public static void AddGold(float amount)
        {
            CurrentGold += amount;
        }

        public static bool SpendGold(float amount)
        {
            if (CurrentGold >= amount)
            {
                CurrentGold -= amount;
                return true;
            }
            return false;
        }
    }
}