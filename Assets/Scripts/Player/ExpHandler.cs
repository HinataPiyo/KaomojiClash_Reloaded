namespace Player
{
    using UI;
    using UnityEngine;

    public interface IExpHandler
    {
        int HasPlayerEXP { get; }
        void AddPlayerEXP(int exp);
        int Level { get; }
        int LevelUpCount { get; }
        void ResetLevelUpCount();
    }
    
    public class ExpHandler : MonoBehaviour, IExpHandler
    {
        static readonly int DefaultLevelUpBorder = 100;
        public int HasPlayerEXP { get; private set; } = 0;
        public int Level { get; private set; } = 1;     // プレイヤーのレベルは例外で1スタートとする
        public int GetLevelUpBorder() => DefaultLevelUpBorder * Level;

        public int LevelUpCount { get; private set; } = 0; // レベルアップ回数を追跡するプロパティ
        public void ResetLevelUpCount() => LevelUpCount = 0; // レベルアップ回数をリセットするメソッド

        
        IPlayerEXPUI playerEXPUI;

        void Awake()
        {
            ApiProvider.Register<IExpHandler>(this);
        }

        void Start()
        {
            playerEXPUI = ApiProvider.Get<IPlayerEXPUI>();
            AddPlayerEXP(0); // 初期化時にUIを更新
        }

        void CheckLevelUp()
        {
            int levelUpBorder = DefaultLevelUpBorder * Level;
            if (HasPlayerEXP >= levelUpBorder)
            {
                Level++;
                LevelUpCount++;
                HasPlayerEXP -= levelUpBorder;
                Debug.Log($"<color=green>Level Up! New Level: {Level}, Remaining EXP: {HasPlayerEXP}</color>");
            }
        }

        public void AddPlayerEXP(int exp)
        {
            HasPlayerEXP += exp;
            CheckLevelUp();
            playerEXPUI.SetEXP(HasPlayerEXP, exp, GetLevelUpBorder(), Level);
        }
    }
}