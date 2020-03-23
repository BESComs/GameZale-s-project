using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using Work_Directory.Denis.Scenes.NewLauncher.Scripts;

#endif

public class LessonDataConverter : SerializedMonoBehaviour
{
    public Dictionary<string, string> folders = new Dictionary<string, string>();

    public List<(string, LessonData)> lessons;


#if UNITY_EDITOR
    [Button]
    public void GrabLessonsData()
    {
        lessons.Clear();
        var allPossibleGames = GetComponentsInChildren<FolderController>(true).ToList();
        var games = allPossibleGames.FindAll(fc => fc.isGame);

        for (var i = 0; i < games.Count; i++)
        {
            var game = games[i];
            var categoryName = game.transform.parent.name;
            var gameName = game.GetComponent<TextMeshPro>().text;
            var gameIcon = game.transform.GetComponentInChildren<SpriteRenderer>().sprite;
            var lessonData = ScriptableObject.CreateInstance<LessonData>();
            lessonData.lessonName = gameName;
            lessonData.lessonIcon = gameIcon;
            lessonData.sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(game.gamePath);
            lessonData.OnValidate();
            lessons.Add((categoryName, lessonData));
        }
    }

    [Button]
    public void CreateLessonsData()
    {
        for (var i = 0; i < lessons.Count; i++)
        {
            var (categoryName, lessonData) = lessons[i];
            var folderPath = GetOrCreateFolderPath(categoryName);
            var scenePath = lessonData.scenePath;
            var lastIndex = scenePath.LastIndexOf('/');
            var assetName = scenePath.Substring(lastIndex);
            assetName = assetName.Substring(1, assetName.Length - 7);
            var assetPath = Path.Combine(folderPath, assetName) + ".asset";
            SaveLessonData(assetPath, lessonData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string GetOrCreateFolderPath(string folderName)
    {
        if (folders.ContainsKey(folderName))
        {
            return folders[folderName];
        }
        else
        {
            var guid = AssetDatabase.CreateFolder("Assets/Lessons Data", folderName);
            var newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            folders.Add(folderName, newFolderPath);
            return newFolderPath;
        }
    }

    private void SaveLessonData(string path, LessonData lessonData)
    {
        AssetDatabase.CreateAsset(lessonData, path);
    }

    public LessonPack lp;
    
    [Button, Obsolete("Useless, dont need it")]
    public void UpdateMaxScores()
    {
        var folderPath = EditorUtility.OpenFolderPanel("Select folder", "Assets/Lessons Data", "");
        var assetPathStart = folderPath.IndexOf("Assets", StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets("t:LessonData", new[] {folderPath});
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var lessonAsset = AssetDatabase.LoadAssetAtPath<LessonData>(assetPath);
            EditorUtility.SetDirty(lessonAsset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [Button]
    public void SetLessonIds()
    {
        var folderPath = EditorUtility.OpenFolderPanel("Select folder", "Assets/Lessons Data", "");
        var assetPathStart = folderPath.IndexOf("Assets", StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets("t:LessonData", new[] {folderPath});
        int id = 1;
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var lessonAsset = AssetDatabase.LoadAssetAtPath<LessonData>(assetPath);
            lessonAsset.lessonId = id;
            id++;
            EditorUtility.SetDirty(lessonAsset);
        }
    }

    [Button]
    public void SetCategoryIds()
    {
        var folderPath = EditorUtility.OpenFolderPanel("Select folder", "Assets/Categories Data", "");
        var assetPathStart = folderPath.IndexOf("Assets", StringComparison.Ordinal);
        folderPath = folderPath.Substring(assetPathStart);
        var assetGuids = AssetDatabase.FindAssets("t:CategoryData", new[] {folderPath});
        int id = 1;
        foreach (var assetGuid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var categoryAsset = AssetDatabase.LoadAssetAtPath<CategoryData>(assetPath);
            categoryAsset.categoryId = id;
            id++;
            EditorUtility.SetDirty(categoryAsset);
        }
    }
    
    
    

#endif
}