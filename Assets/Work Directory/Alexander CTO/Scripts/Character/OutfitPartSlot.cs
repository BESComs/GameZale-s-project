using System;


[Serializable]
public class OutfitPartSlot
{
	public OutfitType type;

	public OutfitPart  equippedPart { get; private set; }

	public bool isEmpty { get; private set; } = true;

	public int BaseItemId;
	public int TextureVariantId;

	public void EquipOutfit(OutfitPart outfitPart, int baseItemId, int textureVariantiD)
	{
		equippedPart = outfitPart;
		BaseItemId = baseItemId;
		TextureVariantId = textureVariantiD;
		isEmpty = false;
		
		equippedPart.EnableOutfit();
	}

	public void RemoveOutfit()
	{
		if (isEmpty == true) return;

		equippedPart.DisableOutfit();	
		isEmpty = true;
	}

}