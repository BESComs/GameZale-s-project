using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherBackButtonV2 : MonoBehaviour
{
    public static LauncherBackButtonV2 Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    
    public void OnBackPressed()
    {
        LauncherV2.Instance.OnBackPressed();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}