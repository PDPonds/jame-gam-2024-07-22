using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;

    private void Awake()
    {
        GeneratAudioSource();
    }

    private void Start()
    {
        Play("BGS");
    }

    void GeneratAudioSource()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.audioGroup;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null) return;
        if (!s.source.isPlaying) s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null) return;
        if (s.source.isPlaying) s.source.Pause();
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null) return;
        s.source.PlayOneShot(s.clip);
    }


}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup audioGroup;
    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}