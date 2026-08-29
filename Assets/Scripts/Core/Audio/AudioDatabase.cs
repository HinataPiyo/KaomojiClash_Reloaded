using UnityEngine;

public enum SEAudioName 
{ None = -1, ButtonClick_00, Cancel_00, Contact, Ready, Fight }

public enum BGMName
{ None = -1, Home, Battle_Fighting, Battle_Moving, }

public enum ReflectName
{ None = -1, Reflect_00, Reflect_01, Reflect_02 }

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "KaomojiClash_Reloaded/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] BGMClipData[] bgmClips;
    [SerializeField] SEClipData[] seClips;
    [SerializeField] ReflectClipData[] reflectClips;

    public AudioClip GetBGMClip(BGMName name)
    {
        foreach (var data in bgmClips)
        {
            if (data.name == name)
                return data.clip;
        }
        return null;
    }

    public AudioClip GetSEClip(SEAudioName name)
    {
        foreach (var data in seClips)
        {
            if (data.name == name)
                return data.clip;
        }
        return null;
    }

    public AudioClip GetReflectClip()
    {
        // ランダム選出
        ReflectName select = (ReflectName)Random.Range(0, 3);
        foreach (var data in reflectClips)
        {
            if (data.name == select)
                return data.clip;
        }
        return null;
    }
}

[System.Serializable]
public class SEClipData
{
    public SEAudioName name;
    public AudioClip clip;
}

[System.Serializable]
public class BGMClipData
{
    public BGMName name;
    public AudioClip clip;
}

[System.Serializable]
public class ReflectClipData
{
    public ReflectName name;
    public AudioClip clip;
}