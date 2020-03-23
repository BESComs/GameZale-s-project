using UnityEngine;

namespace Work_Directory.Denis.Scripts
{
    public class ButtonEffect : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseExit()
        {
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = 1f;
            _spriteRenderer.color = spriteRendererColor;
        }

        private void OnMouseEnter()
        {
        
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = .75f;
            _spriteRenderer.color = spriteRendererColor;
        }
    }
}
