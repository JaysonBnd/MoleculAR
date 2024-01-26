using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionModal : MonoBehaviour
{
    public void ReverseActive()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
