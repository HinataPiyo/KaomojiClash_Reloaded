namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkillNameButton : SkillUIElement
    {
        [SerializeField] TextMeshProUGUI skillName;
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            button?.onClick.AddListener(() => ButtonOnClick());
        }

        void ButtonOnClick()
        {
            SkillDescriptionHandler.OnSkillDescriptionUpdated?.Invoke(SkillData, CurrentCost);
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
        public int CurrentCost { get; private set; }
        Image[] skillLevelIcons;

        public virtual void SetSkill(SkillData skillData, int currentLevel)
        {
            SkillData = skillData;
            CurrentCost = currentLevel;

            for (int i = 0; i < skillLevelIcons.Length; i++)
            {
                skillLevelIcons[i].gameObject.SetActive(i < skillData.MaxLevel());
                skillLevelIcons[i].color = i < CurrentCost ? Color.red : Color.gray;
            }
        }
    }
}