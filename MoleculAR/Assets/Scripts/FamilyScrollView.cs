using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FamilyScrollView : MenuScrollView<FamilyButton>
{
    public MoleculeScrollView moleculeScrollView;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FamilyGetRequest());
    }

    public IEnumerator FamilyGetRequest()
    {
        var familyUri = this.moleculeSpawner.GetMoleculePathUrl();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(familyUri))
        {
            webRequest.SetRequestHeader("bypass-tunnel-reminder", "true");
            webRequest.SetRequestHeader("User-Agent", $"MoleculAR for {SystemInfo.operatingSystem}");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = familyUri.Split('/');
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
                    var familyDictionnary = JsonUtility.FromJson<FamilyListJson>(webRequest.downloadHandler.text);
                    this.CreateButtons(familyDictionnary);
                    break;
            }
        }
    }

    public void CreateButtons(FamilyListJson familyDictionnary)
    {
        int i = 0;
        foreach (var familyData in familyDictionnary.families)
        {
            var button = Instantiate(this.scrollViewButtonPrefab, this.content.transform);
            button.SetButtonData(this, i, familyData.name, familyData.path);
            this.buttonList.Add(button);
            i += 1;
        }
    }

    protected override void DoSelectedButtonAction(int buttonSelectedId, string elementId)
    {
        this.moleculeScrollView.SetMoleculeFamily(elementId);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
