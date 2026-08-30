namespace UI
{
    using TMPro;
    using UnityEngine;
    
    public class SkillDescriptionUI : SkillUIElement
    {
        [SerializeField] TextMeshProUGUI skillDescriptionText;
        [SerializeField] CanvasGroup canvasGroup;

        /// <summary>
        /// スキルの説明を設定します。
        /// 現在のレベルが最大レベルを超えている場合、UIを半透明にします。
        /// </summary>
        public void SetSkillDescription(SkillData skillData, int currentLevel, int currentIndex)
        {
            if(currentIndex < currentLevel)
            {
                canvasGroup.alpha = 1f; // 通常の透明度にする
            }
            else
            {
                canvasGroup.alpha = 0.5f; // 半透明にする
            }

            SetSkill(skillData, currentIndex + 1);
            string description = skillData.GetDescription(currentIndex + 1); // レベルは1から始まるため、currentIndexに1を加える
            skillDescriptionText.text = description;
        }
    }
}