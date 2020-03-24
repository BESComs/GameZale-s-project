using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "outfitPreset", menuName = "Outfit Preset", order = 1)]
public class OutfitPreset : ScriptableObject
{
    public static int globalId = 1;
    
    public int id;
    public int price = 100;
    
    public Sprite icon;
    
    public string meshName;

    
    public Texture texture;
    public Vector2 textureOffset;
    public Vector2 textureScale;
    
    
    

}
