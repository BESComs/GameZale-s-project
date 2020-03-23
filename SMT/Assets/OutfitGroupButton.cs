using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutfitGroupButton : MonoBehaviour
{
    public OutfitType outfitType;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Call);
    }

    private void Call()
    {
        OutfitContent.Instance.PopulateContentByType(outfitType);
    }
}


