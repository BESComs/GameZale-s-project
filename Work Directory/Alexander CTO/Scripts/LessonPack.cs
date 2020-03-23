using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "lessonPack", menuName = "Lesson Pack", order = 1)]
public class LessonPack : ScriptableObject
{
    [SerializeField]
    private List<SubjectInfo> subjectsInfo = new List<SubjectInfo>();
    [SerializeField]
    private List<ThemeInfo> themesInfo = new List<ThemeInfo>();
    [SerializeField]
    private List<LessonInfo> lessonsInfo = new List<LessonInfo>();
    
    [Button]
    public void ClearSubjects()
    {
        subjectsInfo.Clear();
    }

    [Button]
    public void ClearThemes()
    {
        themesInfo.Clear();;
    }

    [Button]
    public void ClearLessons()
    {
        lessonsInfo.Clear();
    }
    
    public void AddSubject(SubjectInfo subjectInfo)
    {
        subjectsInfo.Add(subjectInfo);
    }

    public void AddTheme(ThemeInfo themeInfo)
    {
        themesInfo.Add(themeInfo);
    }

    public void AddLesson(LessonInfo lessonInfo)
    {
        lessonsInfo.Add(lessonInfo);
    }

    public bool HasSubject(int subjectId)
    {
        return subjectsInfo.Exists(s => s.id == subjectId);
    }

    public bool HasTheme(int themeId)    
    {
        return themesInfo.Exists(t => t.id == themeId);
    }

    public bool HasLesson(int lessonThemeId, int lessonId)
    {
        return lessonsInfo.Exists(l => l.lessonId == lessonId && l.themeId == lessonThemeId);
    }

    public void SetLessonMaxScore(string lessonName, int maxScore)
    {
        try
        {
            var lessonIndex = lessonsInfo.FindIndex(l => l.title == lessonName);
            var lessonInfo = lessonsInfo[lessonIndex];
            lessonInfo.maxScore = maxScore;
            lessonsInfo[lessonIndex] = lessonInfo;

        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogError(e.Message + "\n" + e.Data);
            
        }
    }

    #if UNITY_EDITOR
    
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button]
    public void GenerateCsv()
    {
        const string subjectTitle = "id;title";
        const string themeTitle = "id;subjectId;title";
        const string lessonTitle = "id;themeId;title;maxScore;maxCoins";

        const string subjectsFileName = "/subjects.csv";
        const string themesFileName = "/themes.csv";
        const string lessonsFileName = "/exercises.csv";
        
        var savePath = EditorUtility.SaveFolderPanel("Сохранить в папку", "", "");
        if (!string.IsNullOrEmpty(savePath))
        {
            var sb = new StringBuilder();
            sb.AppendLine(subjectTitle);
            foreach (var subject in subjectsInfo)
                sb.AppendLine($"{subject.id};{subject.title}");
            File.WriteAllText(savePath + subjectsFileName, sb.ToString(), Encoding.UTF8);
            sb.Clear();
            sb.AppendLine(themeTitle);
            foreach (var theme in themesInfo)
                sb.AppendLine($"{theme.id};{theme.subjectId};{theme.title}");
            File.WriteAllText(savePath + themesFileName, sb.ToString(), Encoding.UTF8);
            sb.Clear();
            sb.AppendLine(lessonTitle);
            foreach (var lesson in lessonsInfo)
                sb.AppendLine($"{lesson.uniqueId};{lesson.themeId};{lesson.title};{lesson.maxScore};{lesson.maxScore}");
            File.WriteAllText(savePath + lessonsFileName, sb.ToString(), Encoding.UTF8);
        }
    }
    #endif

    public int GetUniqueLessonId(int themeId, int lessonId)
    {
        return lessonsInfo.Find(l => l.themeId == themeId && l.lessonId == lessonId).uniqueId;
    }
}

[Serializable]
public struct SubjectInfo
{
    public int id;
    public string title;
}

[Serializable]
public struct ThemeInfo
{
    public int id;
    public int subjectId;
    public string title;
}

[Serializable]
public struct LessonInfo
{
    public int uniqueId;
    public int lessonId;
    public int themeId;
    public string title;
    public int maxScore;
}