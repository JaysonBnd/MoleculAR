using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoleculeButton : ScrollViewButton
{
    public Image moleculeImage;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetMoleculeImage(Sprite sprite)
    {
        this.moleculeImage.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
