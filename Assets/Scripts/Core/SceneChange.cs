using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneChange
{
    void ChangeScene(SceneName sceneName);
    SceneName CurrentScene { get; }
}

public enum SceneName { Title, Home, Battle }
public class SceneChange : MonoBehaviour, ISceneChange
{
    static SceneChange I;
    SceneName beforeScene;
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

        CurrentScene = SceneManager.GetActiveScene().name switch
        {
            "TitleScene" => SceneName.Title,
            "HomeScene" => SceneName.Home,
            "BattleScene" => SceneName.Battle,
            _ => throw new System.Exception("Unknown scene name")
        };
        beforeScene = CurrentScene;
    }

    public void ChangeScene(SceneName sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        beforeScene = CurrentScene;
        ApiProvider.Get<ILetterBox>().Close();
        yield return new WaitForSeconds(1.0f);

        ApiProvider.Get<IAudioManager>().PlayBGM(sceneName switch
        {
            SceneName.Title => BGMName.None,
            SceneName.Home => BGMName.Home,
            SceneName.Battle => BGMName.None,
            _ => throw new System.Exception("Unknown scene name")
        });

        // これは非同期でシーンをロードするための処理です。シーンのロードが完了するまで待機します。
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameToString[sceneName]);
        CurrentScene = sceneName;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        LetterBoxAnimation();
        Debug.Log($"<color=green>Scene changed to {sceneName}</color>");
    }

    void LetterBoxAnimation()
    {
        if(CurrentScene == SceneName.Battle)
        {
            ApiProvider.Get<ILetterBox>().AllOpen();
        }
        else
        {
            ApiProvider.Get<ILetterBox>().Open();
        }
    }
}