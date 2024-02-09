using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoleculeScrollView : MenuScrollView
{
    private string moleculeFamily = "";
    protected new List<MoleculeButton> buttonList = new List<MoleculeButton>();
    private Dictionary<string, Sprite> moleculeImageDictionnary = new Dictionary<string, Sprite>();
    private Coroutine moleculeByFamilyCoroutine;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetMoleculeFamily(string moleculeFamily)
    {
        if (this.moleculeFamily != moleculeFamily)
        {
            if (moleculeByFamilyCoroutine != null)
            {
                StopCoroutine(this.moleculeByFamilyCoroutine);
            }

            this.ClearScrollView();
            if (moleculeFamily != "")
            {
                moleculeByFamilyCoroutine = StartCoroutine(this.MoleculeByFamilyGetRequest(moleculeFamily));
            }
        }
    }

    IEnumerator MoleculeByFamilyGetRequest(string moleculeFamily)
    {
        var moleculeFamilyUri = $"{this.moleculeSpawner.apiUrl}/{this.moleculeSpawner.GetMoleculePathUrl()}/{moleculeFamily}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(moleculeFamilyUri))
        {
            webRequest.SetRequestHeader("bypass-tunnel-reminder", "true");
            webRequest.SetRequestHeader("User-Agent", $"MoleculAR for {SystemInfo.operatingSystem}");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = moleculeFamilyUri.Split('/');
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
                    var moleculeDictionnary = JsonUtility.FromJson<Dictionary<string, Dictionary<string, string>>>(webRequest.downloadHandler.text);
                    this.moleculeFamily = moleculeFamily;
                    // A correct website page.
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    // this.atomList = this.JsonToAtomsItem(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public void CreateButtons() { }

    IEnumerator GetMoleculeImage(string moleculeFamily, List<MoleculeButton> buttonList)
    {
        foreach (var button in buttonList)
        {
            var moleculeId = button.GetElementId();
            var moleculePath = $"{moleculeFamily}/{moleculeId}";
            if (!this.moleculeImageDictionnary.ContainsKey(moleculePath))
            {
                yield return this.MoleculeImageGetRequest(moleculePath);
            }

            if (this.moleculeImageDictionnary.ContainsKey(moleculePath))
            {
                button.SetMoleculeImage(this.moleculeImageDictionnary[moleculePath]);
            }
        }
    }

    IEnumerator MoleculeImageGetRequest(string moleculePath)
    {
        var moleculeImageUri = $"{this.moleculeSpawner.apiUrl}/{this.moleculeSpawner.GetMoleculePathUrl()}/{moleculePath}/image";

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(moleculeImageUri))
        {
            webRequest.SetRequestHeader("bypass-tunnel-reminder", "true");
            webRequest.SetRequestHeader("User-Agent", $"MoleculAR for {SystemInfo.operatingSystem}");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = moleculeImageUri.Split('/');
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
                    var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    if (this.moleculeImageDictionnary.ContainsKey(moleculePath))
                    {
                        this.moleculeImageDictionnary[moleculePath] = sprite;
                    }
                    else
                    {
                        this.moleculeImageDictionnary.Add(moleculePath, sprite);
                    }

                    break;
            }
        }
    }

    public void ResetScrollView()
    {
        foreach (var button in this.buttonList)
        {
            Destroy(button.gameObject);
        }
        buttonList.Clear();
        moleculeImageDictionnary.Clear();
    }

    protected override void DoSelectedButtonAction(int buttonSelectedId, string elementId)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
