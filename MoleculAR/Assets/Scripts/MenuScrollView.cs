using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuScrollView : MonoBehaviour
{
    public MoleculeSpawner moleculeSpawner;
    protected List<ScrollViewButton> buttonList = new List<ScrollViewButton>();
    public ScrollViewButton scrollViewButtonPrefab;
    private Color baseButtonColor;
    private Color selectedButtonColor = new Color(0.5f, 0.5f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        var imageRenderer = this.scrollViewButtonPrefab.GetComponent<Image>();

        this.baseButtonColor = imageRenderer.color;
    }

    public void SelectedButtonUpdate(int buttonSelectedId, string elementId)
    {
        for (int i = 0; i < this.buttonList.Count; i++)
        {
            ScrollViewButton button = this.buttonList[i];
            Image buttonImage = button.GetComponent<Image>();
            if (i == buttonSelectedId)
            {
                buttonImage.color = this.selectedButtonColor;
            }
            else
            {
                buttonImage.color = this.baseButtonColor;
            }
        }
        this.DoSelectedButtonAction(buttonSelectedId, elementId);
    }

    protected abstract void DoSelectedButtonAction(int buttonSelectedId, string elementId);

    protected void ClearScrollView()
    {
        foreach (var button in this.buttonList)
        {
            Destroy(button.gameObject);
        }
        buttonList.Clear();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
