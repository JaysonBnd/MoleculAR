using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
    public void OpenURL()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdo_rlxjNvB6kj0A34kSLO-2n5OuhPeWMB-FHd0JSTQXTAMeQ/viewform?usp=sf_link");
    }
}
