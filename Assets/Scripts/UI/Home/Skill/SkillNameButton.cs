namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkillNameButton : SkillUIElement
    {
        [SerializeField] TextMeshProUGUI skillName;
        Button button;

        protected override void Awake()
        {
            base.Awake();
            button = GetComponent<Button>();
            button?.onClick.AddListener(() => ButtonOnClick());
        }

        void ButtonOnClick()
        {
            SkillDescriptionHandler.OnSkillDescriptionUpdated?.Invoke(SkillData, CurrentLevel);
            ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.ButtonClick_00);
        }

        public override void SetSkill(SkillData skillData, int currentLevel)
        {
            base.SetSkill(skillData, currentLevel);
            skillName.text = skillData.SkillName;
        }
    }

    public class SkillUIElement : MonoBehaviour
    {
        public SkillData SkillData { get; private set; }
        public int CurrentLevel { get; private set; }
        [SerializeField] Transform skillLevelIconParent;
        Image[] skillLevelIcons;

        protected virtual void Awake()
        {
            skillLevelIcons = skillLevelIconParent.GetComponentsInChildren<Image>();
        }

        public virtual void SetSkill(SkillData skillData, int currentLevel)
        {
            SkillData = skillData;
            CurrentLevel = currentLevel;

            for (int i = 0; i < skillLevelIcons.Length; i++)
            {
                if (i < CurrentLevel)
                {
                    skillLevelIcons[i].color = Color.red;
                }
                else
                {
                    skillLevelIcons[i].color = Color.gray;
                }
            }
        }
    }
}