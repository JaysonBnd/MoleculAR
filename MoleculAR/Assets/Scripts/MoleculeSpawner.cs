using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using static UnityEngine.Rendering.DebugUI;

public class MoleculeSpawner : MonoBehaviour
{
    [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
    public Camera cameraToFace;

    [Tooltip("The list of prefabs available to spawn.")]
    public MoleculeFactory moleculeFactoryPrefab;

    [Tooltip("Optional prefab to spawn for each spawned object. Use a prefab with the Destroy Self component to make " +
        "sure the visualization only lives temporarily.")]
    public GameObject spawnVisualizationPrefab;

    [Tooltip("The index of the prefab to spawn. If outside the range of the list, this behavior will select " +
        "a random object each time it spawns.")]
    public string pathToFetch = "";

    [Tooltip("Whether to only spawn an object if the spawn point is within view of the camera.")]
    public bool onlySpawnInView = true;


    [Tooltip("The size, in viewport units, of the periphery inside the viewport that will not be considered in view.")]
    public float viewportPeriphery = 0.15f;

    public string apiUrl = "http://localhost:5000";
    private string atomPathUrl = "api/atom";
    private string moleculePathUrl = "api/molecule";

    public UIScript uiScript;

    public Dictionary<string, Dictionary<string, Texture2D>> moleculeTexture2DDictionary = new Dictionary<string, Dictionary<string, Texture2D>>();
    public List<AtomItem> atomList = new List<AtomItem>();

    /// <summary>
    /// Event invoked after an object is spawned.
    /// </summary>
    /// <seealso cref="TrySpawnObject"/>
    public event Action<GameObject> ObjectSpawned;

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>

    /// <summary>
    /// Attempts to spawn an object from <see cref="objectPrefabs"/> at the given position. The object will have a
    /// yaw rotation that faces <see cref="cameraToFace"/>, plus or minus a random angle within <see cref="spawnAngleRange"/>.
    /// </summary>
    /// <param name="spawnPoint">The world space position at which to spawn the object.</param>
    /// <param name="spawnNormal">The world space normal of the spawn surface.</param>
    /// <returns>Returns <see langword="true"/> if the spawner successfully spawned an object. Otherwise returns
    /// <see langword="false"/>, for instance if the spawn point is out of view of the camera.</returns>
    /// <remarks>
    /// The object selected to spawn is based on <see cref="spawnOptionIndex"/>. If the index is outside
    /// the range of <see cref="objectPrefabs"/>, this method will select a random prefab from the list to spawn.
    /// Otherwise, it will spawn the prefab at the index.
    /// </remarks>
    /// <seealso cref="objectSpawned"/>
    /// 

    private void Start()
    {
        this.cameraToFace = Camera.main;
        StartCoroutine(this.AtomGetRequest(this.apiUrl));

    }

    public void SetNewUrl(string url)
    {
        StartCoroutine(this.AtomGetRequest(url));
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
    IEnumerator AtomGetRequest(string url)
    {
        var atomUri = $"{url}/{this.atomPathUrl}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(atomUri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = atomUri.Split('/');
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
                    // A correct website page.
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.atomList = this.JsonToAtomsItem(webRequest.downloadHandler.text);
                    this.apiUrl = url;

                    break;
            }
        }
    }

    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        if (this.onlySpawnInView)
        {
            var inViewMin = this.viewportPeriphery;
            var inViewMax = 1f - this.viewportPeriphery;
            var pointInViewportSpace = this.cameraToFace.WorldToViewportPoint(spawnPoint);
            if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
            {
                return false;
            }
        }

        var pathToFetch = this.pathToFetch;


        if (pathToFetch.Length == 0)
        {
            return false;
        }

        var moleculeFactory = Instantiate(this.moleculeFactoryPrefab, this.transform);

        moleculeFactory.transform.position = spawnPoint;

        var facePosition = this.cameraToFace.transform.position;
        var forward = facePosition - spawnPoint;
        BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
        moleculeFactory.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        if (this.spawnVisualizationPrefab != null)
        {
            var visualizationTrans = Instantiate(this.spawnVisualizationPrefab).transform;
            visualizationTrans.position = spawnPoint;
            visualizationTrans.rotation = moleculeFactory.transform.rotation;
        }

        ObjectSpawned?.Invoke(moleculeFactory.gameObject);

        StartCoroutine(this.StartMoleculeFactoryCoroutine(moleculeFactory, this.atomList, $"{this.apiUrl}/{this.moleculePathUrl}/{pathToFetch}"));

        return true;
    }

    private IEnumerator StartMoleculeFactoryCoroutine(MoleculeFactory moleculeFactory, List<AtomItem> atomItemList, string uriMolecule)
    {
        this.uiScript.UpdateButton();
        moleculeFactory.MoleculeGetRequest(atomItemList, uriMolecule);
        this.uiScript.UpdateButton();
        yield return null;
    }
}
