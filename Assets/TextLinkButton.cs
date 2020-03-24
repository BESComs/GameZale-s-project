using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLinkButton : MonoBehaviour
{
    public string url;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Call);
    }

    private void Call()
    {
        Application.OpenURL(url);
    }
}
