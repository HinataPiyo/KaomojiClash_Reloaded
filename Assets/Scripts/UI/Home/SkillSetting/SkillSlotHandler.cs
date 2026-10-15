namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;

    public class SkillSlotHandler : MonoBehaviour
    {
        [SerializeField] GameObject addSlot;
        [SerializeField] GameObject lockedSlot;
        [SerializeField] GameObject settingSlot;

        [Header("Skill Info")]
        [SerializeField] TextMeshProUGUI skillName;
        [SerializeField] TextMeshProUGUI skillCost;
        
        SkillData skillData;
        float requireAmount;        // スロットをアンロックするために必要な金額

        /// <summary>
        /// スロットを初期化する
        /// </summary>
        /// <param name="amount">スロットをアンロックするために必要な金額</param>
        public void Initialize(float amount)
        {
            LockSlot();
            SetRequireAmount(amount);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = "-";
            lockedSlot.GetComponentInChildren<Button>().interactable = false;
        }

        void SetRequireAmount(float amount)
        {
            requireAmount = amount;
        }

        /// <summary>
        /// スロットのアンロック条件を現在の所持金と比較する
        /// </summary>
        /// <param name="currentGold">現在の所持金</param>
        public bool Comparison(float currentGold)
        {
            // 現在の所持金が必要金額以上かどうかを比較する
            if (currentGold >= requireAmount)
            {
                return true;
            }

            return false;
        }

        public void LockSlot()
        {
            addSlot.SetActive(false);
            lockedSlot.SetActive(true);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = 
                "Required amount : " + requireAmount.ToString("#,###");

            bool isComplete = Funds.CurrentGold >= requireAmount;
            lockedSlot.GetComponentInChildren<Button>().interactable = isComplete;
        }

        public void UnlockSlot()
        {
            addSlot.SetActive(true);
            lockedSlot.SetActive(false);
            settingSlot.SetActive(false);
        }

        public void SetSkillData(SkillData skillData)
        {
            this.skillData = skillData;
            skillName.text = skillData.SkillName;
            skillCost.text = skillData.Cost.ToString();

            addSlot.SetActive(false);
            lockedSlot.SetActive(false);
            settingSlot.SetActive(true);
        }
    }
}