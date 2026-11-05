using UnityEngine;

public interface IAudioManager
{
    void PlaySE(SEAudioName name);
    void PlayBGM(BGMName name);
    void PlayReflectSE();
    void StopBGM();
}

public class AudioManager : SceneSingleton<AudioManager>, IAudioManager
{
    [SerializeField] AudioDatabase audioDatabase;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    protected override bool PersistAcrossScenes => true;

    protected override void OnSingletonAwake() => DontDestroyOnLoad(gameObject);

    public void PlaySE(SEAudioName name)
    {
        if(name == SEAudioName.None) return;

        AudioClip clip = audioDatabase.GetSEClip(name);
        if (clip != null) seSource.PlayOneShot(clip);
    }

    public void PlayBGM(BGMName name)
    {
        if(name == BGMName.None) return;

        AudioClip clip = audioDatabase.GetBGMClip(name);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void PlayReflectSE()
    {
        AudioClip clip = audioDatabase.GetReflectClip();
        if (clip != null) seSource.PlayOneShot(clip);
    }
}