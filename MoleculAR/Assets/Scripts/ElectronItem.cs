using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronItem : MonoBehaviour
{
    public float globalScale = 0.09f;

    private Transform higherParent;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetHigherParent(Transform parent)
    {
        this.higherParent = parent;
    }

    public void SetColor(Color color)
    {

        var renderer = this.GetComponent<Renderer>();
        var trailRenderer = this.GetComponent<TrailRenderer>();

        renderer.material.color = color;
        var colorGradient = trailRenderer.colorGradient;
        var colorKeyGradient = colorGradient.colorKeys;
        colorKeyGradient[0].color = color;

        var colorAlphaGradient = colorGradient.alphaKeys;
        colorAlphaGradient[0].alpha = color.a;
        colorAlphaGradient[1].alpha = color.a;

        colorGradient.SetKeys(colorKeyGradient, colorAlphaGradient);
        trailRenderer.colorGradient = colorGradient;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.higherParent != null)
        {
            this.transform.localScale = Vector3.one;
            //this.transform.localScale = Vector3.Scale(thirdParent.localScale, (this.transform.lossyScale * this.globalScale));
            this.transform.localScale = Vector3.Scale(this.higherParent.localScale, new Vector3(globalScale / transform.lossyScale.x, globalScale / transform.lossyScale.y, globalScale / transform.lossyScale.z));

            var trailRenderer = this.GetComponent<TrailRenderer>();
            trailRenderer.widthMultiplier = this.transform.lossyScale.x;
        }
    }
}
