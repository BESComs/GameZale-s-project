using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharCustomizationSaver : MonoBehaviour
{
    public const string CustomizatonSaveKey = "CharCustomizationSave";

    public static CharCustomizationSaver Instance;
    
    private CharCustomizationController charController;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCharCustomizationController(CharCustomizationController charCustCont)
    {
        charController = charCustCont;
    }

    public CharCustomizationData CreateCurrentCustomizationData()
    {
        var customizationData = new CharCustomizationData();
        customizationData.genderData = charController.gender;
        foreach (var outfitSlot in charController.outfitSlots)
        {
            customizationData.SetTextureVariant(outfitSlot.BaseItemId, outfitSlot.TextureVariantId, outfitSlot.type);
        }

        return customizationData;
    }
    
    [Button]
    public void SaveData()
    {
        var customizationData = CreateCurrentCustomizationData();
        var jsonData = JsonUtility.ToJson(customizationData);
        PlayerPrefs.SetString(CustomizatonSaveKey, jsonData);
        PlayerPrefs.Save();
    }

    [Button]
    public CharCustomizationData LoadData()
    {
        var jsonData = PlayerPrefs.GetString(CustomizatonSaveKey);
        return JsonUtility.FromJson<CharCustomizationData>(jsonData);
    }

}

[Serializable]
public class CharCustomizationData
{
    public GenderData genderData;

    public TextureVariantData hatData;
    public TextureVariantData hairData;
    public TextureVariantData torsoData;
    public TextureVariantData braceLeftData;
    public TextureVariantData braceRightData;
    public TextureVariantData legsData;
    public TextureVariantData footwearData;
    
    public void SetTextureVariant(int itemId, int textureVariantId, OutfitType outfitType)
    {
        switch (outfitType)
        {
            case OutfitType.Hat:
                hatData.SetValues(itemId, textureVariantId);
                break;
            case OutfitType.Hair:
                hairData.SetValues(itemId, textureVariantId);
                break;
            case OutfitType.Torso:
                torsoData.SetValues(itemId, textureVariantId);
                break;
            case OutfitType.Legs:
                legsData.SetValues(itemId, textureVariantId);
                break;
            case OutfitType.Footwear:
                footwearData.SetValues(itemId, textureVariantId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(outfitType), outfitType, null);
        }
    }
}

[Serializable]
public enum GenderData
{
    Male, Female
}


[Serializable]
public struct TextureVariantData
{
    public int baseItemId;
    public int textureVariantId;


    public void SetValues(int baseItem, int textureVariant)
    {
        baseItemId = baseItem;
        textureVariantId = textureVariant;
    }
}
