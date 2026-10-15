namespace UI
{
    using UnityEngine;
    
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        [SerializeField] SymbolListHandler symbolListHandler;
        [SerializeField] SkillDescriptionHandler skillDescriptionHandler;
        [SerializeField] SkillSlotsController skillSlotsCTRL;

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

                    // *スロットの中にスキルを追加しようとしていたら
                    if(skillSlotsCTRL.SelectedSlot != null)
                    {
                        // 選択したスロットにスキルを設定する
                        skillSlotsCTRL.SelectedSlot.SetSkillData(skillData);
                        skillSlotsCTRL.ClearSelectedSlot();     // 選択されていない状態に戻す
                    }
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