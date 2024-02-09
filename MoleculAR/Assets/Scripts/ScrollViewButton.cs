using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollViewButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private MenuScrollView menuScrollView;
    private int buttonIndex = -1;
    private string elementName = "";
    private string elementId = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetButtonData(MenuScrollView menuScrollView, int buttonIndex, string elementName, string elementId)
    {
        this.menuScrollView = menuScrollView;
        this.buttonIndex = buttonIndex;
        this.buttonText.SetText(elementName);
        this.elementName = elementName;
        this.elementId = elementId;
    }

    public void ButtonPressed()
    {
        if (this.menuScrollView != null)
        {
            this.menuScrollView.SelectedButtonUpdate(this.buttonIndex, this.elementId);
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
