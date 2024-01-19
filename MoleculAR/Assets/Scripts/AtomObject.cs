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

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetCamera(Camera cam)
    {
        this.cam = cam;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var distanceToCamera = Vector3.Distance(this.transform.position, this.cam.transform.position);
        
        if (distanceToCamera < this.thresholdDistanceVisibility)
        {
            if (!this.atomCanva.enabled)
            {
                this.atomCanva.enabled = true;
            }
            float percent = 1.0f - (distanceToCamera - this.thresholdDistanceOpacity) / (this.thresholdDistanceVisibility - this.thresholdDistanceOpacity);
            Debug.Log($"{distanceToCamera} {this.thresholdDistanceVisibility - this.thresholdDistanceOpacity} {percent}" );

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
