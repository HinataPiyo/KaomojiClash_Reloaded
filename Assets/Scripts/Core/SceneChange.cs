using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneChange
{
    void ChangeScene(SceneName sceneName);
    SceneName CurrentScene { get; }
}

public enum SceneName { Title, Home, Battle }
public class SceneChange : MonoBehaviour, ISceneChange
{
    SceneChange I;
    static readonly Dictionary<SceneName, string> sceneNameToString = new Dictionary<SceneName, string>
    {
        { SceneName.Title, "TitleScene" },
        { SceneName.Home, "HomeScene" },
        { SceneName.Battle, "BattleScene" }
    };
    public SceneName CurrentScene { get; private set; }

    void Awake()
    {
        if(I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        
        I = this;
        DontDestroyOnLoad(gameObject);
        ApiProvider.Register<ISceneChange>(this);
    }

    public void ChangeScene(SceneName sceneName)
    {
        CurrentScene = sceneName;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameToString[sceneName]);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log($"<color=green>Scene changed to {sceneName}</color>");
    }
}