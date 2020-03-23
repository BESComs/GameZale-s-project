using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonsDatabase : MonoBehaviour
{
    public static LessonsDatabase Instance { get; private set; }

    private int lastOpenedTheme;
    private int lastOpenedLesson;
    
    public LessonPack lessonPack;
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetOpenedTheme(int themeNumber)
    {
        lastOpenedTheme = themeNumber;
    }

    public void SetOpenedLesson(int lessonNumber)
    {
        lastOpenedLesson = lessonNumber;
    }

    public int GetCurrentLessonId()
    {
        return lessonPack.GetUniqueLessonId(lastOpenedTheme, lastOpenedLesson);
    }
    

        
    
}