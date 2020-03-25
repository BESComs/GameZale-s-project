using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "lessonData", menuName = "Lesson Data", order = 1)]
public class LessonData : ScriptableObject
{
    [ReadOnly]
    public int lessonId;
    public int maxScore;
    public string lessonName;
    public Sprite lessonIcon;
    public string scenePath;

#if UNITY_EDITOR
    public SceneAsset sceneAsset;
    public void OnValidate()
    {
        if (sceneAsset != null)
        {
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
        }
    }
#endif
}