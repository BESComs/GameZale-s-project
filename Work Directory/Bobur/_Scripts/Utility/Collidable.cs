using System;
using UnityEngine;

namespace _Scripts.Utility
{
    [RequireComponent(typeof(Collider2D))]
    public class Collidable : MonoBehaviour
    {
        private Action<Collision2D> _onCollisionEnter;
        private Action<Collision2D> _onCollisionExit;

        public void SetOnCollisionEnter(Action<Collision2D> func) => _onCollisionEnter = func;
        public void SetOnCollisionExit(Action<Collision2D> func) => _onCollisionExit = func;

        private void OnCollisionEnter2D(Collision2D other)
        {
            _onCollisionEnter.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            _onCollisionExit.Invoke(other);
        }
    }
}