using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class CharCustomizationController : SerializedMonoBehaviour
{
	public GenderData gender;
	
	public OutfitPartsContainer partsContainer;
	public List<OutfitPartSlot> outfitSlots;

	public bool loadCustomizationData;

	private CharCustomizationData customizationData;
	
	private void Start()
	{
		foreach (var part in partsContainer.outfitParts)
		{
			part.DisableOutfit();
		}
		
		foreach (var outfitSlot in outfitSlots)
		{
			var defaultOutfitPart = partsContainer.defaultSlots[outfitSlot.type];
			outfitSlot.EquipOutfit(defaultOutfitPart, defaultOutfitPart.id, 0);
		}

		if (loadCustomizationData)
		{
			customizationData = CharCustomizationSaver.Instance.LoadData();
			EquipSavedPart(customizationData.hatData);
			EquipSavedPart(customizationData.hairData);
			EquipSavedPart(customizationData.torsoData);
			EquipSavedPart(customizationData.braceLeftData);
			EquipSavedPart(customizationData.braceRightData);
			EquipSavedPart(customizationData.legsData);
			EquipSavedPart(customizationData.footwearData);
		}
	}

	private void EquipSavedPart(TextureVariantData data)
	{
		EquipPartById(data.baseItemId, data.textureVariantId);
	}
	
	public void EquipPartById(int baseItemId, int textureVariantId)
	{
		var newOutfitPart = partsContainer.GetPartById(baseItemId);
		var occupiedSlotTypes = newOutfitPart.occupiedSlotsTypes;
		var oldSlots = FindSlotsForOutfit(newOutfitPart);

		foreach (var oldSlot in oldSlots)
		{
			oldSlot.RemoveOutfit();
		}
		
		foreach (var partType in occupiedSlotTypes)
		{  
			var slot = outfitSlots.Find(s => s.type == partType);
			slot.RemoveOutfit();
			slot.EquipOutfit(newOutfitPart, baseItemId, textureVariantId);
		}
		
		foreach (var slot in outfitSlots)
		{
			if (slot.isEmpty)
			{
				var defaultOutfitPart = partsContainer.defaultSlots[slot.type];
				slot.EquipOutfit(defaultOutfitPart, defaultOutfitPart.id, 0);
			}
		}
		
		newOutfitPart.SetTextureVariant(textureVariantId);
	}
	
	
	public OutfitPartSlot[] FindSlotsForOutfit(OutfitPart outfitPart)
	{
		var oldOutfitPart = outfitSlots.Find(s => s.type == outfitPart.Type).equippedPart;
		var oldSlots = outfitSlots.FindAll(s => oldOutfitPart.occupiedSlotsTypes.Contains(s.type));

		return oldSlots.ToArray();
	}



	#if UNITY_EDITOR
	[Button]
	public void InitializeOutfitSlots()
	{
		foreach (OutfitType outfitPartType in Enum.GetValues(typeof(OutfitType)))
		{
			var outfitPartSlot = new OutfitPartSlot();
			outfitPartSlot.type = outfitPartType;
			outfitSlots.Add(outfitPartSlot);
		}		
	}
	
	
	
	
	#endif
	
	
	
	
	
	
	
	
	
	
	
}