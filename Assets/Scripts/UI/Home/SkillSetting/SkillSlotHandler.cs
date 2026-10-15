namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;

    public enum SkillSlotState { Locked, Unlocked, Set }
    public class SkillSlotHandler : MonoBehaviour
    {
        SkillSlotState slotState;
        [SerializeField] Button addSlot;
        [SerializeField] Button lockedSlot;
        [SerializeField] Button settingSlot;

        [Header("Skill Info")]
        [SerializeField] TextMeshProUGUI skillName;
        [SerializeField] TextMeshProUGUI skillCost;

        [Header("Selected Slot")]
        [SerializeField] Color selectedSlotColor;
        Color defaultSlotColor;
    
        public float RequireAmount { get; private set; }        // スロットをアンロックするために必要な金額

        /// <summary>
        /// スロットを初期化する
        /// </summary>
        /// <param name="amount">スロットをアンロックするために必要な金額</param>
        public void Initialize(float amount, System.Action<SkillSlotHandler> onClickLockedSlot, System.Action<SkillSlotHandler> onClickAddSlot)
        {
            LockSlot();
            SetRequireAmount(amount);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = "-";

            //* 確認画面を表示する(自身のスロットを渡しControllerで状態を変更させる)
            lockedSlot.onClick.AddListener(() => onClickLockedSlot?.Invoke(this));
            addSlot.onClick.AddListener(() => onClickAddSlot?.Invoke(this));
            settingSlot.onClick.AddListener(() => onClickAddSlot?.Invoke(this));
        }

        void SetRequireAmount(float amount)
        {
            RequireAmount = amount;
        }
        
        void ChangeSlotState(SkillSlotState newState)
        {
            slotState = newState;
        }

        /// <summary>
        /// スロットのアンロック条件を現在の所持金と比較する
        /// </summary>
        /// <param name="currentGold">現在の所持金</param>
        public bool Comparison()
        {
            // 現在の所持金が必要金額以上かどうかを比較する
            if (Funds.CurrentGold >= RequireAmount)
            {
                return true;
            }

            return false;
        }

        public void SelectedSlot()
        {
            defaultSlotColor = addSlot.GetComponent<Image>().color;
            addSlot.GetComponent<Image>().color = selectedSlotColor;
            settingSlot.GetComponent<Image>().color = selectedSlotColor;
        }

        public void DeselectSlot()
        {
            addSlot.GetComponent<Image>().color = defaultSlotColor;
            settingSlot.GetComponent<Image>().color = defaultSlotColor;
        }

        public void LockSlot()
        {
            addSlot.gameObject.SetActive(false);
            lockedSlot.gameObject.SetActive(true);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = 
                "Required amount : " + RequireAmount.ToString("#,###");

            lockedSlot.interactable = Comparison();
            ChangeSlotState(SkillSlotState.Locked);
        }

        public void UnlockSlot()
        {
            addSlot.gameObject.SetActive(true);
            lockedSlot.gameObject.SetActive(false);
            settingSlot.gameObject.SetActive(false);
            ChangeSlotState(SkillSlotState.Unlocked);
        }

        public void SetSkillData(SkillData skillData)
        {
            DeselectSlot();
            skillName.text = skillData.SkillName;
            skillCost.text = skillData.Cost.ToString();

            addSlot.gameObject.SetActive(false);
            lockedSlot.gameObject.SetActive(false);
            settingSlot.gameObject.SetActive(true);
            ChangeSlotState(SkillSlotState.Set);
        }
    }
}