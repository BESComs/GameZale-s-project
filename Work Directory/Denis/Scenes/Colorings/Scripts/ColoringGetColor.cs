using UnityEngine;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    public class ColoringGetColor : MonoBehaviour
    {
        
        private Color _currentColor;
        public Coloring coloring;
        
        private void Awake()
        {
            _currentColor = GetComponent<SpriteRenderer>().color;
        }
        //делает выбранный цвет текущим для скрипта Coloring
        private void OnMouseDown()
        {
            coloring.selectedColor.color = _currentColor;
            coloring.currentSelectColor = _currentColor;
        }
    }
}
