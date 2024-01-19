using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class BondManager : MonoBehaviour
{
    public LineRenderer linePrefab;

    private List<LineRenderer> linesList;
    // Start is called before the first frame update
    void Start()
    {
    }

    void setPosition(LineRenderer startLine, Vector3 startPosition, LineRenderer endLine, Vector3 endPosition)
    {
        var midPosition = (startPosition + endPosition) / 2;

        startLine.SetPosition(0, startPosition);
        startLine.SetPosition(1, midPosition);
        endLine.SetPosition(0, midPosition);
        endLine.SetPosition(1, endPosition);
    }

    void SetColor(LineRenderer startLine, Color startColor, LineRenderer endLine, Color endColor)
    {
        var startGradient = new Gradient();
        var endGradient = new Gradient();

        var startGradientColor = new GradientColorKey[1];
        startGradientColor[0] = new GradientColorKey(startColor, 0.0f);
        var endGradientColor = new GradientColorKey[1];
        endGradientColor[0] = new GradientColorKey(endColor, 0.0f);

        var gradientAlpha = new GradientAlphaKey[1];
        gradientAlpha[0] = new GradientAlphaKey(1.0f, 0.0f);

        startGradient.SetKeys(startGradientColor, gradientAlpha);
        endGradient.SetKeys(endGradientColor, gradientAlpha);

        startLine.colorGradient = startGradient;
        endLine.colorGradient = endGradient;
    }

    public void InitializeBond()
    {
        this.linesList = new List<LineRenderer>();
    }

    public void AddBond(Vector3 startPosition, Vector3 endPosition, Color startColor, Color endColor)
    {

        var startLine = LineRenderer.Instantiate(linePrefab, this.transform);
        var endLine = LineRenderer.Instantiate(linePrefab, this.transform);

        this.setPosition(startLine, startPosition, endLine, endPosition);
        this.SetColor(startLine, startColor, endLine, endColor);

        this.linesList.Add(startLine);
        this.linesList.Add(endLine);
    }

    public void UpdateBondWidthMultiplier(float widthMultiplier)
    {
        foreach (var line in this.linesList)
        {
            line.widthMultiplier = widthMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
