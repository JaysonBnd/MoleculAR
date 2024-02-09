using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuScrollView<T>: MonoBehaviour where T: ScrollViewButton
{
    public MoleculeSpawner moleculeSpawner;
    protected List<T> buttonList = new List<T>();
    public T scrollViewButtonPrefab;
    public GameObject content;
    private Color baseButtonColor = new Color(0.9f, 0.9f, 0.9f);
    private Color selectedButtonColor = new Color(0.7f, 0.7f, 0.7f);

    // Start is called before the first frame update
    void Start()
    {
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

    public void ClearScrollView()
    {
        Debug.Log("TAMERE");
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
