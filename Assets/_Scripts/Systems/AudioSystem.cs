using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : Systems
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip, float vol = 1)
    {
        _soundSource.PlayOneShot(clip, vol);
    }

}
