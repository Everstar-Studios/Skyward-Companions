using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Skyward/Configs/Player Config", fileName = "PlayerConfig")]
public class PlayerConfig : BaseConfig
{
    public float heightConversionFactor = 1.5f;
    public AudioClip[] jumpSounds;
    [FormerlySerializedAs("jumpLandingSounds")] public AudioClip[] softLandingSounds;
    public AudioClip[] hardLandingSounds;
    public AudioClip[] deathSoudns;

    public void PlayRandomJumpSound()
    {
        if (jumpSounds != null && jumpSounds.Length > 0)
            AudioSystem.PlayRandomSFX(jumpSounds);
    }

    public void PlayRandomSoftLandingSound()
    {
        if (softLandingSounds != null && softLandingSounds.Length > 0)
            AudioSystem.PlayRandomSFX(softLandingSounds);
    }

    public void PlayRandomHardLandingSound()
    {
        if (hardLandingSounds != null && hardLandingSounds.Length > 0)
            AudioSystem.PlayRandomSFX(hardLandingSounds);
    }

    public void PlayRandomDeathSound()
    {
        if (deathSoudns != null && deathSoudns.Length > 0)
            AudioSystem.PlayRandomSFX(deathSoudns);
    }
}
