using Skyward.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Skyward.UI
{
    [CreateAssetMenu(fileName = "RootAsset", menuName = "Skyward/UI/Root")]
    public class RootAsset : UIAsset
    {
        public VisualTreeAsset rootElementAsset;
        
        public override VisualElement GetVisualElement()
        {
            return rootElementAsset.Instantiate();
        }
    }

}
