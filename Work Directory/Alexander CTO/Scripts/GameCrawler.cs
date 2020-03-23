 using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
 using UnityEngine.Serialization;
 using Work_Directory.Denis.Scenes.NewLauncher.Scripts;

 [Obsolete("Used to grab Denchiks and Bobur lesson data")]
public class GameCrawler : SerializedMonoBehaviour
{
#if UNITY_EDITOR
    

    public LessonPack lessonPack;
    
    public Dictionary<string, string> lessons = new Dictionary<string, string>();
    
    
    [Button]
    private void GrabGamesInfo()
    {
        var games = GetComponentsInChildren<FolderController>(true);
        var lessons = new List<FolderController>(games);
        lessons = lessons.FindAll(c => c.isGame);

        const int extensionLength = 6;

        var uniqueThemes = new List<string>();
        var lessonThemes = new List<(string, string)>();

        var lessonId = 1;
        
        foreach (var lesson in lessons)
        {
            var lessonPath = lesson.gamePath;
            var lastSlashIndex = lessonPath.LastIndexOf('/') + 1;
            var lessonName = lessonPath.Substring(lastSlashIndex, lessonPath.Length - lastSlashIndex - extensionLength);
            var themeName = lesson.transform.parent.name;
            
            lessonThemes.Add((lessonName, themeName));

            if (!uniqueThemes.Contains(themeName))
                uniqueThemes.Add(themeName);

            var lessonInfo = new LessonInfo { uniqueId = lessonId, title = lessonName, themeId = uniqueThemes.IndexOf(themeName) + 1 };
            lessonPack.AddLesson(lessonInfo);

        }

        for (int i = 0; i < uniqueThemes.Count; i++)
        {
            var themeInfo = new ThemeInfo {id = i + 1, subjectId = 1, title = uniqueThemes[i]};
            lessonPack.AddTheme(themeInfo);
        }
        
        lessonPack.Save();

    }


    [Button]
    public void CheckForDuplicates()
    {
        var games = GetComponentsInChildren<FolderController>(true);
        var lessons = new List<FolderController>(games);
        lessons = lessons.FindAll(c => c.isGame);

        const int extensionLength = 6;

        var uniqueLessons = new HashSet<string>();
        
        foreach (var lesson in lessons)
        {
            var lessonPath = lesson.gamePath;
            var lastSlashIndex = lessonPath.LastIndexOf('/') + 1;
            var lessonName = lessonPath.Substring(lastSlashIndex, lessonPath.Length - lastSlashIndex - extensionLength);

            if(!uniqueLessons.Add(lessonName)) print(lessonName);
            
        }
        
    }
#endif    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

