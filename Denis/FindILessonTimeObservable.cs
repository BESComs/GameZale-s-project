
using System;
using System.Linq;
using UnityEngine;

namespace Work_Directory.Denis
{
    public class FindILessonTimeObservable : MonoBehaviour
    {
        public string LessonCountOnScene()
        {
            var tmp = FindObjectsOfType<Transform>();
            var tmp1 = tmp.Select(transform1 => transform1.GetComponent<ILessonStatsObservable>()).Where(tmp3 => tmp3 != null).ToList();
            var levels = string.Empty;
            if (tmp1.Count == 1)
            {
                levels = tmp1[0].MaxScore.ToString();
            }
            return tmp1.Count + " " + levels;
        }
        
        public int MaxScore()
        {
            var tmp = FindObjectsOfType<Transform>();
            var tmp1 = tmp.Select(transform1 => transform1.GetComponent<ILessonStatsObservable>()).Where(tmp3 => tmp3 != null).ToList();
            if (tmp1.Count == 1)
            {
                return tmp1[0].MaxScore;
            }
            return -1;
        }
        
    }
}
