using System;
using UnityEngine;

namespace _Scripts.Utility
{
    [ExecuteInEditMode]
    public class Clickable : MonoBehaviour
    {
        private Action callback;

        private void OnEnable()
        {
            if (GetComponent<Collider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }

        private void OnMouseDown()
        {
            callback?.Invoke();
        }

        public Collider2D Collider2D => GetComponent<Collider2D>();

        public void SetCallback(Action func) => callback = func;
        public void DisableClick() => Collider2D.enabled = false;
        public void EnableClick() => Collider2D.enabled = true;
    }
}