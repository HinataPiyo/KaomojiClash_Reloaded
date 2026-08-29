using UnityEngine;

public interface IAudioManager
{
    void PlaySE(SEAudioName name);
    void PlayBGM(BGMName name);
    void StopBGM();
}

public class AudioManager : MonoBehaviour, IAudioManager
{
    AudioManager I;

    [SerializeField] AudioDatabase audioDatabase;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            ApiProvider.Register<IAudioManager>(this);
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
}