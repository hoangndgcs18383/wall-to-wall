using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public struct AudioData
{
    public string key;
    public AudioClip clip;
}

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioData[] sfxClips;
    [SerializeField] private AudioData[] backgroundClips;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    private Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _backgroundClips = new Dictionary<string, AudioClip>();

    void Awake()
    {
        _sfxClips.Clear();
        foreach (var audioData in sfxClips)
        {
            _sfxClips.Add(audioData.key, audioData.clip);
        }

        _backgroundClips.Clear();
        foreach (var audioData in backgroundClips)
        {
            _backgroundClips.Add(audioData.key, audioData.clip);
        }

        PlayBGM("BGM_FIRST_SCREEN", volume: 0.3f);
    }

    public void PlayBGM(string key, bool isLoop = true, float volume = 1)
    {
        if (_backgroundClips.ContainsKey(key))
        {
            bgmSource.clip = _backgroundClips[key];
            bgmSource.loop = isLoop;
            bgmSource.volume = 0f;
            bgmSource.DOFade(volume, 1f);
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.DOFade(0f, 1f).OnComplete(() => { bgmSource.Stop(); });
    }

    public void PlaySfx(string key)
    {
        if (_sfxClips.ContainsKey(key))
        {
            sfxSource.PlayOneShot(_sfxClips[key]);
        }
    }
    
    public void SetOnOffSfx(bool isOn)
    {
        sfxSource.mute = !isOn;
    }
    
    public void SetOnOffBGM(bool isOn)
    {
        bgmSource.mute = !isOn;
    }
}