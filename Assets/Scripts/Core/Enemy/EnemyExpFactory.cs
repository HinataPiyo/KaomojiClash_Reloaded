namespace Enemy
{
    using UnityEngine;

    public interface IEnemyExpFactory
    {
        int CreateEnemyEXP(KaomojiData data);
        int WaveEXPPool { get; }
        void AddWaveEXPPool(int exp);
        void ResetWaveEXPPool();
    }

    public class EnemyExpFactory : SceneSingleton<EnemyExpFactory>, IEnemyExpFactory
    {
        static readonly int DefaultEXP = 35;

        public int WaveEXPPool { get; private set; } = 0;

        public void AddWaveEXPPool(int exp) => WaveEXPPool += exp;
        public void ResetWaveEXPPool() => WaveEXPPool = 0;

        protected override void OnSingletonAwake()
        {
        }

        /// <summary>
        /// 敵のEXPを生成する
        /// </summary>
        /// <param name="data">敵のデータ</param>
        /// <returns>生成されたEXP</returns>
        public int CreateEnemyEXP(KaomojiData data)
        {
            int equipPartCount = 0;
            foreach (SymbolType type in System.Enum.GetValues(typeof(SymbolType)))
            {
                if (data.GetSymbolDataByType(type) != null) equipPartCount++;
            }

            return DefaultEXP * equipPartCount;
        }
    }
}