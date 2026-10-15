namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        public static System.Action OnSkillNameListUpdated;

        void Awake()
        {
            Clear();

            OnSkillNameListUpdated += SetSkillNameList;
        }

        void OnDestroy()
        {
            OnSkillNameListUpdated -= SetSkillNameList;
        }

        /// <summary>
        /// 付与されているスキルの名前リストを表示する
        /// </summary>
        public void SetSkillNameList()
        {
            Clear();

            foreach (var skillData in GlobalVariables.HasSkills)
            {
                SkillNameButton button = Instantiate(skillNameButtonPrefab, skillNameButtonParent);
                button.SetSkill(skillData);
            }
        }

        void Clear()
        {
            foreach (Transform child in skillNameButtonParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}