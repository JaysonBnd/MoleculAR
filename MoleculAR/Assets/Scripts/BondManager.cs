using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;


class Bond
{
    public LineRenderer start;
    public LineRenderer end;
}

public class BondManager : MonoBehaviour
{
    public LineRenderer linePrefab;

    private List<List<Bond>> bondsList;
    // Start is called before the first frame update
    void Start()
    {
        this.bondsList = new List<List<Bond>>();
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
        this.bondsList = new List<List<Bond>>();
    }

    public void AddBond(Vector3 startPosition, Vector3 endPosition, Color startColor, Color endColor, int order)
    {
        List<Bond> tmpBonds = new List<Bond>();
        for (int i = 0; i < order; i++)
        {
            float scale = (float)(i + 1) / (order + 1);

            var startLine = LineRenderer.Instantiate(linePrefab, this.transform);
            var endLine = LineRenderer.Instantiate(linePrefab, this.transform);

            var tmpStartPosition = startPosition;
            Quaternion tmpQuaternion = new Quaternion();
            tmpQuaternion.SetFromToRotation(startLine.transform.position, endLine.transform.position);

            Debug.Log(tmpQuaternion);
            tmpStartPosition += (startPosition - endPosition) * (0.20f * (1.0f - scale) - (0.20f * scale));

            var tmpEndPosition = endPosition;
            tmpEndPosition += (startPosition - endPosition) * (0.20f * (1.0f - scale) - (0.20f * scale));

            this.setPosition(startLine, tmpStartPosition, endLine, tmpEndPosition);
            this.SetColor(startLine, startColor, endLine, endColor);

            tmpBonds.Add(new Bond()
            {
                start = startLine,
                end = endLine
            });
        }
        this.bondsList.Add(tmpBonds);
    }

    public void UpdateBondWidthMultiplier(float widthMultiplier)
    {
        foreach (var bonds in this.bondsList)
        {
            foreach (var bond in bonds)
            {

                bond.start.widthMultiplier = widthMultiplier;
                bond.end.widthMultiplier = widthMultiplier;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
