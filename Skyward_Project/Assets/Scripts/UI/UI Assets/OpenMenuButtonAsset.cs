using Skyward.UI;
using UnityEngine;

namespace Skyward.UI
{
    [CreateAssetMenu(fileName = "OpenMenuButton", menuName = "Skyward/UI/Open Menu Button")]
    public class OpenMenuButtonAsset : ButtonAsset
    {
        public MenuAsset menuAsset;
        
        public override void Action()
        {
            base.Action();

            UIGameMenu.ShowUI(menuAsset);
        }
    }

}
