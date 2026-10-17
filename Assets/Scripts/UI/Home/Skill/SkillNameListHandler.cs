namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        [SerializeField] SymbolListHandler symbolListHandler;
        [SerializeField] SkillDescriptionHandler skillDescriptionHandler;
        [SerializeField] SkillSlotsController skillSlotsCTRL;
        [SerializeField] CurrentSymbolHandler currentSymbolHandler;

        // テスト
        [SerializeField] WallReflection testSkillData;

        List<SkillNameButton> skillNameButtons = new List<SkillNameButton>();

        void Awake()
        {
            SkillNameButton[] existingButtons = skillNameButtonParent.GetComponentsInChildren<SkillNameButton>();
            skillNameButtons.AddRange(existingButtons);
            Clear();

            symbolListHandler.OnSelectedSymbolChanged += SetSkillNameList;
            skillSlotsCTRL.OnSlotStateChanged += CanInteractWithCost;
            if(!GlobalVariables.HasSkills.Contains(testSkillData))
                GlobalVariables.HasSkills.Add(testSkillData);
        }

        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= SetSkillNameList;
            skillSlotsCTRL.OnSlotStateChanged -= CanInteractWithCost;
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
                skillNameButtons.Add(button);
                button.OnSkillButtonClicked += (skillData) => 
                {
                    // ここにスキルボタンがクリックされたときの処理を記述
                    skillDescriptionHandler.SetSkillDescription(skillData);

                    // *スロットの中にスキルを追加しようとしていたら
                    if(skillSlotsCTRL.SelectedSlot != null)
                    {
                        // 選択したスロットにスキルを設定する
                        skillSlotsCTRL.SelectedSlot.SetSkillData(skillData);
                        currentSymbolHandler.AddOrUpdateSkillData(skillSlotsCTRL.SelectedSlot.SlotIndex, skillData);
                        skillSlotsCTRL.ClearSelectedSlot();     // 選択されていない状態に戻す
                        skillSlotsCTRL.SlotUsageByCost(currentSymbolHandler.CurrentSymbolData);
                    }
                };

                button.SetSkill(skillData);
            }

            CanInteractWithCost();
        }

        /// <summary>
        /// スキルのコストに応じてボタンの操作可否を更新する
        /// </summary>
        void CanInteractWithCost()
        {
            foreach (var button in skillNameButtons)
            {
                if (button == null || button.SkillData == null)
                    continue;

                // スキルのコストに応じてボタンの操作可否を設定する
                button.Interactable(skillSlotsCTRL.CanPlaceSkill(button.SkillData));
            }
        }

        void Clear()
        {
            foreach (var button in skillNameButtons)
            {
                Destroy(button.gameObject);
            }

            skillNameButtons.Clear();
        }
    }
}