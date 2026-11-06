namespace UI
{
    using TMPro;
    using UnityEngine;
    
    /// <summary>
    /// AIリファクタリング: Presenterから指示されたスキル説明を表示するView。
    /// </summary>
    public class SkillDescriptionHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI skillDescriptionText;

        void Awake()
        {
            Clear();
        }

        public void SetSkillDescription(SkillData skillData)
        {
            skillDescriptionText.text = skillData == null ? string.Empty : skillData.GetDescription();
        }

        public void Clear()
        {
            skillDescriptionText.text = string.Empty;
        }
    }
}