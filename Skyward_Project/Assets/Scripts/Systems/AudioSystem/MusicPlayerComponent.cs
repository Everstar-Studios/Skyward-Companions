using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSoundPlayer : MonoBehaviour
{
    public AudioAsset audioAsset;
    protected AudioInstance audioInstance;

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioSystem.Instance != null);
        audioInstance = AudioSystem.CreateAudioInstance(audioAsset, gameObject);
        if (CanPlayAutomatically())
            audioInstance?.Play();    
    }

    protected virtual bool CanPlayAutomatically() => true;
}
