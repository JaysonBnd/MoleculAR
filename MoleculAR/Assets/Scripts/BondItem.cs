using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondItem : MonoBehaviour
{
    public GameObject bondStart;
    public GameObject bondEnd;
    public ElectronPair electronPair;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame

    public void SetElectronHigherParent(Transform parent)
    {
        this.electronPair.SetElectronHigherParent(parent);
    }

    public void SetColor(Color startColor, Color endColor)
    {
        var rendererStart = bondStart.GetComponent<Renderer>();
        var rendererEnd = bondEnd.GetComponent<Renderer>();
        rendererStart.material.color = startColor;
        rendererEnd.material.color = endColor;

        electronPair.SetColor(startColor, endColor);
    }

    void Update()
    {

    }
}
