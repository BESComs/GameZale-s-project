using System;
using UnityEngine;

namespace _Scripts.PictureMatchGame
{
    [Serializable]
    public class SpriteObject
    {
        [SerializeField] private int Id;
        [SerializeField] private Sprite sprite;

        public Sprite GetSprite()
        {
            return sprite;
        }

        public int GetId()
        {
            return Id;
        }

    }
}