namespace UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// AIリファクタリング: スキル一覧の生成・表示と選択入力だけを担当するView。
    /// </summary>
    public class SkillNameListHandler : MonoBehaviour
    {
        [SerializeField] SkillNameButton skillNameButtonPrefab;
        [SerializeField] Transform skillNameButtonParent;

        readonly List<SkillNameButton> skillNameButtons = new List<SkillNameButton>();

        public event Action<SkillData> OnSkillSelected;

        void Awake()
        {
            skillNameButtons.AddRange(skillNameButtonParent.GetComponentsInChildren<SkillNameButton>());
            Clear();
        }

        void OnDestroy()
        {
            Clear();
        }

        public void SetSkillNameList(IEnumerable<SkillData> skillDatas)
        {
            if (skillDatas == null)
                throw new ArgumentNullException(nameof(skillDatas));

            Clear();
            foreach (SkillData skillData in skillDatas)
            {
                if (skillData == null)
                    continue;

                SkillNameButton button = Instantiate(skillNameButtonPrefab, skillNameButtonParent);
                button.SetSkill(skillData);
                button.OnSkillButtonClicked += HandleSkillButtonClicked;
                skillNameButtons.Add(button);
            }
        }

        public void RefreshInteractability(Func<SkillData, bool> canSelectSkill)
        {
            if (canSelectSkill == null)
                throw new ArgumentNullException(nameof(canSelectSkill));

            foreach (SkillNameButton button in skillNameButtons)
            {
                if (button != null && button.SkillData != null)
                    button.Interactable(canSelectSkill(button.SkillData));
            }
        }

        void HandleSkillButtonClicked(SkillData skillData)
        {
            OnSkillSelected?.Invoke(skillData);
        }

        void Clear()
        {
            foreach (SkillNameButton button in skillNameButtons)
            {
                if (button == null)
                    continue;

                button.OnSkillButtonClicked -= HandleSkillButtonClicked;
                Destroy(button.gameObject);
            }

            skillNameButtons.Clear();
        }
    }
}
