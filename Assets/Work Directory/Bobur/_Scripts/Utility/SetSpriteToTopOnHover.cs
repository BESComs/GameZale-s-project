using UnityEngine;
using Work_Directory.Bobur._Scripts.JigsawPuzzle;

namespace DefaultNamespace
{
    public class SetSpriteToTopOnHover : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseEnter()
        {
            if(_spriteRenderer != null)
                _spriteRenderer.sortingOrder = 1;
        }

        private void OnMouseExit()
        {
            if (GetComponent<JigsawDragHandler>() != null && !GetComponent<JigsawDragHandler>().movingBack)
                _spriteRenderer.sortingOrder = 0;
        }
    }
}