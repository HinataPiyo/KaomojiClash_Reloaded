namespace UI
{
    using UnityEngine;

    /// <summary>
    /// AIリファクタリング: スキル設定画面のPresenter。入力を受けてサービスと各Viewを調整する。
    /// </summary>
    public class SkillSlotsController : MonoBehaviour
    {
        [SerializeField] Transform skillSlotsParent;
        [SerializeField] ConfirmationPanel confirmationPanel;
        [SerializeField] CurrentSymbolHandler currentSymbolHandler;
        [SerializeField] SkillDescriptionHandler skillDescriptionHandler;
        [SerializeField] SymbolListHandler symbolListHandler;
        [SerializeField] SkillNameListHandler skillNameListHandler;
        [SerializeField] WallReflection testSkillData;

        SkillSlotHandler[] skillSlots;
        SkillSlotHandler selectedSlot;
        SymbolMouthData currentSymbolData;
        readonly SkillLoadoutService skillLoadoutService = new SkillLoadoutService();

        static readonly int[] RequireAmounts = { 100, 1000, 2500, 5000, 10000 };
        const string ConfirmationMessage = "Do you want to unlock this skill slot?";

        void Awake()
        {
            skillSlots = skillSlotsParent.GetComponentsInChildren<SkillSlotHandler>();
            if (skillSlots.Length > RequireAmounts.Length)
                throw new System.InvalidOperationException("The number of skill slots exceeds the configured unlock prices.");

            for (int i = 0; i < skillSlots.Length; i++)
            {
                int slotIndex = i;
                skillSlots[i].Initialize(
                    slotIndex,
                    RequireAmounts[i],
                    HandleLockedSlotClicked,
                    HandleSlotClicked);
            }

            if (testSkillData != null && !GlobalVariables.HasSkills.Contains(testSkillData))
                GlobalVariables.HasSkills.Add(testSkillData);

            symbolListHandler.OnSelectedSymbolChanged += HandleSymbolChanged;
            skillNameListHandler.OnSkillSelected += HandleSkillSelected;
        }

        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= HandleSymbolChanged;
            skillNameListHandler.OnSkillSelected -= HandleSkillSelected;
        }

        /// <summary>
        /// 記号が変わったときの処理
        /// </summary>
        void HandleSymbolChanged(SymbolData symbolData)
        {
            currentSymbolData = symbolData as SymbolMouthData;
            currentSymbolHandler.SetCurrentSymbol(symbolData);
            ClearSelectedSlot();
            skillDescriptionHandler.Clear();
            skillNameListHandler.SetSkillNameList(GlobalVariables.HasSkills);
            RefreshSlots();
        }

        /// <summary>
        /// スロットがクリックされたときの処理
        /// </summary>
        void HandleSlotClicked(SkillSlotHandler slot)
        {
            if (selectedSlot != null)
                selectedSlot.DeselectSlot();

            selectedSlot = slot;
            selectedSlot.SelectedSlot();
            skillDescriptionHandler.SetSkillDescription(slot.SkillData);
            RefreshSkillButtons();
        }

        /// <summary>
        /// ロックされたスロットがクリックされたときの処理
        /// </summary>
        void HandleLockedSlotClicked(SkillSlotHandler slot)
        {
            ClearSelectedSlot();
            skillDescriptionHandler.Clear();
            RefreshSkillButtons();

            if (!CanUnlockSlot(slot))
            {
                return;
            }

            confirmationPanel.Show(ConfirmationMessage, () => UnlockSlot(slot));
        }

        /// <summary>
        /// スロットをアンロックする
        /// </summary>
        void UnlockSlot(SkillSlotHandler slot)
        {
            confirmationPanel.Hide();
            if (!skillLoadoutService.TryUnlockNextSlot(
                currentSymbolData,
                slot.SlotIndex,
                skillSlots.Length,
                Player.Funds.CurrentGold,
                slot.RequireAmount))
            {
                RefreshSlots();
                return;
            }

            // ゴールドを消費してアンロック
            Player.Funds.SpendGold(slot.RequireAmount);
            RefreshSlots();
        }

        /// <summary>
        /// スキルが選択されたときの処理。選択中のスロットに割り当てる
        /// </summary>
        /// <param name="skillData"></param>
        void HandleSkillSelected(SkillData skillData)
        {
            skillDescriptionHandler.SetSkillDescription(skillData);
            if (selectedSlot == null || currentSymbolData == null)
                return;

            // スキルを割り当てる
            if (!skillLoadoutService.TryAssignSkill(
                currentSymbolData,
                selectedSlot.SlotIndex,
                skillData,
                skillSlots.Length))
            {
                ClearSelectedSlot();
                RefreshSlots();
                return;
            }

            // 割り当て成功したので更新
            selectedSlot.SetSkillData(skillData);
            selectedSlot = null;
            RefreshCostLocks();
            RefreshSkillButtons();
        }

        /// <summary>
        /// スロットの状態を更新する（アンロックやスキル割り当て反映）
        /// </summary>
        void RefreshSlots()
        {
            if (currentSymbolData == null)
            {
                foreach (SkillSlotHandler slot in skillSlots)
                {
                    slot.LockSlot();
                    slot.SetLockedSlotInteractable(false);
                }

                RefreshSkillButtons();
                return;
            }

            for (int i = 0; i < skillSlots.Length; i++)
            {
                // アンロックされているか確認
                if (i >= currentSymbolData.SkillSlot_Unlocked)
                {
                    skillSlots[i].LockSlot();
                    skillSlots[i].SetLockedSlotInteractable(CanUnlockSlot(skillSlots[i]));
                    continue;
                }

                // スキルを取得
                SkillData skillData = i < currentSymbolData.SkillDatas.Count
                    ? currentSymbolData.SkillDatas[i]
                    : null;

                // スキルがあるか
                if (skillData == null)
                    skillSlots[i].UnlockSlot();
                else
                    skillSlots[i].SetSkillData(skillData);
            }

            RefreshCostLocks();
            RefreshSkillButtons();
        }

        /// <summary>
        /// コストロックを更新する（使用中スロットを反映）
        /// </summary>
        void RefreshCostLocks()
        {
            foreach (SkillSlotHandler slot in skillSlots)
                slot.ResetByCostLock();

            if (currentSymbolData == null)
                return;

            bool[] occupiedSlots = skillLoadoutService.GetOccupiedSlots(currentSymbolData, skillSlots.Length);
            for (int i = 0; i < occupiedSlots.Length; i++)
            {
                if (occupiedSlots[i])
                    skillSlots[i].ByCostLockSlot();
            }
        }

        /// <summary>
        /// スキルボタンの状態を更新する（割り当て可能か）
        /// </summary>
        void RefreshSkillButtons()
        {
            skillNameListHandler.RefreshInteractability(CanAssignSkill);
        }

        /// <summary>
        /// 選択中のスロットに割り当て可能か
        /// </summary>
        bool CanAssignSkill(SkillData skillData)
        {
            if (currentSymbolData == null)
                return false;

            // 選択中スロットがあるか確認
            if (selectedSlot != null)
            {
                return skillLoadoutService.CanAssignSkill(
                    currentSymbolData,
                    selectedSlot.SlotIndex,
                    skillData,
                    skillSlots.Length);
            }

            return skillLoadoutService.CanAssignSkillAnywhere(currentSymbolData, skillData, skillSlots.Length);
        }

        /// <summary>
        /// スロットをアンロック可能か
        /// </summary>
        bool CanUnlockSlot(SkillSlotHandler slot)
        {
            return skillLoadoutService.CanUnlockNextSlot(
                currentSymbolData,
                slot.SlotIndex,
                skillSlots.Length,
                Player.Funds.CurrentGold,
                slot.RequireAmount);
        }

        /// <summary>
        /// 選択中スロットをクリアする
        /// </summary>
        void ClearSelectedSlot()
        {
            if (selectedSlot == null)
                return;

            selectedSlot.DeselectSlot();
            selectedSlot = null;
        }
    }
}
