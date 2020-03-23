using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable SuggestVarOrType_BuiltInTypes

public class GradeView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameLabel;

    public int gradeNumber;
    public string gradeName;
    public Sprite gradeIcon;

    public List<CategoryData> categories;

    public void OnButtonClicked()
    {
        LauncherV2.Instance.OpenGrade(this);
    }
    

#if UNITY_EDITOR
    private void OnValidate()
    {
        icon.sprite = gradeIcon;
        nameLabel.text = gradeName;
    }

    [Button]
    public void LoadCategoryInFolder()
    {
        const string assetsFolderName = "Assets";
        const string assetType = "t:" + nameof(CategoryData);
        const string defaultFolder = "Assets/Categories Data";
        const string title = "Select folder";
        
        var folderPath = EditorUtility.OpenFolderPanel(title, defaultFolder, "");
        var assetPathStart = folderPath.IndexOf(assetsFolderName, StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets(assetType, new[] {folderPath});
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var categoryAsset = AssetDatabase.LoadAssetAtPath<CategoryData>(assetPath);
            categories.Add(categoryAsset);
        }
    }

    [Button]
    public void ClearCategories()
    {
        categories.Clear();
    }
    
#endif
}