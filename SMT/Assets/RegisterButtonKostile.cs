using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterButtonKostile : MonoBehaviour
{
    private Button button;
    
    public Toggle confirmToggle;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        button.interactable = confirmToggle.isOn;
    }
}
