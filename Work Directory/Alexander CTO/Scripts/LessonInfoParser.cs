using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LessonInfoParser : MonoBehaviour
{
    public LessonPack lessonPack;

#if UNITY_EDITOR


    [Button]
    public void RecrecateLessonPack()
    {
        lessonPack.ClearSubjects();
        lessonPack.ClearThemes();
        lessonPack.ClearLessons();

        var subjectUniqueId = 0;
        var themeUniqueId = 0;
        var lessonUniqueId = 0;

        var subjects = GetComponentsInChildren<GradeView>();

        foreach (var subject in subjects)
        {
            subjectUniqueId++;
            var subjectInfo = new SubjectInfo {id = subjectUniqueId, title = subject.gradeName};
            lessonPack.AddSubject(subjectInfo);

            foreach (var category in subject.categories)
            {
                themeUniqueId++;
                var themeInfo = new ThemeInfo
                    {id = themeUniqueId, subjectId = subjectUniqueId, title = category.categoryName};
                lessonPack.AddTheme(themeInfo);

                foreach (var lesson in category.lessons)
                {
                    lessonUniqueId++;
                    var lessonInfo = new LessonInfo
                    {
                        uniqueId = lessonUniqueId, lessonId = lesson.lessonId, themeId = themeUniqueId,
                        title = lesson.lessonName, maxScore = lesson.maxScore
                    };
                    lessonPack.AddLesson(lessonInfo);
                }
            }
        }

        lessonPack.Save();
    }

    [Button]
    public void UpdateLessonPack()
    {
        var maxThemeUniqueId = 0;
        var maxLessonUniqueId = 0;

        var newSubjects = new List<GradeView>();
        var newThemes = new List<(int subjectId, CategoryData category)>();
        var newLessons = new List<(int themeId, LessonData lesson)>();

        var subjects = GetComponentsInChildren<GradeView>();

        foreach (var subject in subjects)
        {
            var subjectId = subject.gradeNumber;
            if (lessonPack.HasSubject(subjectId) == false)
                newSubjects.Add(subject);

            foreach (var category in subject.categories)
            {
                var themeId = category.categoryId;
                if (lessonPack.HasTheme(themeId) == false)
                    newThemes.Add((subjectId, category));
                else
                    maxThemeUniqueId = Mathf.Max(maxThemeUniqueId, themeId);

                foreach (var lesson in category.lessons)
                {
                    var lessonId = lesson.lessonId;
                    if (lessonPack.HasLesson(themeId, lesson.lessonId) == false)
                        newLessons.Add((themeId, lesson));
                    else
                    {
                        var uniqueLessonId = lessonPack.GetUniqueLessonId(themeId, lessonId);
                        maxLessonUniqueId = Mathf.Max(maxLessonUniqueId, uniqueLessonId);
                    }
                }
            }
        }

        foreach (var subject in newSubjects)
            lessonPack.AddSubject(new SubjectInfo {id = subject.gradeNumber, title = subject.gradeName});

        foreach ((int subjectId, var category) in newThemes)
        {
            maxThemeUniqueId++;
            var info = new ThemeInfo {id = maxThemeUniqueId, title = category.categoryName, subjectId = subjectId};
            lessonPack.AddTheme(info);
        }

        foreach ((int themeId, var lesson) in newLessons)
        {
            maxLessonUniqueId++;
            var lessonInfo = new LessonInfo
            {
                uniqueId = maxLessonUniqueId,
                lessonId = lesson.lessonId,
                title = lesson.lessonName,
                maxScore = lesson.maxScore,
                themeId = themeId
            };
            lessonPack.AddLesson(lessonInfo);
        }
        lessonPack.Save();
    }


#endif
}