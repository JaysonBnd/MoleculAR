using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI;

public class MoleculeFactory : MonoBehaviour
{
    // Start is called before the first frame update
    private List<AtomObject> objectAtomsList;
    public BondManager bondsManager;
    private string uriAtom;

    private float lastScale;

    private List<AtomItem> atomList;
    public string urlToGet;

    public BondManager bondPrefab;
    public AtomObject atomPrefab;

    public bool isIntancied = false;

    void Start()
    {


        this.objectAtomsList = new List<AtomObject>();
        this.bondsManager.InitializeBond();
        this.uriAtom = "http://localhost:5000/api/atom";

        this.atomList = new List<AtomItem>();
        // A correct website page.
        if (this.isIntancied)
        {
            StartCoroutine(this.AtomGetRequest());

        }

        this.lastScale = (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3;
    }

    List<AtomItem> JsonToAtomsItem(string json_string)
    {
        var json_result = JsonUtility.FromJson<AtomRequestJson>(json_string);
        var atomList = new List<AtomItem>();

        for (int i = 0; i < json_result.atoms.Length; i++)
        {
            var atom = new AtomItem();
            var atom_json = json_result.atoms[i];
            atom.AtomicNumber = atom_json.AtomicNumber;
            atom.Symbol = atom_json.Symbol;
            atom.Scale = atom_json.Scale;
            atom.Color = new Color((float)atom_json.R / 255, (float)atom_json.G / 255, (float)atom_json.B / 255, (float)atom_json.A / 255);


            atomList.Add(atom);
        }

        return atomList;
    }

    IEnumerator AtomGetRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.uriAtom))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = this.urlToGet.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.atomList = this.JsonToAtomsItem(webRequest.downloadHandler.text);

                    // A correct website page.
                    StartCoroutine(this.MoleculeGetRequest(this.atomList, this.urlToGet));
                    break;
            }
        }
    }

    MoleculeItem JsonToMoleculeItem(string json_string)
    {
        var json_result = JsonUtility.FromJson<MoleculeRequestJson>(json_string);
        var molecule = new MoleculeItem();

        for (int i = 0; i < json_result.elements.Length; i++)
        {
            var atom = new MoleculeAtom
            {
                atomNumber = json_result.elements[i],
                position = new Vector3(json_result.coords[i * 3], json_result.coords[(i * 3) + 1], json_result.coords[(i * 3) + 2])
            };

            molecule.atomsList.Add(atom);
        }

        for (int i = 0; i * 2 < json_result.bonds.connections.index.Length; i++)
        {
            int first = json_result.bonds.connections.index[i * 2];
            int second = json_result.bonds.connections.index[(i * 2) + 1];
            int order = json_result.bonds.order[i];
            var bond = new MoleculeBond()
            {
                first = first,
                second = second,
                order = order
            };

            molecule.bondsList.Add(bond);
        }
        return molecule;
    }

    public IEnumerator MoleculeGetRequest(List<AtomItem> atomItemList, string uriMolecule)
    {
        //uri_molecule = $"{this.GetIP()}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uriMolecule))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uriMolecule.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    var molecule = this.JsonToMoleculeItem(webRequest.downloadHandler.text);

                    InitializeMolecule(molecule, atomItemList);
                    break;
            }
        }
    }

    void InitializeMolecule(MoleculeItem molecule, List<AtomItem> atomItemList)
    {
        Debug.Log("Je suis arrivé jusqu'ici");
        var lowerYPoint = Mathf.Infinity;

        for (int i = 0; i < molecule.atomsList.Count; i++)
        {
            MoleculeAtom atom = molecule.atomsList[i];

            AtomItem atomData = new AtomItem() { AtomicNumber = 0, Color = Color.white, Scale = 1.0f, Symbol = "" };
            if (atomItemList.Count > atom.atomNumber - 1)
            {
                atomData = atomItemList[atom.atomNumber - 1];
            }

            AtomObject atomObject = GameObject.Instantiate(atomPrefab, this.transform);

            atomObject.SetData(Camera.main, atomData.Symbol, atom.position, atomData.Scale, atomData.Color);
            if (atomObject.transform.position.y - 1.0f < lowerYPoint)
            {
                lowerYPoint = atomObject.transform.position.y - 1.0f;
            }
            this.objectAtomsList.Add(atomObject);
        }

        var tmpPosition = this.transform.localPosition;
        tmpPosition.y -= lowerYPoint;
        this.transform.localPosition = tmpPosition;

        for (int i = 0; i < molecule.bondsList.Count; i++)
        {
            var bond = molecule.bondsList[i];
            var firstAtom = this.objectAtomsList[bond.first];
            var secondAtom = this.objectAtomsList[bond.second];

            var startPos = firstAtom.transform.position;
            var endPos = secondAtom.transform.position;

            Color firstColor = firstAtom.color;
            Color secondColor = secondAtom.color;

            this.bondsManager.AddBond(startPos, endPos, firstColor, secondColor, bond.order, $"{firstAtom.symbol}_{secondAtom.symbol}");
        }
    }


    // Update is called once per frame
    void Update()
    {
        float newScale = (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3;
        if (this.lastScale != (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3)
        {
            this.lastScale = newScale;

            this.bondsManager.UpdateBondWidthMultiplier(this.lastScale);

        }
    }
}
