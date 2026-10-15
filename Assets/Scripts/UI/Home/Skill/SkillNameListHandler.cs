namespace UI
{
    using UnityEngine;
    
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        [SerializeField] SymbolListHandler symbolListHandler;
        [SerializeField] SkillDescriptionHandler skillDescriptionHandler;

        // テスト
        [SerializeField] WallReflection testSkillData;

        void Awake()
        {
            Clear();

            symbolListHandler.OnSelectedSymbolChanged += SetSkillNameList;
            if(!GlobalVariables.HasSkills.Contains(testSkillData))
                GlobalVariables.HasSkills.Add(testSkillData);
        }

        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= SetSkillNameList;
        }

        /// <summary>
        /// 付与されているスキルの名前リストを表示する
        /// </summary>
        public void SetSkillNameList(SymbolData symbolData)
        {
            Clear();

            foreach (var skillData in GlobalVariables.HasSkills)
            {
                SkillNameButton button = Instantiate(skillNameButtonPrefab, skillNameButtonParent);
                button.OnSkillButtonClicked += (skillData) => 
                {
                    // ここにスキルボタンがクリックされたときの処理を記述
                    skillDescriptionHandler.SetSkillDescription(skillData);
                };

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