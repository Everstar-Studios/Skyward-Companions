using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Asset", menuName = "Skyward/Audio/Audio Asset")]
public class AudioAsset : ScriptableObject
{
    [SerializeField]
    private EventReference sound;

    internal EventReference SoundRef => sound;

    public void Play(Vector3 position = default)
    {
        AudioSystem.PlayOneShot(this, position);
    }
}
