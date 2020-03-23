using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LessonsButton : MonoBehaviour
{
    public GameObject lessonsStuff;
    public GameObject customizationStuff;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        lessonsStuff.SetActive(true);
        customizationStuff.SetActive(false);
    }

}