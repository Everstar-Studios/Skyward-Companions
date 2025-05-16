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
}
