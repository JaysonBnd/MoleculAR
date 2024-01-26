using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Button openButton;
    public Button deleteButton;
    public Button optionButton;

    public GameObject moleculeSpawner;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void UpdateButton()
    {
        if (this.moleculeSpawner.transform.childCount > 0)
        {
            this.openButton.gameObject.SetActive(false);
            this.deleteButton.gameObject.SetActive(true);
        }
        else
        {
            this.openButton.gameObject.SetActive(true);
            this.deleteButton.gameObject.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
