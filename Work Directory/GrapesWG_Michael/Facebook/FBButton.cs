using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBButton : MonoBehaviour
{
    [SerializeField] private FacebookManager.PresedButton typeOfButton;
    private void Start()
    {
        FacebookManager.FillInList(gameObject, typeOfButton);
        GetComponent<Button>().onClick.AddListener(()=> 
            FacebookManager.ThisButtonPressed(typeOfButton));
    }
}
