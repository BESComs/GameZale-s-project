using UnityEngine;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    public class ColoringSetColor : MonoBehaviour
    {
        //скрипт висит на всех частях раскраски
        public int currentIndex;
        public SpriteRenderer spriteRenderer;
        //Используется для определения вверхнего спрайта 
        //переменная нужна так как спрайты не полностью нарезаны
        public int spriteSo;
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteSo = spriteRenderer.sortingOrder;
        }       
    }
}
