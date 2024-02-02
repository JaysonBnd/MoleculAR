using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;


public class BondManager : MonoBehaviour
{
    public BondItem bondPrefab;

    private List<List<BondItem>> bondsList;
    // Start is called before the first frame update
    void Start()
    {
        this.bondsList = new List<List<BondItem>>();
    }

    public void InitializeBond()
    {
        this.bondsList = new List<List<BondItem>>();
    }

    public void AddBond(Vector3 startPosition, Vector3 endPosition, Color startColor, Color endColor, int order, string nameAtomsBond)
    {
        var bondEmptyObject = new GameObject($"Bonds_{nameAtomsBond}");

        var localScale = bondEmptyObject.transform.localScale;
        localScale.z = Vector3.Distance(startPosition, endPosition);
        bondEmptyObject.transform.localScale = localScale;
        bondEmptyObject.transform.position = startPosition;
        bondEmptyObject.transform.LookAt(endPosition);

        List<BondItem> tmpBonds = new List<BondItem>();
        for (int i = 0; i < order; i++)
        {
            float scale = (float)(i + 1) / (order + 1);

            bondEmptyObject.transform.parent = this.transform;

            var bond = LineRenderer.Instantiate(bondPrefab, bondEmptyObject.transform);
            bond.name = $"Bond_{i}";

            var localEmptyObjectLocationY = (0.3f * (1.0f - scale)) - (0.3f * scale);

            var bondPosition = bond.transform.localPosition;
            bondPosition.y = localEmptyObjectLocationY;

            var bondScale = bond.transform.localScale;
            bondScale.x *= (float)order / (order * 3 - 2);
            bondScale.y *= (float)order / (order * 3 - 2);
            bond.transform.localScale = bondScale;

            bond.transform.localPosition = bondPosition;

            Debug.Log($"{bond.name} - {localEmptyObjectLocationY}");

            bond.SetColor(startColor, endColor);

            tmpBonds.Add(bond);
        }
        this.bondsList.Add(tmpBonds);
    }

    public void SetHigherParent(Transform parent)
    {
        foreach (var bonds in this.bondsList)
        {
            foreach(var bond in bonds)
            {
                bond.SetElectronHigherParent(parent);
            }
        }
    }

    public void UpdateBondWidthMultiplier(float widthMultiplier)
    {
        //foreach (var bonds in this.bondsList)
        //{
        //foreach (var bond in bonds)
        //{

        //       bond.start.widthMultiplier = widthMultiplier;
        //bond.end.widthMultiplier = widthMultiplier;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
