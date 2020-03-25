using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


public class OutfitPart : MonoBehaviour {

	public int id;
	public string itemName;
	public TextureOffsetChanger textureOffsetChanger;
	
 	public List<OutfitType> occupiedSlotsTypes;
	public List<Transform> linkedMeshes;
	public List<TextureVariant> textureVariants;

	public OutfitType Type => occupiedSlotsTypes[0];

	private void Awake()
	{
		textureOffsetChanger = GetComponent<TextureOffsetChanger>();
	}

	public bool IsTypeOf(OutfitType type)
	{
		return occupiedSlotsTypes.Contains(type);
	}

	public void EnableOutfit()
	{
		gameObject.SetActive(true);
		
		foreach (var linkedMesh in linkedMeshes)
			linkedMesh.gameObject.SetActive(true);
	}

	public void DisableOutfit()
	{
		gameObject.SetActive(false);
		
		foreach (var linkedMesh in linkedMeshes)
			linkedMesh.gameObject.SetActive(false);
	}

	public void SetTextureVariant(int textureVariantId)
	{
		var tv = textureVariants.Find(t => t.id == textureVariantId);
		textureOffsetChanger.SetOffset(tv.textureOffset);

		foreach (var linkedMesh in linkedMeshes)
		{
			var offsetChanger = linkedMesh.GetComponent<TextureOffsetChanger>();
			offsetChanger?.SetOffset(tv.textureOffset);
		}
	}
	
	
	
	#if UNITY_EDITOR

	[Button]
	public void LinkTextureVariants()
	{
		textureOffsetChanger = GetComponent<TextureOffsetChanger>();
	}
	
	[Button]
	public void InitTextureVariants()
	{
		for (var i = textureVariants.Count - 1; i >= 0; i--)
		{
			var tv = textureVariants[i];
			tv.baseItemId = id;
			textureVariants[i] = tv;
		}
	}
  
	[Button]
	public void CreateTextureVariants()
	{
		if (textureVariants.Count > 0) return;

		for (int i = 0; i < 4; i++)
		{
			var tv = new TextureVariant { baseItemId = id, id = i, textureOffset = new Vector2(i * 0.25f, 0)};
			textureVariants.Add(tv);
		}
	}
	
	#endif
	
	
}