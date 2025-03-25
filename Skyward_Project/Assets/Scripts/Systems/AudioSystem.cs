using Skyward.Core;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSystem : BaseSystem<AudioSystem>
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        
        audioSource = GetComponent<AudioSource>();
    }
    
    public static void Play(AudioClip clip)
    {
        Instance.audioSource.PlayOneShot(clip);
    }
}
