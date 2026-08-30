namespace UI
{
    using UnityEngine;
    
    /// <summary>
    /// スキルの説明を管理するハンドラークラスです。
    /// このクラスは、スキルの説明をUIに設定するためのメソッドを提供します。
    /// </summary>
    public class SkillDescriptionHandler : MonoBehaviour
    {
        [SerializeField] SkillDescriptionUI skillDescriptionUI;
        [SerializeField] Transform skillDescriptionParent;

        public static System.Action<SkillData, int> OnSkillDescriptionUpdated;

        void Awake()
        {
            Clear();
            OnSkillDescriptionUpdated += SetSkillDescription;
        }

        void OnDestroy()
        {
            OnSkillDescriptionUpdated -= SetSkillDescription;
        }

        /// <summary>
        /// 指定されたスキルデータと現在のレベルに基づいて、スキルの説明をUIに設定します。
        /// </summary>
        public void SetSkillDescription(SkillData skillData, int currentLevel)
        {
            Clear();
            
            for(int i = 0; i < skillData.MaxLevel(); i++)
            {
                SkillDescriptionUI ui = Instantiate(skillDescriptionUI, skillDescriptionParent);
                ui.SetSkillDescription(skillData, currentLevel, i);
            }
        }

        void Clear()
        {
            foreach (Transform child in skillDescriptionParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}