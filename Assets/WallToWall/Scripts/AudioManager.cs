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

    private bool _isSfxOn = true;
    private bool _isBgmOn = true;

    void Awake()
    {
        _sfxClips.Clear();
        _isSfxOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        IsBgmOn = PlayerPrefs.GetInt("Music", 1) == 1;

        //load audio clips online

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
        if (!IsBgmOn)
        {
            bgmSource.volume = 0;
        }
    }

    public bool IsBgmOn
    {
        get => _isBgmOn = PlayerPrefs.GetInt("Music", 1) == 1;
        set => _isBgmOn = value;
    }

    public void PlayBGM(string key, bool isLoop = true, float volume = 1)
    {
        if (!IsBgmOn) return;
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
        if (!_isSfxOn) return;
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
        if (isOn) PlayBGM("BGM_MENU", volume: 0.3f);
    }

    public void SetBGMSlow()
    {
        if (!IsBgmOn) return;
        bgmSource.Stop();
    }

    public void SetBGMNormal()
    {
        if (!IsBgmOn) return;
        bgmSource.Play();
    }
}