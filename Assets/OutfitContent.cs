using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitContent : MonoBehaviour
{
    public static OutfitContent Instance;

    public OutfitView outfitViewPrefab;

    public OutfitControllerNew outfitController;
    
    private void Awake()
    {
        Instance = this;
    }

    public void PopulateContentByType(OutfitType outfitType)
    {
        ClearContent();
        var presets = outfitController.FindPresetsOfType(outfitType);
        foreach (var preset in presets)
        {
            AddOutfitView(preset);
        }
    }

    public void PreviewOutfit(OutfitPreset outfitPreset)
    {
        outfitController.PreviewOutfit(outfitPreset);
    }
    
    
    private void AddOutfitView(OutfitPreset outfitPreset)
    {
        var outfitView = Instantiate(outfitViewPrefab, transform, false);
        outfitView.Init(outfitPreset);
    }

    private void ClearContent()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    
    
}