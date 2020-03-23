using System.Collections.Generic;
using UnityEngine;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    //скрипт используется для выбора палитры цветов и выбора цвета 
    public class BrushAnimation : MonoBehaviour
    {
        //начальный размер используется для анимации
        private Vector2 _startScale;
        //список дочерних объектов
        private List<GameObject> _gameObjects;
        //скрипт отвечающий за основные действия в игре
        public Coloring coloring;
        private void Awake()
        {
            _startScale = transform.localScale;
            _gameObjects = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++)
                _gameObjects.Add(transform.GetChild(i).gameObject);
        }
        
        //увеличение и уменьшение палитры при наведении и выборе
        private void OnMouseEnter()
        {
            if(this != coloring.currentSelectedBrush)
                coloring.tasks.Enqueue(new ToScale(transform, AnimationCurve.EaseInOut(0,0,.025f,1), _startScale * 1.1f));
        }

        private void OnMouseExit()
        {
            if(this != coloring.currentSelectedBrush)
                coloring.tasks.Enqueue(new ToScale(transform, AnimationCurve.EaseInOut(0,0,.025f,1), _startScale));
        }

        //при выборе возвращает в стандартный размер предыдущий выбранный элемент если это не он сам и показыват палитру выбранного цвета
        private void OnMouseDown()
        {
            if (coloring.currentSelectedBrush != null)
            {
                if(coloring.currentSelectedBrush == this) return;
                coloring.tasks.Enqueue(new ToScale(coloring.currentSelectedBrush.transform, AnimationCurve.EaseInOut(0,0,.025f,1), _startScale));
            }

            coloring.currentSelectedBrush = this;
            coloring.ShowPalettes(_gameObjects);
        }
    }
}
