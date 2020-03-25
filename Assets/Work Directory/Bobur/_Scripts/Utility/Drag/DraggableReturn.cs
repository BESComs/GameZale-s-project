using System;
using UnityAsync;
using UnityEngine;

namespace _Scripts.Utility.Drag
{
    [RequireComponent(typeof(Collider2D))]
    public class DraggableReturn : MonoBehaviour
    {
        private Vector3 _previouMousePos;
        private Vector3 _startPosition;
        private bool _goBack;
        public float returnSpeed = 2.8f;

        private void Start()
        {
            _goBack = true;
        }

        private void OnMouseDown()
        {
            _previouMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _startPosition = transform.localPosition;
        }

        private void OnMouseDrag()
        {
            Vector3 delta = _previouMousePos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition -= delta;
            _previouMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            _previouMousePos = Vector3.zero;
            
            if(_goBack)
                ReturnToInitialPosition();
        }

        private async void ReturnToInitialPosition()
        {
            for (float i = 0; i < 1f; i += Time.deltaTime * returnSpeed)
            {
                if(this == null) return; 
                i = (i > 1f) ? 1f : i;
                transform.localPosition = Vector3.Lerp(transform.localPosition, _startPosition, i);
                await Await.NextUpdate();
            }
        }

        public void GoingBack(bool value) => _goBack = value;
    }
}