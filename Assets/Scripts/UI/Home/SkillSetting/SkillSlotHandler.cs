namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Player;

    public class SkillSlotHandler : MonoBehaviour
    {
        [SerializeField] GameObject activeSlot;
        [SerializeField] GameObject lockedSlot;
        float requireAmount;        // スロットをアンロックするために必要な金額

        /// <summary>
        /// スロットを初期化する
        /// </summary>
        /// <param name="amount">スロットをアンロックするために必要な金額</param>
        public void Initialize(float amount)
        {
            UnlockSlot();
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
            activeSlot.SetActive(false);
            lockedSlot.SetActive(true);

            lockedSlot.GetComponentInChildren<TextMeshProUGUI>().text = 
                "Required amount : " + requireAmount.ToString("#,###");

            bool isComplete = Funds.CurrentGold >= requireAmount;
            lockedSlot.GetComponentInChildren<Button>().interactable = isComplete;
        }

        public void UnlockSlot()
        {
            activeSlot.SetActive(true);
            lockedSlot.SetActive(false);
        }

        public void SetSkillData(SkillData skillData)
        {
            
        }
    }
}