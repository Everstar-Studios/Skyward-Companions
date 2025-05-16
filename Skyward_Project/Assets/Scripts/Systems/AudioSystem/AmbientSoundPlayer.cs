using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSoundPlayer : MonoBehaviour, ISkywardComponent
{
    public AudioAsset audioAsset;
    private AudioSource source;

    private void Awake()
    {
        if (CanPlayAutomatically())
            audioAsset.Play(transform.position);
    }

    protected virtual bool CanPlayAutomatically() => true;
    
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
