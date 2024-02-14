using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void MenuScrollViewCallback(int buttonSelectedId, string elementId);

public class ScrollViewButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private MenuScrollViewCallback menuScrollViewCallback;
    private int buttonIndex = -1;
    private string elementName = "";
    private string elementId = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetButtonData<T>(MenuScrollView<T> menuScrollView, int buttonIndex, string elementName, string elementId) where T : ScrollViewButton
    {
        this.menuScrollViewCallback = menuScrollView.SelectedButtonUpdate;
        this.buttonIndex = buttonIndex;
        this.buttonText.SetText(elementName);
        this.elementName = elementName;
        this.elementId = elementId;
    }

    public void ButtonPressed()
    {
        if (this.menuScrollViewCallback != null)
        {
            this.menuScrollViewCallback(this.buttonIndex, this.elementId);
        }
    }

    public string GetElementId()
    {
        return this.elementId;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
