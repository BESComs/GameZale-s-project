using UnityEngine;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    public class UndoRedoColoring : MonoBehaviour
    {
        public Coloring coloring;
        public bool undo;
        
        //Кнопка шага назад/вперед для игры раскраски
        private void OnMouseDown()
        {
            if (undo)
                coloring.UndoColoring();
            else
                coloring.RedoColoring();
        }
    }
}
