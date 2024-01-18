using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI;

public class MoleculeFactory : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> objectAtomsList;
    private List<Tuple<LineRenderer, LineRenderer>> objectBondsList;
    private string uriAtom;

    private List<AtomItem> atomList;
    public string urlToGet;

    public LineRenderer lineRenderer;

    void Start()
    {


        this.objectAtomsList = new List<GameObject>();
        this.objectBondsList = new List<Tuple<LineRenderer, LineRenderer>>();
        this.uriAtom = "http://localhost:5000/api/atom";

        this.atomList = new List<AtomItem>();
        // A correct website page.
        StartCoroutine(this.AtomGetRequest());
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
            atom.Color = new Color(atom_json.R / 255, atom_json.G / 255, atom_json.B / 255, atom_json.A / 255);


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
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.atomList = this.JsonToAtomsItem(webRequest.downloadHandler.text);

                    // A correct website page.
                    StartCoroutine(this.MoleculeGetRequest());
                    break;
            }
        }
        Debug.Log("Ended");
    }

    MoleculeItem JsonToMoleculeItem(string json_string)
    {
        var json_result = JsonUtility.FromJson<MoleculeRequestJson>(json_string);
        var molecule = new MoleculeItem();

        for (int i = 0; i < json_result.elements.Length; i++)
        {
            var atom = new MoleculeAtom();
            atom.atomNumber = json_result.elements[i];
            atom.position = new Vector3(json_result.coords[i * 3], json_result.coords[(i * 3) + 1], json_result.coords[(i * 3) + 2]);

            molecule.atomsList.Add(atom);
        }

        for (int i = 0; i * 2 < json_result.bonds.connections.index.Length; i++)
        {
            int first = json_result.bonds.connections.index[i * 2];
            int second = json_result.bonds.connections.index[(i * 2) + 1];

            var bond = new MoleculeBond();
            bond.first = first;
            bond.second = second;
            molecule.bondsList.Add(bond);
        }
        return molecule;
    }

    string GetIP()
    {
        String strHostName = System.Net.Dns.GetHostName();

        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[^1].ToString();
    }

    IEnumerator MoleculeGetRequest()
    {
        //uri_molecule = $"{this.GetIP()}"
        Debug.Log(this.GetIP());
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.urlToGet))
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
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    var molecule = this.JsonToMoleculeItem(webRequest.downloadHandler.text);

                    InitializeMolecule(molecule);
                    break;
            }
        }
        Debug.Log("Ended");
    }

    void InitializeMolecule(MoleculeItem molecule)
    {
        for (int i = 0; i < molecule.atomsList.Count; i++)
        {
            GameObject atomObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atomObject.transform.SetParent(this.transform);

            MoleculeAtom atomItem = molecule.atomsList[i];

            atomObject.transform.position = atomItem.position;
            AtomItem atomData = this.atomList[atomItem.atomNumber - 1];

            atomObject.transform.localScale = new Vector3(atomData.Scale, atomData.Scale, atomData.Scale);
            var renderer = atomObject.GetComponent<Renderer>();
            renderer.material.color = atomData.Color;

            this.objectAtomsList.Add(atomObject);
        }

        for (int i = 0; i < molecule.bondsList.Count; i++)
        {
            var bond = molecule.bondsList[i];
            LineRenderer firstBondLine = LineRenderer.Instantiate(lineRenderer, this.transform);
            LineRenderer secondBondLine = LineRenderer.Instantiate(lineRenderer, this.transform);

            Color firstColor = this.atomList[molecule.atomsList[bond.first].atomNumber - 1].Color;
            Color secondColor = this.atomList[molecule.atomsList[bond.second].atomNumber - 1].Color;
            var midPoint = (this.objectAtomsList[bond.first].transform.position + this.objectAtomsList[bond.second].transform.position) / 2;

            firstBondLine.startColor = firstColor;
            firstBondLine.endColor = firstColor;
            firstBondLine.SetPosition(0, this.objectAtomsList[bond.first].transform.position);
            firstBondLine.SetPosition(1, midPoint);

            Debug.Log(firstColor);
            Debug.Log(secondColor);
            secondBondLine.startColor = secondColor;
            secondBondLine.endColor = secondColor;
            secondBondLine.SetPosition(0, midPoint);
            secondBondLine.SetPosition(1, this.objectAtomsList[bond.second].transform.position);

            Tuple<LineRenderer, LineRenderer> bondLineTuple = new Tuple<LineRenderer, LineRenderer>(firstBondLine, secondBondLine);
            this.objectBondsList.Add(bondLineTuple);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
