namespace UI
{
    using UnityEngine;

    public class SkillSlotsController : MonoBehaviour
    {
        [SerializeField] Transform skillSlotsParent;
        [SerializeField] ConfirmationPanel confirmationPanel;
        SkillSlotHandler[] skillSlots;

        // スキル変更処理に必要な項目
        public SkillSlotHandler SelectedSlot { get; private set; }
        public bool HasSelectedSlot => SelectedSlot != null;
        public void ClearSelectedSlot() => SelectedSlot = null;

        // スロットを解放するためのスロット数ごとの必要金額
        static readonly int[] requireAmount = { 100, 1000, 2500, 5000, 10000 };
        static readonly string ConfirmationMessage = "Do you want to unlock this skill slot?";
        void Awake()
        {
            skillSlots = skillSlotsParent.GetComponentsInChildren<SkillSlotHandler>();

            // 各スロットに必要金額を設定する
            for (int i = 0; i < skillSlots.Length; i++)
            {
                int requiredAmount = requireAmount[i];
                skillSlots[i].Initialize(requiredAmount,
                (s) => {
                    if(HasSelectedSlot)     // 既に選択されているスロットがある場合は選択を解除する
                        SelectedSlot.DeselectSlot();
                    // スロットをアンロックするか確認する
                    confirmationPanel.Show(ConfirmationMessage, () => OnClickConfirmationYes(s));
                }, 
                (s) => {
                    // スロットの追加ボタンがクリックされたときの処理
                    if(HasSelectedSlot)     // 既に選択されているスロットがある場合は選択を解除する
                        SelectedSlot.DeselectSlot();
                    SelectedSlot = s;
                    s.SelectedSlot();
                });
            }
        }

        /// <summary>
        /// 確認画面で「はい」を選択した際に実行されるメソッド
        /// </summary>
        /// <param name="requiredAmount">アンロックするスロットに必要な金額</param>
        void OnClickConfirmationYes(SkillSlotHandler slot)
        {
            confirmationPanel.Hide();
            Player.Funds.SpendGold(slot.RequireAmount);
            slot.UnlockSlot();
        }

        /// <summary>
        /// 記号データを選択した際に実行されるメソッド
        /// SkillSlotsListを更新する
        /// </summary>
        /// <param name="data">設定する記号データ</param>
        public void SetSymbolData(SymbolMouthData data)
        {
            for (int i = 0; i < skillSlots.Length; i++)
            {
                // 現在アンロックしてるスロットを確認
                if (i < data.SkillSlot_Unlocked)
                {
                    // スキルデータが存在する場合は設定し、存在しない場合はスロットをアンロックする
                    if (i < data.SkillDatas.Count)
                    {
                        skillSlots[i].SetSkillData(data.SkillDatas[i]);
                        continue;
                    }

                    skillSlots[i].UnlockSlot();
                }
                else
                {
                    // スロットがアンロックされていない場合はロックする
                    skillSlots[i].LockSlot();
                }
            }
        }
    }
}