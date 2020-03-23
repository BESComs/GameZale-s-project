using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Object = UnityEngine.Object;

#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    [Serializable]
    public class Paint : IGame
    {
        [TitleGroup("Paint game properties")]
        //Нпозиция откуда начинают расставляться кисти
        public Vector2 startPosition;
        //отступы между кистями
        public Vector2 margin;
        //размер кистей
        public Vector2 size;
        //colorsInLine - цвета в одноый строке 
        [HideReferenceObjectPicker] public List<ColorsInLine> colors = new List<ColorsInLine>();
        //элементы раскраски
        [HideInInspector] public List<SetColor> setColors;
        //цвет который выбран изначально
        public Color defaultColor = Color.white;
        //список кистей
        [HideInInspector] public List<PickColor> pickColors;
        [HideReferenceObjectPicker] public GetColor lastic = new GetColor();
        //префаб раскраски содержит дочернии элементы с спрайтами которые нужно покрасить
        public GameObject paintObjectsPrefab;
        private GameObject _cachePref;
        //конечное изображение
        public Sprite finalImage;
        //первый клик
        [HideInInspector] public bool firstClick;
        //id текущей выбранной кисти
        [HideInInspector] public int currentColorId;
        //текущий выбранный цвет
        [HideInInspector] public Color? currentColor;
        //мышка на кисти
        [HideInInspector] public bool mouseOnBrush;
        //игровой конструктор
        private GameConstructor _gameConstructor;
        //Родительский обьект
        private GameObject _parentObject;
        //обьекты которые будут анимированы
        private List<GameObject> _animatedObjects;
        //канвас текущей сцены
        private GameObject _canvas;
        //игра закончена
        [HideInInspector] public bool gameFinished;
        //обьект - кисть вместо курсора
        [HideInInspector]public GameObject cursorGo;

        //стартовый метод
        public IGame Init(GameConstructor constructor, GameObject parent)
        {
            //инициализация выше описанных переменных
            _gameConstructor = constructor;
            _parentObject = parent;
            _animatedObjects = new List<GameObject>();
            mouseOnBrush = false;
            pickColors = new List<PickColor>();
            gameFinished = false;
            _gameConstructor.GetHelpButtonParent.SetActive(true);
            //cоздание начальной сцены в игре
            CreateStartScene();
            //настройки канваса для того чтобы кисть менялась на курсор когда она находится на рамке
            _canvas = GameObject.Find("Canvas");
            _canvas.AddComponent<ChangeCursorOnCanvasBorder>();
            _canvas.GetComponent<ChangeCursorOnCanvasBorder>().img = GameObject.Find("Border").transform;
            _canvas.GetComponent<ChangeCursorOnCanvasBorder>().ms = this;
            _canvas.GetComponent<ChangeCursorOnCanvasBorder>().StartFunc();
            _gameConstructor.SetOnUpdate(FixedUpdate);
            return this;
        }

        private void CreateStartScene()
        {
            setColors = new List<SetColor>();
            //создание разукращиваемого изображения
            _cachePref = Object.Instantiate(paintObjectsPrefab, new Vector3(-4,0,0), new Quaternion());
            
            _cachePref.transform.SetParent(_parentObject.transform);
            
            _cachePref.SetActive(false);
            _animatedObjects.Add(_cachePref);

            _gameConstructor.GetNextButton.interactable = true;
            
            currentColor = null;
            //оступы для позиции кистей
            var marginX = new List<float>();
            var marginY = new List<float>();
            //создание кистей и ластика
            lastic.color = defaultColor;
            lastic.colorId = -1;
            colors[colors.Count - 1].colors.Add(lastic);
            foreach (var getColor in colors)
            {
                //максимальная высота и суммарная длина
                var maxY = 0f;
                var sumX = 0f;
                foreach (var color in getColor.colors) 
                {
                    //создание кисти
                    var tmpGo = Object.Instantiate(color.brush, _parentObject.transform, true);
                    //спрайт кисти и кончика кисти
                    var sr1 = tmpGo.GetComponent<SpriteRenderer>();
                    var sr2 = tmpGo.GetChild(0).GetComponent<SpriteRenderer>();

                    GameObject gameObject;
                    (gameObject = tmpGo.gameObject).SetActive(false);
                    _animatedObjects.Add(gameObject);

                    
                    var position = tmpGo.position;
                    //Дистанция от кончика кисти до её центра
                    var dist = position - tmpGo.GetChild(0).position;
                    sr2.color = color.color;
                    sr1.drawMode = SpriteDrawMode.Sliced;
                    sr2.drawMode = SpriteDrawMode.Sliced;
                    //изменение размера кисти и кончика кисти с учетом соотношения сторон
                    var size2 = sr1.size;
                    var ratio = (color.advancedOptions
                        ? Mathf.Min(color.size.x / size2.x, color.size.y / size2.y)
                        : Mathf.Min(size.x / size2.x, size.y / size2.y));
                    sr1.size *= ratio;
                    sr2.size *= ratio;
                    dist *= ratio;
                    //изменить позицию кончика кисти при изменении размера кисти
                    tmpGo.GetChild(0).position = position - dist;
                    color.colorTransform = tmpGo;
                    var size1 = sr1.size;
                    color.size = size1;
                    //подсчет отступов
                    maxY = color.advancedOptions ? maxY : Mathf.Max(maxY, size1.y);
                    sumX += color.advancedOptions ? 0 : size1.x + margin.x;
                    //добовление необходимых компонентов и назначение полей
                    var pc = tmpGo.gameObject.AddComponent<PickColor>();
                    pc.ms = this;
                    pc.color = color.color;
                    pc.id = color.colorId;
                    var pc2D = tmpGo.gameObject.AddComponent<PolygonCollider2D>();
                    pc2D.autoTiling = true;
                    
                    pickColors.Add(pc);
               
                }
                marginX.Add(sumX - margin.x);
                marginY.Add(maxY + margin.y);
            }

            if (marginY.Count != 0)
                marginY[marginY.Count - 1] -= margin.y;
            
            //изменение позиций кистей в зависимости от посчитанных отступов
            var lastYPos = 0f;
            var counter = 0;
            foreach (var colorsInLine in colors)
            {
                var lastXPos = 0f;
                foreach (var getColor in colorsInLine.colors)
                {
                    
                    getColor.colorTransform.localPosition = getColor.advancedOptions
                        ? getColor.position
                        : new Vector2(lastXPos
                                      + getColor.size.x / 2f, lastYPos - getColor.size.y / 2f);
                    if (getColor.advancedOptions) continue;
                    var localPosition = getColor.colorTransform.localPosition;
                    lastXPos = localPosition.x + getColor.size.x / 2f + margin.x;
                    localPosition += (Vector3)startPosition - new Vector3(marginX[counter] / 2f, 0,0);
                    getColor.colorTransform.localPosition = localPosition;
                    getColor.colorTransform.GetComponent<PickColor>().StartFunc(_parentObject);
                }
            
                lastYPos -= marginY[counter];
                counter++;
            }
            
            for (var i = 0; i < _cachePref.transform.childCount; ++i)
            {
                if(_cachePref.transform.GetChild(i).GetComponent<SetColor>() == null) continue;
                setColors.Add(_cachePref.transform.GetChild(i).GetComponent<SetColor>());
            }
            
            var bs = _gameConstructor.GetHelpButton.gameObject.AddComponent<ButtonScript>();
            bs.helpButton = _gameConstructor.GetHelpButton;
            bs.helpImage = finalImage;
            bs.StartFunc();
        }

        
        //движение кисти за курсором 
        private void FixedUpdate()
        {
            if (cursorGo == null) return;
            if (Camera.main != null)
                cursorGo.transform.position =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }

        #region IGame
        //проверка ответа если все элементы раскраски имеют правильный id цвета
        public bool CheckAnswer()
        {
            foreach (var setColor in setColors)
                if (setColor.currentColorId != setColor.correctColorId)
                    return false;
        
            return true;
        }

        //появление на сцену
        public async Task SceneIn()
        {
            foreach (var animatedObject in _animatedObjects)
            {
                animatedObject.SetActive(true);
                AnimationUtility.ScaleIn(animatedObject);
            }

            await new WaitForSeconds(1f);
        }
        
        //исчезновение из сцены
        public async Task SceneOut()
        {
            foreach (var animatedObject in _animatedObjects)
            {
                AnimationUtility.ScaleOut(animatedObject);
            }
            
            await new WaitForSeconds(1f);
            _gameConstructor.GetHelpButtonParent.SetActive(false);
        }

        //изменить cостояние после ответа игрока
        public void ChangeStateAfterAnswer()
        {
            firstClick = false;
            colors[colors.Count - 1].colors.RemoveAt(colors[colors.Count - 1].colors.Count - 1);
            Object.Destroy(_gameConstructor.GetHelpButton.GetComponent<ButtonScript>());
            Object.Destroy(_canvas.GetComponent<ChangeCursorOnCanvasBorder>());
            Cursor.visible = true;
        }
        #endregion
    }

    [Serializable]
    public class ColorsInLine
    {
        [HideReferenceObjectPicker, LabelText("Colors Line")]
        public List<GetColor> colors = new List<GetColor>();
    }

    [Serializable]
    public class GetColor
    {
        [HideInInspector] public Transform colorTransform;
        [FoldoutGroup("Color")]
        public int colorId;
        [FoldoutGroup("Color")]
        public Color color;
        [FoldoutGroup("Color")]
        public Transform brush;
        [FoldoutGroup("Color")]
        public bool advancedOptions;
        [FoldoutGroup("Color"), ShowIf(nameof(advancedOptions))]
        public Vector2 position;
        [FoldoutGroup("Color"), ShowIf(nameof(advancedOptions))]
        public Vector2 size;
    }
}