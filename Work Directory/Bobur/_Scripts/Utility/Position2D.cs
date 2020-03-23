using System;
using UnityEngine;

namespace _Scripts.Utility
{
    [Serializable]
    public class Position2D
    {
       [SerializeField] private float _x;
       [SerializeField] private float _y;

        public Position2D()
        {
            _x = 0f;
            _y = 0f;
        }

        public Position2D(float x, float y)
        {
            _x = x;
            _y = y;
        }
        
        public float X
        {
            get { return _x; }
        }

        public float Y
        {
            get { return _y; }
        }
    }
}