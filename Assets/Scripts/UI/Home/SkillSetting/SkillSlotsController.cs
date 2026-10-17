namespace UI
{
    using System.Collections.Generic;
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
        public void ClearSelectedSlot()
        {
            SelectedSlot = null;
            OnSlotStateChanged?.Invoke();
        }

        /// <summary>
        /// 選択スロットやスロットの使用状況が変化したときに通知される
        /// </summary>
        public event System.Action OnSlotStateChanged;

        /// <summary>
        /// 指定スロットを起点に、スキルのコスト分の連続した空きスロットがあるか
        /// </summary>
        public bool CanPlaceSkill(SkillData skill, int startSlot)
        {
            int cost = Mathf.Max(1, skill.Cost);
            if (startSlot < 0 || startSlot + cost > skillSlots.Length)
                return false;

            // 置き換え対象(startSlot)以外のスキルが占有しているスロットを集計する
            bool[] occupied = new bool[skillSlots.Length];
            for (int i = 0; i < skillSlots.Length; i++)
            {
                if (i == startSlot || skillSlots[i].SkillData == null)
                    continue;

                int end = Mathf.Min(i + Mathf.Max(1, skillSlots[i].SkillData.Cost), skillSlots.Length);
                for (int k = i; k < end; k++)
                    occupied[k] = true;
            }

            for (int k = startSlot; k < startSlot + cost; k++)
            {
                if (!skillSlots[k].IsUnlocked || occupied[k])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// スキルを設定できるスロットが存在するか(スロット選択中は選択スロットのみ判定)
        /// </summary>
        public bool CanPlaceSkill(SkillData skill)
        {
            if (HasSelectedSlot)
                return CanPlaceSkill(skill, SelectedSlot.SlotIndex);

            for (int i = 0; i < skillSlots.Length; i++)
            {
                if (CanPlaceSkill(skill, i))
                    return true;
            }

            return false;
        }

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
                    OnSlotStateChanged?.Invoke();
                });
            }

            symbolListHandler.OnSelectedSymbolChanged += SetSymbolData;
        }
        
        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= SetSymbolData;
        }

        /// <summary>
        /// スキルのコスト分スロットを消費する
        /// </summary>
        public void SlotUsageByCost(SymbolMouthData data)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));

            for (int i = 0; i < skillSlots.Length; i++)
                skillSlots[i].ResetByCostLock();

            int unlockedSlotCount = Mathf.Min(data.SkillSlot_Unlocked, skillSlots.Length);
            List<SkillData> skills = data.SkillDatas;
            for (int i = 0; i < skills.Count && i < unlockedSlotCount; i++)
            {
                SkillData skill = skills[i];
                if (skill == null)
                    continue;

                int cost = Mathf.Max(0, skill.Cost);
                int lastConsumedSlot = Mathf.Min(i + cost, unlockedSlotCount);
                for (int slotIndex = i; slotIndex < lastConsumedSlot; slotIndex++)
                    skillSlots[slotIndex].ByCostLockSlot();
            }

            OnSlotStateChanged?.Invoke();
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
            OnSlotStateChanged?.Invoke();
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

                OnSlotStateChanged?.Invoke();
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

            SlotUsageByCost(symbolMouthData);
        }
    }
}