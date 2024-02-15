using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class MoleculeFactory : MonoBehaviour
{
    // Start is called before the first frame update
    private List<AtomObject> objectAtomsList = new List<AtomObject>();
    public BondManager bondsManager;
    private string uriAtom = "https://epitech-vir-tunnel.loca.lt/api/atom";

    private float lastScale = 1.0f;

    private List<AtomItem> atomList = new List<AtomItem>();
    public string urlToGet;

    public BondManager bondPrefab;
    public AtomObject atomPrefab;

    public bool isIntancied = false;

    private bool isMoleculeIntanciate = false;

    private Transform lowerAtom;

    public Transform higherParent;
    private UnityEngine.InputSystem.Gyroscope gyro;

    private float distanceToSpin = 4.0f;
    private List<Vector3> vectorList = new List<Vector3>();
    private float higherDistance = 0.0f;
    private float rotationInertia = 0.0f;

    void Start()
    {
        if (higherParent == null)
        {
            this.higherParent = this.transform;
        }

        this.bondsManager.InitializeBond();


        if (SystemInfo.supportsGyroscope)
        {
            this.gyro = UnityEngine.InputSystem.Gyroscope.current;
            InputSystem.EnableDevice(gyro);
            this.gyro.samplingFrequency = 1.0f / 15.0f;
        }

        // A correct website page.
        if (this.isIntancied)
        {
            StartCoroutine(this.AtomGetRequest());

        }
    }

    public bool IsMoleculeIntanciate()
    {
        return this.isMoleculeIntanciate;
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
            webRequest.SetRequestHeader("bypass-tunnel-reminder", "true");
            webRequest.SetRequestHeader("User-Agent", $"MoleculAR for {SystemInfo.operatingSystem}");
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
            webRequest.SetRequestHeader("bypass-tunnel-reminder", "true");
            webRequest.SetRequestHeader("User-Agent", $"MoleculAR for {SystemInfo.operatingSystem}");
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
                    this.isMoleculeIntanciate = true;
                    break;
            }
        }
    }

    void InitializeMolecule(MoleculeItem molecule, List<AtomItem> atomItemList)
    {
        var lowerYPoint = Mathf.Infinity;

        var farest_atom = 0.0f;
        var atom_scale = 0.0f;

        var totalPosition = Vector3.zero;
        foreach (MoleculeAtom atom in molecule.atomsList)
        {
            totalPosition += atom.position;
        }

        var centerDelta = totalPosition / molecule.atomsList.Count;
        Debug.Log($"Molecule Center Delta {centerDelta}");

        for (int i = 0; i < molecule.atomsList.Count; i++)
        {
            MoleculeAtom atom = molecule.atomsList[i];

            AtomItem atomData = new AtomItem() { AtomicNumber = 0, Color = Color.white, Scale = 1.0f, Symbol = "" };
            if (atomItemList.Count > atom.atomNumber - 1)
            {
                atomData = atomItemList[atom.atomNumber - 1];
            }

            AtomObject atomObject = GameObject.Instantiate(atomPrefab, this.transform);
            Debug.Log(atom.position);
            atomObject.SetData(Camera.main, atomData.Symbol, atom.position - centerDelta, atomData.Scale, atomData.Color);
            if (atomObject.transform.localPosition.y - atomObject.transform.localScale.y < lowerYPoint)
            {
                lowerYPoint = atomObject.transform.localPosition.y - atomObject.transform.localScale.y;
                this.lowerAtom = atomObject.transform;
            }
            this.objectAtomsList.Add(atomObject);

            var distance_to_atom = Vector3.Distance(this.transform.position, atomObject.transform.position);

            if (distance_to_atom > farest_atom)
            {
                farest_atom = distance_to_atom;
                atom_scale = atomObject.scale;
            }
        }
        var sphereCollider = this.GetComponent<SphereCollider>();
        sphereCollider.radius = farest_atom;

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
            this.bondsManager.SetHigherParent(this.higherParent);
        }
    }


    // Update is called once per frame
    void Update()
    {
        float newScale = (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3;
        if (this.lastScale != (this.transform.localScale.x + this.transform.localScale.y + this.transform.localScale.z) / 3)
        {
            this.bondsManager.UpdateBondWidthMultiplier(newScale);
            this.lastScale = newScale;
        }

        if (this.gyro != null)
        {
            var velocity = this.gyro.angularVelocity.ReadValue();

            foreach (var vector in this.vectorList)
            {
                var distance = Vector3.Distance(velocity, vector);

                if (distance > this.distanceToSpin)
                {
                    this.rotationInertia = 360.0f * 3;
                }

                if (distance > this.higherDistance)
                {
                    this.higherDistance = distance;
                }
            }
            this.vectorList.Add(velocity);

            if (this.vectorList.Count > 15) {
                this.vectorList.RemoveAt(0);
            }
        }

        var calcul = this.rotationInertia * 0.6f * Time.deltaTime;
        this.transform.Rotate(Vector3.up, calcul * 1.5f);
        this.rotationInertia -= calcul;
        if (calcul < 2.5f) {
            this.rotationInertia = 0.0f;
        }
    }
}
