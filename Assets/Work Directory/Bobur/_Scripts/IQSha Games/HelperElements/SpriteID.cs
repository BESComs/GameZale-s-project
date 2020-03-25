using Sirenix.OdinInspector;
using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements
{
    [HideReferenceObjectPicker]
    public class SpriteID
    {
        [SerializeField, PreviewField(30), HorizontalGroup("Group")] private Sprite sprite;
        [SerializeField, HorizontalGroup("Group"), ReadOnly] private int id;

        public int ID
        {
            get => id;
            set => id = value;
        }
        public Sprite Sprite => sprite;
    }
}