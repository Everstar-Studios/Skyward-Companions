using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSoundPlayer : MonoBehaviour, ISkywardComponent
{
    public AudioClip clip;
    private AudioSource source;

    protected virtual bool CanPlayAutomatically() => true;

    private void Awake()
    {
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        if (CanPlayAutomatically())
            source.Play();
    }
    
    public void Pause()
    {
        source.Pause();
    }

    public void Unpause()
    {
        source.UnPause();
    }

    protected void Stop()
    {
        source.Stop();
    }
}
