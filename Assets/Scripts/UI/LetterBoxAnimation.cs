namespace UI
{
    using UnityEngine;

    public interface ILetterBox
    {
        void ShowLetterBox();
        void Open();
        void Close();
        void AllOpen();
    }

    public class LetterBoxAnimation : MonoBehaviour, ILetterBox
    {
        [SerializeField] Animator anim;
        [SerializeField] CanvasGroup canvasGroup;

        public const string ShowTrigger = "Show";
        public const string OpenTrigger = "Open";
        public const string CloseTrigger = "Close";
        public const string AllOpenTrigger = "AllOpen";

        void Awake()
        {
            ApiProvider.Register<ILetterBox>(this);
        }

        void OnDestroy()
        {
            ApiProvider.Unregister<ILetterBox>();
        }

        public void ShowLetterBox()
        {
            anim.SetTrigger(ShowTrigger);
            BlockInput(false);
        }

        public void Open()
        {
            anim.SetTrigger(OpenTrigger);
            BlockInput(false);
        }

        public void Close()
        {
            anim.SetTrigger(CloseTrigger);
            BlockInput(true);
        }

        public void AllOpen()
        {
            anim.SetTrigger(AllOpenTrigger);
            BlockInput(false);
        }

        void BlockInput(bool block)
        {
            canvasGroup.blocksRaycasts = block;
            canvasGroup.interactable = block;
        }
    }
}