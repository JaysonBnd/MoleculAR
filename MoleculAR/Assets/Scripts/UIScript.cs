using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class UIScript : MonoBehaviour
{
    public Button openButton;
    public Button deleteButton;
    public Button optionButton;

    public MoleculeSpawner moleculeSpawner;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ClearAllObjects()
    {
        foreach (Transform child in moleculeSpawner.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateButton()
    {
        switch (this.moleculeSpawner.GetStatus())
        {

            case 0:
                this.openButton.gameObject.SetActive(true);
                this.deleteButton.gameObject.SetActive(false);
                break;
            case 1:
                this.openButton.gameObject.SetActive(false);
                this.deleteButton.gameObject.SetActive(false);
                break;
            case 2:
                this.openButton.gameObject.SetActive(false);
                this.deleteButton.gameObject.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateButton();
    }
}
