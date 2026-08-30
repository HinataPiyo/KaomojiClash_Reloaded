namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        public static System.Action<Dictionary<SkillData, int>> OnSkillNameListUpdated;

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
        public void SetSkillNameList(Dictionary<SkillData, int> allSkillsWithLevels)
        {
            Clear();

            foreach (var skillWithLevel in allSkillsWithLevels)
            {
                SkillNameButton button = Instantiate(skillNameButtonPrefab, skillNameButtonParent);
                button.SetSkill(skillWithLevel.Key, skillWithLevel.Value);
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