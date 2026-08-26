namespace UI
{
    using UnityEngine;
    
    public interface IStartWaveAnimationUI
    {
        void ShowStartWaveAnimation();
        void HideStartWaveAnimation();
    }

    public class StartWaveAnimationUI : MonoBehaviour, IStartWaveAnimationUI
    {

        void Start()
        {
            ApiProvider.Register<IStartWaveAnimationUI>(this);
            HideStartWaveAnimation();
        }
        public void ShowStartWaveAnimation()
        {
            gameObject.SetActive(true);
        }

        public void HideStartWaveAnimation()
        {
            gameObject.SetActive(false);
        }
    }
}