using System.Collections;
using Skyward.Core;
using Skyward.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIGameMenu : BaseSystem<UIGameMenu>
{
    private UIDocument uiDocument;

    protected override void Awake()
    {
        base.Awake();

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
    }

    public static void ShowUI(UIAsset asset)
    {
        VisualElement mainMenuSlabElement = asset.GetVisualElement();
        VisualElement root = mainMenuSlabElement.Q(name: "Root");
        VisualElement newRoot = new VisualElement();
        newRoot.name = asset.name;
        newRoot.style.position = Position.Absolute;
        newRoot.style.width = new Length(100f, LengthUnit.Percent);
        newRoot.style.height = new Length(100f, LengthUnit.Percent);
        newRoot.Add(root);
        Instance.uiDocument.rootVisualElement.Add(newRoot);
        
        if (EventSystem.current != null)
        {
            GameObject panelObject = GameObject.Find(Instance.uiDocument.panelSettings.name);
            EventSystem.current.SetSelectedGameObject(panelObject);
        }
    }
}
