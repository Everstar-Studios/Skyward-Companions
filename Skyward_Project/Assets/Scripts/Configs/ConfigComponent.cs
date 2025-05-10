using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfigComponent : MonoBehaviour
{
    [SerializeField, AssetReferenceUILabelRestriction("Config")]
    private List<BaseConfig> configs;
    public IEnumerable<BaseConfig> Configs => configs;
}
