using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameLabel;
    
    public LessonData lessonData;

    public void Init(LessonData lessonData)
    {
        this.lessonData = lessonData;
        icon.sprite = lessonData.lessonIcon;
        nameLabel.text = lessonData.lessonName;   
    }

    public void OnButtonClicked()
    {
        LessonsDatabase.Instance.SetOpenedLesson(lessonData.lessonId);
        LauncherV2.Instance.OpenLesson(lessonData);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (lessonData != null) Init(lessonData);
    }
#endif
}