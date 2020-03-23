using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class OutfitControllerNew : MonoBehaviour
{
    public List<OutfitSlot> outfitSlots;

    public List<OutfitPreset> outfitPresets;

    public Sprite defaultIcon;

    private void Awake()
    {
        outfitSlots = new List<OutfitSlot>(GetComponentsInChildren<OutfitSlot>(true));
    }

    public List<OutfitPreset> FindPresetsOfType(OutfitType outfitType)
    {
        var outfitSlotsMatch = outfitSlots.FindAll(os => os.outfitType == outfitType);
        var foundOutfitPresets = new List<OutfitPreset>();
        foreach (var slot in outfitSlotsMatch)
        {
            var slotMatchedPresets = outfitPresets.FindAll(op => op.meshName == slot.name);
            foundOutfitPresets.AddRange(slotMatchedPresets);
        }
        return foundOutfitPresets;
    }
    
    public void PreviewOutfit(OutfitPreset outfitPreset)
    {
        var outfitSlot = outfitSlots.Find(os => os.name == outfitPreset.meshName);
        var prevOutfitSlots = outfitSlots.FindAll(os => os.outfitType == outfitSlot.outfitType);
        
        foreach (var slot in prevOutfitSlots) slot.gameObject.SetActive(false);
        
        
        outfitSlot.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture = outfitPreset.texture;
        outfitSlot.gameObject.SetActive(true);
    }
    
#if UNITY_EDITOR

    [Button]
    public void LoadOutfitInFolder()
    {
        const string AssetsFolderName = "Assets";
        const string AssetType = "t:" + nameof(OutfitPreset);
        var folderPath = EditorUtility.OpenFolderPanel("Select folder", "Assets/3 Textures/OutfitPresets", "");
        var assetPathStart = folderPath.IndexOf(AssetsFolderName, StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets(AssetType, new[] {folderPath});
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var outfitPreset = AssetDatabase.LoadAssetAtPath<OutfitPreset>(assetPath);
            outfitPresets.Add(outfitPreset);
        }
    }

    [Button]
    public void InitStuff(bool resetGlobalId)
    {
        var childOutfitSlots = GetComponentsInChildren<OutfitSlot>(true);
        var textures = new List<Texture>(Resources.LoadAll<Texture>("CharTextures"));

        if (resetGlobalId) OutfitPreset.globalId = 1;

        foreach (var outfitSlot in childOutfitSlots)
        {
            var outfitSlotLower = outfitSlot.name.ToLower();
            TrimOutfitName(ref outfitSlotLower);
            var slotTextures = textures.FindAll(t => t.name.ToLower().Contains(outfitSlotLower));
            foreach (var texture in slotTextures)
            {
                var outfitPreset = ScriptableObject.CreateInstance<OutfitPreset>();
                outfitPreset.id = OutfitPreset.globalId++;
                outfitPreset.meshName = outfitSlot.name;
                outfitPreset.icon = defaultIcon;
                outfitPreset.price = 100;
                outfitPreset.texture = texture;
                outfitPreset.textureScale = Vector2.one;
                outfitPreset.textureOffset = Vector2.zero;
                var assetPath = "Assets/3 Textures/OutfitPresets/" + name + "_outfit_" + outfitPreset.id + ".asset";
                AssetDatabase.CreateAsset(outfitPreset, assetPath);
                print(outfitPreset.id);
            }
        }

        AssetDatabase.SaveAssets();
    }
    

    public static void TrimOutfitName(ref string outfitName)
    {
        const string delimiter = "::";
        if (!outfitName.Contains(delimiter)) return;

        var delimiterIndex = outfitName.IndexOf(delimiter, StringComparison.Ordinal);
        outfitName = outfitName.Substring(0, delimiterIndex);
    }


    [Button]
    public void GenerateOutfitDataCSV()
    {
        var sb = new StringBuilder();
        foreach (var preset in outfitPresets)
        {
            var presetString = $"{preset.id};{preset.meshName}_{preset.id};{preset.price}";
            sb.AppendLine(presetString);
        }
        print(sb.ToString());
    }
    
    
#endif
    
}