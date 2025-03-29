using System;
using Skyward.Core;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientSoundPlayer : MonoBehaviour, ISkywardComponent
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        AudioSystem.AddAmbientPlayer(this);
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
}
