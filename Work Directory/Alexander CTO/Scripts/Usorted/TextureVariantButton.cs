using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureVariantButton : MonoBehaviour
{
    private OutfitType type;
    
    public TextureVariant textureVariant;
    
    public Image icon;

    public void InitButton(TextureVariant textureVariant, OutfitType type)
    {
        this.textureVariant = textureVariant;
        this.type = type;
        icon.sprite = textureVariant.icon;
    }

    
    public void OnButtonClick()
    {
        CharCustomizationPanel.Instance.RequestOutfitEquip(textureVariant.baseItemId, textureVariant.id);
    }
    
}