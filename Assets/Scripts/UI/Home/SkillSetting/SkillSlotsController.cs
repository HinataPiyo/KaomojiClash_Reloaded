namespace UI
{
    using UnityEngine;

    public class SkillSlotsController : MonoBehaviour
    {
        [SerializeField] Transform skillSlotsParent;
        [SerializeField] ConfirmationPanel confirmationPanel;
        [SerializeField] CurrentSymbolHandler currentSymbolHandler;
        [SerializeField] SymbolListHandler symbolListHandler;
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
                int slotIndex = i;

                skillSlots[i].Initialize(slotIndex, requiredAmount,
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

            symbolListHandler.OnSelectedSymbolChanged += SetSymbolData;
        }
        
        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= SetSymbolData;
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
            currentSymbolHandler.CurrentSymbolData.UnlockNextSkillSlot();       // 次のスキルスロットをアンロックする
        }

        /// <summary>
        /// 記号データを選択した際に実行されるメソッド
        /// SkillSlotsListを更新する
        /// </summary>
        /// <param name="data">設定する記号データ</param>
        public void SetSymbolData(SymbolData data)
        {
            if (HasSelectedSlot)
            {
                SelectedSlot.DeselectSlot();
                ClearSelectedSlot();
            }

            SymbolMouthData symbolMouthData = data as SymbolMouthData;
            if (symbolMouthData == null)
            {
                for (int i = 0; i < skillSlots.Length; i++)
                    skillSlots[i].LockSlot();

                return;
            }

            for (int i = 0; i < skillSlots.Length; i++)
            {
                // 現在アンロックしてるスロットを確認
                if (i < symbolMouthData.SkillSlot_Unlocked)
                {
                    // スキルデータが存在する場合は設定し、存在しない場合はスロットをアンロックする
                    if (i < symbolMouthData.SkillDatas.Count)
                    {
                        skillSlots[i].SetSkillData(symbolMouthData.SkillDatas[i]);
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