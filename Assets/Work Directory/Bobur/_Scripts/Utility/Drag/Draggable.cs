using UnityEngine;

namespace _Scripts.Utility.Drag
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        private Vector3 _previouMousePos;
        
        private void OnMouseDown()
        {
            _previouMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        }
    }
}