using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class LessonStatistic 
{    
    static int score = 0;
    static bool hasStart, hasDuration;
    static float startLessonUnityTime, lessonUnityDuration;
    static DateTime startLessonTime;
    private static bool isRight;
    public static void SetScore(int increaseScore){
        score += increaseScore;
        Debug.Log("Score: " + score);
    }
    public static void SetStartLessonTime() {
        ResetStatistic();
        startLessonTime = System.DateTime.Now;
        startLessonUnityTime = Time.time;
        hasStart = true;
#if  UNITY_EDITOR
        try
        {
            Debug.Log("LessonStart: "+startLessonTime.ToString() + 
                      " LessonID "+LessonsDatabase.Instance.GetCurrentLessonId());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
#endif
    }
    public static void SetLessonDurationWithEndLessonTime(){
        lessonUnityDuration = Time.time - startLessonUnityTime;
        hasDuration = true;
        Debug.Log("Duration: " + lessonUnityDuration);
    }

    public static void SetRight(bool right)
    {
        isRight = right;
    }
    public static int GetScore() { return score; }
    public static DateTime GetStartLessonTime() { return startLessonTime; }
    public static string GetStartLessonTimeStr() { return startLessonTime.ToString("s"); }
    public static float GetStartLessonUnityTime() { return startLessonUnityTime; }
    public static float GetLessonUnityDuration() { return lessonUnityDuration; }
    public static bool GetRight() {return isRight;}
    public static bool HasStartAndDuration() {
        SetLessonDurationWithEndLessonTime();
        return hasStart && hasDuration;
        }
    public static void ResetStatistic(){
        score = 0;
        startLessonUnityTime = 0;
        lessonUnityDuration = 0;
        startLessonTime = new DateTime();
        hasStart = false;
        hasDuration = false;
        isRight = false;
    }
}
