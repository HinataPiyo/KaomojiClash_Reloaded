namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;

    public enum SkillSlotState { Locked, Unlocked, Set }
    public class SkillSlotHandler : MonoBehaviour
    {
        [SerializeField] Button addSlot;
        [SerializeField] Button lockedSlot;
        [SerializeField] Button settingSlot;

        [Header("Skill Info")]
        [SerializeField] TextMeshProUGUI skillName;
        [SerializeField] TextMeshProUGUI skillCost;

        [Header("Selected Slot")]
        [SerializeField] Color selectedSlotColor;
        Color defaultAddSlotColor;
        Color defaultSettingSlotColor;
    
        public float RequireAmount { get; private set; }        // スロットをアンロックするために必要な金額
        public int SlotIndex { get; private set; }

        /// <summary>
        /// スロットを初期化する
        /// </summary>
        /// <param name="amount">スロットをアンロックするために必要な金額</param>
        public void Initialize(int slotIndex, float amount, System.Action<SkillSlotHandler> onClickLockedSlot, System.Action<SkillSlotHandler> onClickAddSlot)
        {
            SlotIndex = slotIndex;
            defaultAddSlotColor = addSlot.GetComponent<Image>().color;
            defaultSettingSlotColor = settingSlot.GetComponent<Image>().color;
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
            addSlot.GetComponent<Image>().color = selectedSlotColor;
            settingSlot.GetComponent<Image>().color = selectedSlotColor;
        }

        public void DeselectSlot()
        {
            addSlot.GetComponent<Image>().color = defaultAddSlotColor;
            settingSlot.GetComponent<Image>().color = defaultSettingSlotColor;
        }

        public void LockSlot()
        {
            addSlot.gameObject.SetActive(false);
            lockedSlot.gameObject.SetActive(true);
            settingSlot.gameObject.SetActive(false);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = 
                "Required amount : " + RequireAmount.ToString("#,###");

            lockedSlot.interactable = Comparison();
        }

        public void UnlockSlot()
        {
            addSlot.gameObject.SetActive(true);
            lockedSlot.gameObject.SetActive(false);
            settingSlot.gameObject.SetActive(false);
        }

        public void SetSkillData(SkillData skillData)
        {
            DeselectSlot();
            skillName.text = skillData.SkillName;
            skillCost.text = skillData.Cost.ToString();

            addSlot.gameObject.SetActive(false);
            lockedSlot.gameObject.SetActive(false);
            settingSlot.gameObject.SetActive(true);
        }
    }
}