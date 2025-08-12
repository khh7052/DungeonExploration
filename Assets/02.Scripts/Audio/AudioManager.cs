using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    private AudioMixer audioMixer;
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    protected override void Initialize()
    {
        base.Initialize();

        audioMixer = Resources.Load<AudioMixer>("AudioMixer");

        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer not found in Resources folder.");
            return;
        }

        CreateAudioSource(SoundType.BGM, ref bgmSource);
        CreateAudioSource(SoundType.SFX, ref sfxSource);
    }

    private void CreateAudioSource(SoundType soundType, ref AudioSource audioSource)
    {
        GameObject sourceObject = new(soundType.ToString());
        sourceObject.transform.SetParent(transform, false); // AudioManager의 자식으로 설정
        audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(soundType.ToString())[0];
        audioSource.loop = (soundType == SoundType.BGM); // SFX는 반복 재생하지 않음
        audioSource.playOnAwake = false; // 자동 재생 방지
        audioSource.volume = 1f; // 기본 볼륨 설정
    }


    public void PlayBGM(SoundData soundData)
    {
        if (soundData == null || soundData.clips.Length == 0) return;

        AudioClip clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        PlayBGM(clip, soundData.volume);
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void PlaySFX(SoundData soundData)
    {
        if (soundData == null || soundData.clips.Length == 0) return;

        AudioClip clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        PlaySFX(clip, soundData.volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

}
