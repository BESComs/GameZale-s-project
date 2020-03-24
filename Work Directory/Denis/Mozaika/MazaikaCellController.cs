using UnityEngine;

namespace Work_Directory.Denis.Mozaika
{
    public class MazaikaCellController : MonoBehaviour
    {
        // скрипт отвечающий за покраску элемента мозаики
        public Color currentColor;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentColor = spriteRenderer.color;
        }

        private void OnMouseDown()
        {
            MazaikaManager.Instance.SetColor(this);
            spriteRenderer.color = currentColor;
        }
    }
}
