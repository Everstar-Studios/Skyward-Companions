using UnityEngine;
using UnityEngine.UIElements;

namespace Skyward.UI
{
    public abstract class UIAsset : ScriptableObject
    {
        public abstract VisualElement GetVisualElement();
    }
}