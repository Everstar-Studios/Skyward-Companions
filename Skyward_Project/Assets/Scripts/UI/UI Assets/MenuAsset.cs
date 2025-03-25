using System.Collections.Generic;
using Skyward.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Skyward.UI
{
    [CreateAssetMenu(fileName = "MenuAsset", menuName = "Skyward/UI/Menus/Menu Asset")]
    public class MenuAsset : UIAsset
    {
        public VisualTreeAsset menuElementAsset;
        public string titleText;
        public string navigationElementName = "Navigation";
        public string footerElementName = "Footer";
        public List<UIOption> options;
        public string titleElementName = "Title";
        public string extraTextElementName = "ExtraText";
        public string overlayElementName = "Overlay";
        public override VisualElement GetVisualElement()
        {
            VisualElement menu = menuElementAsset.Instantiate();
            
            if (!string.IsNullOrEmpty(titleText))
            {
                Label titleLabel = menu.Q<Label>(name: titleElementName);
                titleLabel.text = titleText;
            }

            VisualElement navigation = menu.Q(navigationElementName);

            foreach (var option in options)
            {
                var element = option.GetVisualElement();
                navigation.Add(element);
            }

            return menu;
        }
    }
}
