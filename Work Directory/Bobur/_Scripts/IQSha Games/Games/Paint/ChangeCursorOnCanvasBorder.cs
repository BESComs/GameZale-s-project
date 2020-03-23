using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    //изменяет кисть на курсор если он находится на рамке
    public class ChangeCursorOnCanvasBorder : MonoBehaviour
    {
        //ms -main script
        public Paint ms;
        //рамка
        public Transform img;
        //UI raycast
        private GraphicRaycaster _mRayCaster;
        //информация о курсоре
        private PointerEventData _mPointerEventData;
        //система событий в игре
        private EventSystem _mEventSystem;

        public void StartFunc()
        {
            //какая степень прозрачности у рамки для райкаста
            img.GetComponent<Image>().alphaHitTestMinimumThreshold = .05f;
            _mRayCaster = GetComponent<GraphicRaycaster>();
            _mEventSystem = GetComponent<EventSystem>();
        }
        private void Update()
        {
            if(!ms.firstClick || ms.mouseOnBrush)
                return;
        
            _mPointerEventData = new PointerEventData(_mEventSystem)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            //Ui райкаст в место где находится курсор
            _mRayCaster.Raycast(_mPointerEventData, results);
            Cursor.visible = results.Count > 0;
            //если results.Count > 0 то райкаст попал по ui элементу в не празрачную часть
            if(ms.cursorGo != null)
                ms.cursorGo.SetActive(!Cursor.visible);
        }
    }
}