namespace UI
{
    using TMPro;
    using UnityEngine;
    
    /// <summary>
    /// AIリファクタリング: Presenterから指示されたスキル説明を表示するView。
    /// </summary>
    public class SkillDescriptionHandler : CurrentKaomojiEventListener
    {
        [SerializeField] TextMeshProUGUI skillDescriptionText;

        void Awake()
        {
            Clear();
        }

        // [設計変更] 顔文字変更時の説明クリアだけを担当し、購読処理は基底クラスに委譲する。
        protected override void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update)
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