using UnityEngine;

[CreateAssetMenu(menuName = "Skyward/Configs/Player Config", fileName = "PlayerConfig")]
public class PlayerConfig : BaseConfig
{
    public float heightConversionFactor = 1.5f;
    public AudioAsset jumpSound;
    public AudioAsset softLandingSound;
    public AudioAsset hardLandingSound;
    public AudioAsset deathSound;
}
