using System;
using UnityEngine;

namespace _Scripts.Utility
{
    public class ID : MonoBehaviour
    {
        [SerializeField] private int _id;

        public int Id
        {
            get => _id;
            set => _id = value;
        }
    }
}