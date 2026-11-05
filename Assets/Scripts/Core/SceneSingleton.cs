using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    public static T Instance { get; private set; }

    bool initialized;

    protected virtual bool PersistAcrossScenes => false;

    void Awake()
    {
        if (Instance != null && Instance != this &&
            (PersistAcrossScenes || Instance.PersistAcrossScenes || Instance.gameObject.scene == gameObject.scene))
        {
            Debug.LogWarning($"{typeof(T).Name} is duplicated in scene '{gameObject.scene.name}'.", this);
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = (T)this;
        initialized = true;
        OnSingletonAwake();
    }

    void OnDestroy()
    {
        if (!initialized) return;

        try
        {
            OnSingletonDestroy();
        }
        finally
        {
            if (Instance == this) Instance = null;
        }
    }

    protected virtual void OnSingletonAwake() { }
    protected virtual void OnSingletonDestroy() { }
}
