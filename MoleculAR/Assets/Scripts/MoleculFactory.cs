using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI;


public class MoleculeFactory : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> objectAtomsList;
    private List<LineRenderer> objectBondsList;

    public AtomData atomData;
    public string urlToGet;

    public LineRenderer lineRenderer;



    void Start()
    {


        // A correct website page.
        StartCoroutine(this.GetRequest());

        this.objectAtomsList = new List<GameObject>();
        this.objectBondsList = new List<LineRenderer>();

    }

    IEnumerator GetRequest()
    {
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
                    var json_result = JsonUtility.FromJson<MoleculeRequestJson>(webRequest.downloadHandler.text);

                    var molecule = new MoleculeItem();

                    for (int i = 0; i < json_result.elements.Length; i++)
                    {
                        var atom = new AtomItem();
                        atom.atomNumber = json_result.elements[i];
                        atom.position = new Vector3(json_result.coords[i * 3], json_result.coords[(i * 3) + 1], json_result.coords[(i * 3) + 2]);

                        molecule.atomsList.Add(atom);
                    }

                    for (int i = 0; i * 2 < json_result.bonds.connections.index.Length; i++)
                    {
                        int first = json_result.bonds.connections.index[i * 2];
                        int second = json_result.bonds.connections.index[(i * 2) + 1];

                        var bond = new AtomBond();
                        bond.first = first;
                        bond.second = second;
                        molecule.bondsList.Add(bond);
                    }

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

            AtomItem atomItem = molecule.atomsList[i];

            atomObject.transform.position = atomItem.position;
            Atom atomData = this.atomData.Atoms[atomItem.atomNumber - 1];
            atomObject.transform.localScale = new Vector3(atomData.Scale, atomData.Scale, atomData.Scale);
            var renderer = atomObject.transform.GetComponent<MeshRenderer>();
            renderer.material.SetColor("_Color", atomData.Color);

            this.objectAtomsList.Add(atomObject);
        }

        for (int i = 0; i < molecule.bondsList.Count; i++)
        {
            var bond = molecule.bondsList[i];
            LineRenderer bondLine = LineRenderer.Instantiate(lineRenderer, this.transform);

            bondLine.SetPosition(0, this.objectAtomsList[bond.first].transform.position);
            bondLine.SetPosition(1, this.objectAtomsList[bond.second].transform.position);

            this.objectBondsList.Add(bondLine);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
