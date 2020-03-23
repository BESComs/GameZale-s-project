using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FurnitureSaver : MonoBehaviour
{
    [System.Serializable]
    public class FurnitureList
    {
        public List<FurnitureData> data = new List<FurnitureData>();
    }

    public const string FurnitureDataKey = "FurnitureData";
    
    private void Start()
    {
        if (FurnitureSaveAvaliable())
        {
            LoadFurnitureData();
        }
    }

    public bool FurnitureSaveAvaliable()
    {
        return PlayerPrefs.HasKey(FurnitureDataKey);
    }

    [Button]
    public void SaveFurnitureData()
    {
       var childFurniture = new List<FurnitureController>(GetComponentsInChildren<FurnitureController>());
       var furnitureList = new FurnitureList();

       foreach (var cf in childFurniture)
       {
           var fData = new FurnitureData
           {
               name = cf.name,
               localPosition = cf.transform.localPosition,
               localRotation = cf.transform.localRotation,
               currentMeshIndex = cf.currentMeshIndex
           };
           furnitureList.data.Add(fData);
       }

       var jsonData = JsonUtility.ToJson(furnitureList);
       PlayerPrefs.SetString(FurnitureDataKey, jsonData);
       PlayerPrefs.Save();
    }

    
    [Button]
    public void LoadFurnitureData()
    {
        var childFurniture = new List<FurnitureController>(GetComponentsInChildren<FurnitureController>());
        var jsonData = PlayerPrefs.GetString(FurnitureDataKey);
        var furnitureList = JsonUtility.FromJson<FurnitureList>(jsonData);
        foreach (var furnitureData in furnitureList.data)
        {
            var furniture = childFurniture.Find(f => f.name == furnitureData.name);
            furniture.transform.localPosition = furnitureData.localPosition;
            furniture.transform.localRotation = furnitureData.localRotation;
            furniture.SelectMesh(furnitureData.currentMeshIndex);
        }
    }
    
}
