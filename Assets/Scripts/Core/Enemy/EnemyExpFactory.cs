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

    public class EnemyExpFactory : MonoBehaviour, IEnemyExpFactory
    {
        static readonly int DefaultEXP = 10;

        public int WaveEXPPool { get; private set; } = 0;

        public void AddWaveEXPPool(int exp) => WaveEXPPool += exp;
        public void ResetWaveEXPPool() => WaveEXPPool = 0;

        void Awake()
        {
            ApiProvider.Register<IEnemyExpFactory>(this);
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
                if (type == SymbolType.MAX) continue;       // MAXは無視する
                if (data.GetSymbolDataByType(type) != null) equipPartCount++;
            }

            return DefaultEXP * equipPartCount;
        }
    }
}