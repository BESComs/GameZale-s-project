using System.Collections.Generic;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    //cкрипт для кистей
    
    public class PickColor : MonoBehaviour
    {
        //кисть увеличена - выбрана
        public bool scaled;
        //main script
        public Paint ms;
        //текущий цвет кисти
        public Color color;
        //id кисти
        public int id;
        //обьект прекрепляющийся на курсор при взятии кисти
        private GameObject _cursorGo;
        //анимируется
        private bool _inScaling;
        //очередь анимаций
        private readonly Queue<ISequentTask> _tasks = new Queue<ISequentTask>();
        //увеличеный размер стандартный размер
        private Vector2 _scaledSize, _normalSize;
    
        //метод вызывается из ms в начале игры
        public void StartFunc(GameObject parentObject)
        {
            //создание кисти которая при выборе двигается за курсором
            _cursorGo = Instantiate(gameObject).gameObject;
            _cursorGo.transform.SetParent(parentObject.transform);
            _cursorGo.SetActive(false);
            //рукоять кисти на верхний слой
            _cursorGo.GetComponent<SpriteRenderer>().sortingOrder = 9;
            _cursorGo.transform.GetChild(0).gameObject.AddComponent<Rigidbody2D>().isKinematic = true;
            //кончик кисти на более верхний слой
            var sr = _cursorGo.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sr.sortingOrder = 10;
            var pc = _cursorGo.transform.GetChild(0).gameObject.AddComponent<PolygonCollider2D>().points;
            _cursorGo.transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        
            //изменение размера коллайдера кончика кисти
            for (var i = 0; i < pc.Length; ++i)
                pc[i] *= sr.size * .25f;
            //cдвиг центра коллайдера кончика кисти
            _cursorGo.transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().offset -= new Vector2(.2f, 0);
            _cursorGo.transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().points = pc;
            Destroy(_cursorGo.GetComponent<PickColor>());
            Destroy(_cursorGo.GetComponent<PolygonCollider2D>());
            //наклон кисти и изменение размера
            _cursorGo.transform.rotation = Quaternion.Euler(0,0,45);
            _cursorGo.transform.localScale *= .5f;
            _cursorGo.transform.GetChild(0).gameObject.AddComponent<CollisionStay>().ms = ms;
            _normalSize = transform.localScale;
            _scaledSize = _normalSize * 1.3f;
        }

        private void OnMouseEnter()
        {
            //анимирование обьекта если над ним курсор
            if(!scaled)
                _tasks.Enqueue(new ScaleTo(gameObject, _scaledSize,
                    AnimationCurve.EaseInOut(0, 0, .2f, 1)));
            if(!ms.firstClick || scaled) return;
            Cursor.visible = true;
            ms.mouseOnBrush = true;
            if(ms.cursorGo != null)
                ms.cursorGo.SetActive(false);

        }

        private void OnMouseExit()
        {
            //перестать анимировать если курсо вышел за пределы кисти
            if(!scaled)
                _tasks.Enqueue(new ScaleTo(gameObject, _normalSize,
                    AnimationCurve.EaseInOut(0, 0, .2f, 1)));
            if(!ms.firstClick || scaled) return;
            Cursor.visible = false;        
            if(ms.cursorGo != null)
                ms.cursorGo.SetActive(true);
            ms.mouseOnBrush = false;
        }

        private async void Method()
        {
            //проигрывание анимаций
            _inScaling = true;
            if(_tasks.Count != 0)
                await _tasks.Dequeue().RunTask();
            if (_tasks.Count >= 2)
            {
                _tasks.Dequeue().RunTask();
                _tasks.Dequeue().RunTask();
            }
            _inScaling = false;
        }

        private void Update()
        {
            if(!_inScaling)
                Method();
        }

        private void OnMouseOver()
        {
            if (ms.gameFinished || !Input.GetKeyDown(KeyCode.Mouse0) || scaled)
                return;
            //Выбрать кисть при нажатии на нее
            ms.firstClick = true;
            //заменить курсор на кисть
            if(Cursor.visible)
                Cursor.visible = false;
            if(ms.cursorGo != null)
                ms.cursorGo.SetActive(false);
        
            _cursorGo.transform.position = transform.position;
            _cursorGo.SetActive(true);
            //назначить кисть mainScript - у
            ms.cursorGo = _cursorGo;
            ms.currentColor = color;
            ms.currentColorId = id;
            // анимация при выборе
            (new ScaleTo(gameObject, _scaledSize,
                AnimationCurve.EaseInOut(0, 0, .2f, 1))).RunTask();
            //отменить выбор на других кистях
            foreach (var msPickColor in ms.pickColors)
            {
                if(msPickColor.scaled)
                    (new ScaleTo(msPickColor.gameObject, _normalSize,
                        AnimationCurve.EaseInOut(0, 0, .2f, 1))).RunTask();
                msPickColor.scaled = false;
            }
            scaled = true;
        }
    }
}
