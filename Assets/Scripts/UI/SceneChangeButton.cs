namespace UI
{
    using UnityEngine;
    
    public class SceneChangeButton : MonoBehaviour
    {
        [SerializeField] SceneName targetScene;

        public void OnClickChangeScene()
        {
            SceneChange.Instance.ChangeScene(targetScene);
        }
    }
}