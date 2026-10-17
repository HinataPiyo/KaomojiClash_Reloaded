namespace UI
{
    using TMPro;
    using UnityEngine;
    
    /// <summary>
    /// スキルの説明を管理するハンドラークラスです。
    /// このクラスは、スキルの説明をUIに設定するためのメソッドを提供します。
    /// </summary>
    public class SkillDescriptionHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI skillDescriptionText;

        public static System.Action<SkillData> OnSkillDescriptionUpdated;

        public static System.Action OnSkillDescriptionCleared;

        void Awake()
        {
            Clear();
            OnSkillDescriptionUpdated += SetSkillDescription;
            OnSkillDescriptionCleared += Clear;
        }

        void OnDestroy()
        {
            OnSkillDescriptionUpdated -= SetSkillDescription;
            OnSkillDescriptionCleared -= Clear;
        }

        /// <summary>
        /// 指定されたスキルデータと現在のレベルに基づいて、スキルの説明をUIに設定します。
        /// </summary>
        public void SetSkillDescription(SkillData skillData)
        {
            Clear();
            skillDescriptionText.text = skillData.GetDescription();
        }

        void Clear()
        {
            skillDescriptionText.text = string.Empty;
        }
    }
}