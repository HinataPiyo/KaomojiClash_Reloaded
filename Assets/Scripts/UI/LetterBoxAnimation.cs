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
        }

        public void Open()
        {
            anim.SetTrigger(OpenTrigger);
        }

        public void Close()
        {
            anim.SetTrigger(CloseTrigger);
        }

        public void AllOpen()
        {
            anim.SetTrigger(AllOpenTrigger);
        }
    }
}