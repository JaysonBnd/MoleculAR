using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class UIScript : MonoBehaviour
{
    public Button openButton;
    public Button deleteButton;
    public Button optionButton;
    public FamilyScrollView familyScrollView;
    public MoleculeScrollView moleculeScrollView;

    public MoleculeSpawner moleculeSpawner;

    public TMP_InputField urlTextInput;

    public TextMeshProUGUI errorMessageText;
    public ARPlaneManager planeManager;
    private ARFeatheredPlaneMeshVisualizerCompanion[]  planesVisualizer = new ARFeatheredPlaneMeshVisualizerCompanion[0];
    private bool planeVisibility = true;

    // Start is called before the first frame update
    void Start()
    {
        this.urlTextInput.SetTextWithoutNotify(moleculeSpawner.apiUrl);
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateAPIUrl()
    {
        this.moleculeSpawner.SetNewUrl(this.urlTextInput.text);
        this.moleculeScrollView.ClearScrollView();
        this.familyScrollView.ClearScrollView();
        StartCoroutine(this.familyScrollView.FamilyGetRequest());
    }

    public void ClearAllObjects()
    {
        foreach (Transform child in moleculeSpawner.transform)
        {
            Destroy(child.gameObject);
        }
        this.moleculeSpawner.SetStatus(0);
    }

    public void UpdateButton()
    {
        switch (this.moleculeSpawner.GetStatus())
        {
            case -1:
                this.openButton.gameObject.SetActive(false);
                this.deleteButton.gameObject.SetActive(false);
                if (this.moleculeSpawner.GetErrorMessage().Length > 0)
                {
                    this.errorMessageText.gameObject.SetActive(true);
                    this.errorMessageText.SetText(this.moleculeSpawner.GetErrorMessage());
                }
                else
                {
                    this.errorMessageText.SetText("");
                    this.errorMessageText.gameObject.SetActive(false);
                }
                break;
            case 0:
                this.openButton.gameObject.SetActive(true);
                this.deleteButton.gameObject.SetActive(false);
                if (this.moleculeSpawner.GetErrorMessage().Length > 0)
                {
                    this.errorMessageText.gameObject.SetActive(true);
                    this.errorMessageText.SetText(this.moleculeSpawner.GetErrorMessage());
                }
                else
                {
                    this.errorMessageText.SetText("");
                    this.errorMessageText.gameObject.SetActive(false);
                }
                break;
            case 1:
                this.openButton.gameObject.SetActive(false);
                this.deleteButton.gameObject.SetActive(false);
                this.errorMessageText.SetText("");
                this.errorMessageText.gameObject.SetActive(false);
                break;
            case 2:
                this.openButton.gameObject.SetActive(false);
                this.deleteButton.gameObject.SetActive(true);
                this.errorMessageText.SetText("");
                this.errorMessageText.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdatePlaneVisibility()
    {
        Debug.Log(this.planesVisualizer.Length);
        foreach (var planeVisualizer in this.planesVisualizer)
        {
            planeVisualizer.visualizeSurfaces = this.planeVisibility;
        }
    }

    public void ChangePlaneVisibility()
    {
        this.planeVisibility = !this.planeVisibility;
        this.UpdatePlaneVisibility();
    }
    // Update is called once per frame
    void Update()
    {
        var planesArray = this.planeManager.GetComponentsInChildren<ARFeatheredPlaneMeshVisualizerCompanion>();
        if (planesArray != null)
        {
            if (planesArray.Length != this.planesVisualizer.Length)
            {
                this.planesVisualizer = planesArray;
                this.UpdatePlaneVisibility();
            }
        }
        this.UpdateButton();
    }
}
