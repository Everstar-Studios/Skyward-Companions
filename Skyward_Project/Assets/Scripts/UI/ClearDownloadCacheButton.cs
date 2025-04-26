using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Skyward.UI
{
    public class ClearDownloadCacheButton : UIButton
    {
        public override void OnClick()
        {
            base.OnClick();

            foreach (var levelLabel in ConfigSystem.GetConfig<LevelConfig>().levels)
            {
                Addressables.ClearDependencyCacheAsync(levelLabel.sceneLabel.labelString);
            }
        }
    }
}

