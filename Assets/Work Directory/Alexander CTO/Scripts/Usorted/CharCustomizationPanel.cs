using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CharCustomizationPanel : MonoBehaviour
{
    public static CharCustomizationPanel Instance { get; private set; }

    public InputField nameField;
    
    private OutfitPartsContainer outfitPartsContainer;
    
    public CharCustomizationController charController;
    public TextureVariantButton textureVariantButtonPrefab;
    public List<CharOutfitGroup> outfitGroups;

    private void Awake()
    {
        Assert.IsNull(Instance, "CharCustomizationPanel Instance duplicate");
        Instance = this;
    }

    private void Start()
    {
        SetCharController(charController);
    }

    public void StartGame()
    {
        
    }
    
    public void SetCharController(CharCustomizationController charCustomizationController)
    {
        charController?.gameObject.SetActive(false);
        charController = charCustomizationController;
        charController.gameObject.SetActive(true);
        outfitPartsContainer = charController.partsContainer;
        CharCustomizationSaver.Instance.SetCharCustomizationController(charCustomizationController);
        UpdateOutfitSlotsUI();

    }

    public void UpdateOutfitSlotsUI()
    {
        foreach (var outfitGroup in outfitGroups)
        {
            var outfitSlots = outfitPartsContainer.GetPartsOfType(outfitGroup.outfitType);

            outfitGroup.ClearTextureVariants();
            
            foreach (var outfitSlot in outfitSlots)
            {
                foreach (var textureVariant in outfitSlot.textureVariants)
                {
                    outfitGroup.AddTextureVariant(textureVariantButtonPrefab, textureVariant, outfitSlot.Type);
                }
            }
        }
    }

    public void RequestOutfitEquip(int baseItemId, int textureVariantId)
    {
        charController.EquipPartById(baseItemId, textureVariantId);
    }
    
}





[Serializable]
public class CharOutfitGroup
{
    public OutfitType outfitType;
    public Transform variantButtonsParent;

    public void ClearTextureVariants()
    {
        foreach (Transform child in variantButtonsParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    
    public void AddTextureVariant(TextureVariantButton buttonPrefab, TextureVariant textureVariant, OutfitType type)
    {
        var button = GameObject.Instantiate(buttonPrefab, variantButtonsParent, false);
        button.InitButton(textureVariant, type);
    }

}

[Serializable]
public struct TextureVariant
{
    public int baseItemId;
    public int id;
    public string name;
    public Vector2 textureOffset;
    public Sprite icon;
}