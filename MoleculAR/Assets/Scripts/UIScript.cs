using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Button openButton;
    public Button deleteButton;
    public Button optionButton;

    public GameObject objectSpawner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.objectSpawner.transform.childCount > 0)
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
}
