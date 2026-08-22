namespace Player
{
    using UnityEngine;

    public interface ICombo
    {
        float ComboDamageRate { get; }
        float ComboSpeedRate { get; }
        void AddCombo();
    }

    // プレイヤーが敵に被ダメージを受けずに攻撃し続けることができたらステータス全体が上昇する。
    // 主に、攻撃力と速度が上昇する。    
    public class Combo : MonoBehaviour, ICombo
    {
        static readonly float ComboDuration = 3f;           // コンボが途切れるまでの時間
        static readonly float ComboMaxDamageRate = 1.5f;    // コンボ中の最大攻撃力倍率
        static readonly float ComboMaxSpeedRate = 1.2f;     // コンボ中の最大速度倍率
        static readonly int ComboMaxCount = 15;             // コンボの最大数

        float comboTimer = 0f;                              // コンボタイマー
        int comboCount = 0;                                 // コンボ数
        int beforeComboCount = 0;                           // 前回のコンボ数

        public float ComboDamageRate => Mathf.Lerp(0.8f, ComboMaxDamageRate, (float)comboCount / ComboMaxCount);
        public float ComboSpeedRate => Mathf.Lerp(0.8f, ComboMaxSpeedRate, (float)comboCount / ComboMaxCount);

        public void AddCombo()
        {
            comboCount++;
            comboTimer = ComboDuration;
        }

        void Awake()
        {
            ApiProvider.Register<ICombo>(this);
        }

        void Update()
        {
            // コンボタイマーを減算する
            if (comboCount > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    beforeComboCount = comboCount;
                    comboCount = 0;
                }
            }

            // コンボ数が前回のコンボ数よりも増えた場合、コンボタイマーをリセットする
            if(comboCount > beforeComboCount)
            {
                beforeComboCount = comboCount;
                comboTimer = ComboDuration;
            }
        }
    }
}