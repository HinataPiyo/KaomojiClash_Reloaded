namespace UI
{
    using UnityEngine;
    
    public interface IStartWaveAnimationUI
    {
        void ShowStartWaveAnimation();
        void HideStartWaveAnimation();
    }

    public class StartWaveAnimationUI : SceneSingleton<StartWaveAnimationUI>, IStartWaveAnimationUI
    {

        protected override void OnSingletonAwake()
        {
        }

        void Start()
        {
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

        public void StartReadySE()
        {
            AudioManager.Instance.PlaySE(SEAudioName.Ready);
        }

        public void StartFightSE()
        {
            AudioManager.Instance.PlaySE(SEAudioName.Fight);
        }
    }
}