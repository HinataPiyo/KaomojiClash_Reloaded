namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SkillNameButton : SkillUIElement
    {
        Button button;
        public event System.Action<SkillData> OnSkillButtonClicked;

        void Awake()
        {
            button = GetComponent<Button>();
            button?.onClick.AddListener(() => ButtonOnClick());
        }

        public void Interactable(bool interactable)
        {
            button.interactable = interactable;
        }

        void ButtonOnClick()
        {
            ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.ButtonClick_00);
            OnSkillButtonClicked?.Invoke(SkillData);
        }
    }

    public class SkillUIElement : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI skillName;
        [SerializeField] protected TextMeshProUGUI skillCost;
        public SkillData SkillData { get; private set; }

        public virtual void SetSkill(SkillData skillData)
        {
            SkillData = skillData;

            skillName.text = skillData.SkillName;
            skillCost.text = skillData.Cost.ToString();
        }
    }
}