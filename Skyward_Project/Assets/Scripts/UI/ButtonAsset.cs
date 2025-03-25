using UnityEngine;
using UnityEngine.UIElements;

namespace Skyward.UI
{
    public abstract class ButtonAsset : UIOption
    {
        public bool executeOnlyOnce = true;
        public AudioClip clickSound;
        public VisualTreeAsset buttonElementAsset;
        public string text = "Button";

        public virtual void Action()
        {
            if (clickSound != null)
                AudioSystem.Play(clickSound);
        }

        public override VisualElement GetVisualElement()
        {
            VisualElement buttonRoot = buttonElementAsset.Instantiate().Q("Root");
            var label = buttonRoot.Q<Label>();
            if (label != null) 
                label.text = text;
            
            buttonRoot.RegisterCallback<MouseUpEvent>(Execute);
            buttonRoot.focusable = true;
            
            return buttonRoot;
        }
        
        internal void Execute(EventBase evt)
        {
            Action();
            if (executeOnlyOnce)
                ((VisualElement)evt.target).UnregisterCallback<MouseUpEvent>(Execute);
        }
    }
}

