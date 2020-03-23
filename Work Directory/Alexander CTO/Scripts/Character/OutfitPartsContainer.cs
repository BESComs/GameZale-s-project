using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class OutfitPartsContainer : SerializedMonoBehaviour
{
	public List<OutfitPart> outfitParts;
	public Dictionary<OutfitType, OutfitPart> defaultSlots;
	
	public OutfitPart[] GetPartsOfType(OutfitType type)
	{
		var outfitParts = this.outfitParts.FindAll(op => op.Type == type);
		return outfitParts.ToArray();
	}

	public OutfitPart GetPartByName(string name)
	{
		var outfitPart = outfitParts.Find(op => op.itemName == name);
		return outfitPart;
	}

	public OutfitPart GetPartById(int id)
	{
		var outfitPart = outfitParts.Find(op => op.id == id);
		return outfitPart;
	}
	

#if UNITY_EDITOR

	[Button]
	public void InitializeContainer()
	{
		var childOutfitParts = GetComponentsInChildren<OutfitPart>(true);
		outfitParts = new List<OutfitPart>(childOutfitParts);
	}

	[Button]
	public void InitializePartsIds()
	{
		for (var i = 0; i < outfitParts.Count; i++)
		{
			outfitParts[i].id = i;
		}
		
	}

	public string iconSpritesFilter;
	public List<Sprite> textureIcons;

	
	[Button]
	void LoadTextureIcons()
	{
		textureIcons = Resources.LoadAll<Sprite>("/").ToList();
		textureIcons.RemoveAll(s => s.name.Contains(iconSpritesFilter) == false);
	}
	
	[Button]
	void InitializeTextureIcons()
	{
		int currentIconIndex = 0;

		foreach (var outfitPart in outfitParts)
		{
			for (var index = 0; index < outfitPart.textureVariants.Count; index++)
			{
				var outfitPartTextureVariant = outfitPart.textureVariants[index];
				outfitPartTextureVariant.icon = textureIcons[currentIconIndex];
				outfitPart.textureVariants[index] = outfitPartTextureVariant;
				currentIconIndex++;
			}
		}
		
	}

	[Button]
	public void InitializeDefaultSlots()
	{
		foreach (var part in outfitParts)
		{
			if (part.GetComponent<DefaultOufitSlot>() != null)
			{
				defaultSlots[part.Type] = part;
			}
		}

	}
	
	
#endif

	
}