using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "categoryData", menuName = "Category Data", order = 1)]
public class CategoryData : ScriptableObject
{
    private const string AssetsFolderName = "Assets";
    private const string AssetType = "t:" + nameof(LessonData);

    public int categoryId;
    public string categoryName;
    public Sprite categoryIcon;

    public List<LessonData> lessons;

#if UNITY_EDITOR

    private void OnEnable()
    {
        if (categoryId != 0) return;
        
        const string assetType = "t:" + nameof(CategoryData);
        var assetGuids = AssetDatabase.FindAssets(assetType, new[] {"Assets"});
        int maxId = 0;
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var category = AssetDatabase.LoadAssetAtPath<CategoryData>(assetPath);
            maxId = Mathf.Max(maxId, category.categoryId);
        }
        categoryId = maxId + 1;
    }


    [Button]
    public void LoadLessonsInFolder()
    {
        var folderPath = EditorUtility.OpenFolderPanel("Select folder", "Assets/1 Lessons Stuff/Lessons Data", "");
        var assetPathStart = folderPath.IndexOf(AssetsFolderName, StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets(AssetType, new[] {folderPath});
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var lessonAsset = AssetDatabase.LoadAssetAtPath<LessonData>(assetPath);
            lessons.Add(lessonAsset);
        }
    }

    [Button]
    public void Clear()
    {
        lessons.Clear();;
    }
    
#endif
}