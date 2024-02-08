using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class AtomObject : MonoBehaviour
{
    public Camera cam;
    public Canvas atomCanva;
    public float thresholdDistanceVisibility;
    public float thresholdDistanceOpacity;
    public TextMeshProUGUI atomSymbol;

    public string symbol = "";
    public Color color = Color.white;
    public float scale = 1.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetData(Camera cam, string symbol,Vector3 position, float scale, Color color)
    {
        this.cam = cam;
        this.symbol = symbol;
        this.color = color;

        this.atomSymbol.text = this.symbol;
        this.name = $"Atom_{symbol}";

        this.transform.position = position;
        this.transform.localScale = new Vector3(scale, scale, scale);
        this.scale = scale;

        var renderer = this.GetComponent<Renderer>();
        renderer.material.color = this.color;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var distanceToCamera = Vector3.Distance(this.transform.position, this.cam.transform.position);
        Debug.Log($"Atom{this.name}_distance_{distanceToCamera}");
        if (distanceToCamera < this.thresholdDistanceVisibility)
        {
            if (!this.atomCanva.enabled)
            {
                this.atomCanva.enabled = true;
            }
            float percent = 1.0f - (distanceToCamera - this.thresholdDistanceOpacity) / (this.thresholdDistanceVisibility - this.thresholdDistanceOpacity);

            if (percent > 1.0f)
            {
                percent = 1.0f;
            }
            else if (percent < 0.0f)
            {
                percent = 0.0f;
            }
            this.atomSymbol.color = new Color(1.0f, 1.0f, 1.0f, percent);

            this.transform.LookAt(this.transform.position + this.cam.transform.rotation * Vector3.forward, this.cam.transform.rotation * Vector3.up);
        }
        else
        {
            if (this.atomCanva.enabled)
            {
                this.atomCanva.enabled = false;
            }

        }
    }
}
