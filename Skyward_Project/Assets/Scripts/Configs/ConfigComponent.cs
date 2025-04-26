using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfigComponent : MonoBehaviour
{
    [SerializeField, AssetReferenceUILabelRestriction("Config")]
    private List<AssetReference> configs;
    public IEnumerable<AssetReference> Configs => configs;
}
