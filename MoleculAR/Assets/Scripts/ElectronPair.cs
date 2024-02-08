using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronPair : MonoBehaviour
{
    private float timer;

    public ElectronItem electronStart;
    public ElectronItem electronEnd;
    private float length = 0.2f;
    private float width = 0.15f;
    private float height = 0.15f;
    public float movementSpeed;
    public float rotationSpeed;
    public float thresholdDistanceVisibility;
    public float thresholdDistanceOpacity;

    private float rotationZ = 0.0f;

    private Color colorStart;
    private Color colorEnd;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetColor(Color colorStart, Color colorEnd)
    {
        this.colorStart = colorStart;
        this.colorEnd = colorEnd;

        this.electronStart.SetColor(colorStart);
        this.electronEnd.SetColor(colorEnd);
    }

    // Update is called once per frame
    void Update()
    {
        this.timer += Time.deltaTime * this.movementSpeed;

        if (this.timer > 2 * Mathf.PI)
        {
            this.timer -= 2 * Mathf.PI;
        }

        this.rotationZ += this.rotationSpeed * Time.deltaTime;
        if (this.rotationZ > 360.0f)
        {
            this.rotationZ -= 360.0f;
        }

        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.rotationZ);

        float xStart = Mathf.Cos(this.timer) * width;
        float yStart = Mathf.Sin(this.timer) * length;
        float zStart = Mathf.Sin(this.timer) * height;

        float xEnd = -Mathf.Cos(this.timer) * width;
        float yEnd = -Mathf.Sin(this.timer) * length;
        float zEnd = -Mathf.Sin(this.timer) * height;

        this.electronStart.transform.localPosition = new Vector3(xStart, yStart, zStart);
        this.electronEnd.transform.localPosition = new Vector3(xEnd, yEnd, zEnd);


    }

    void LateUpdate()
    {
        var distanceToCamera = Vector3.Distance(this.transform.position, Camera.main.transform.position);

        if (distanceToCamera < this.thresholdDistanceVisibility)
        {
            if (!this.electronStart.gameObject.activeSelf)
            {
                this.electronStart.gameObject.SetActive(true);
            }
            if (!this.electronEnd.gameObject.activeSelf)
            {
                this.electronEnd.gameObject.SetActive(true);
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
            this.colorStart.a = percent;
            this.colorEnd.a = percent;
            this.SetColor(this.colorStart, this.colorEnd);
        }
        else
        {
            if (this.electronStart.gameObject.activeSelf)
            {
                this.electronStart.gameObject.SetActive(false);
            }
            if (this.electronEnd.gameObject.activeSelf)
            {
                this.electronEnd.gameObject.SetActive(false);
            }

        }
    }

}
