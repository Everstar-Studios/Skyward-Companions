using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSoundPlayer : MonoBehaviour, ISkywardComponent
{
    public AudioResource audioResource;
    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.resource = audioResource;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
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
