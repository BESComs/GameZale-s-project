using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;
using Object = UnityEngine.Object;
#pragma warning disable 4014
namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Drag_and_Drop
{
    [Serializable]
    public class DragAndDrop : IGame
    {
        public static DragAndDrop Instance { get; private set; }

        [TitleGroup("Drag And Drop game properties", GroupID = "DragAndDrop")]
        [OdinSerialize]
        //стартовая позиция для расстановки обьектов в который ложатся перетаскиваемые обьекты 
        private Vector2 FramesStartPos;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //стартовая позиция для расстановки перетаскиваемых обьектов 
        private Vector2 DraggableElemStartPos;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //cтандартный размер перетаскиваемых обьектов
        private Vector2 DefaultDraggableElemSize;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //cтандартный размер обьектов в который ложатся перетаскиваемые обьекты 
        private Vector2 DefaultFrameSize;
        
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //Будет ли размер текста дефолтный
        private Vector2? defaultTextSize;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //Будет ли размер букв дефолтный
        private float? defaultCharSize;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //текстовый префаб
        private GameObject textMeshProPrefab;
        
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //отступы между обьектами в которые ложатся перетаскиваемые обьекты 
        private Vector2 MarginFrame;
        [OdinSerialize, TitleGroup("DragAndDrop")]
        //отступы между перетаскиваемые обьекты 
        private Vector2 MarginDragElement;
        
        [OdinSerialize, HorizontalGroup("DragAndDrop/bools"), OnValueChanged(nameof(SetPosFrame))]
        //задать позиции обьектам в которые ложатся перетаскиваемые обьекты вручную
        private bool CustomPosFrame;
        [OdinSerialize, HorizontalGroup("DragAndDrop/bools"), OnValueChanged(nameof(SetPosDe))]
        //задать позиции перетаскиваемым обьектам вручную
        private bool CustomPosDE;


        [OdinSerialize, HideReferenceObjectPicker, BoxGroup(GroupID = "DragAndDrop/FrameInLines")]
        //список обьектов в которые ложатся перетаскиваемые обьекты которые лежат на одной линии 
        public List<List<Frame>> FramesInLines = new List<List<Frame>>();

        [Button(ButtonSizes.Medium), BoxGroup(GroupID = "DragAndDrop/FrameInLines")]
        //генерация ID
        public void GenerateIDs() => SetFrameIDs();
        
        [OdinSerialize, HideReferenceObjectPicker, BoxGroup(GroupID = "DragAndDrop/DraggableElems")]
        //список перетаскиваемых обьектов
        public List<DraggableElementInLine> draggableElementsInLine = new List<DraggableElementInLine>();
        
        //родительские обьекты
        private GameObject FrameParent, DraggableElemsParent;
        //находится ли скрипт в ассинхронной операции
        //находится ли скрипт в ассинхронной операции
        [NonSerialized] public bool TaskCompleted;
        //скрипт отвечающий за создания игровых уровней
        private GameConstructor gameConstructor;
        //родительский обьект
        private GameObject parentObject;
        //Обьекты который будут анимированны
        private List<Renderer> animatedObjects;
        //можно ли двигать обьекты
        [HideInInspector] public bool canDrag;
        
        //метод вызываемый в самом начале
        public IGame Init(GameConstructor constructor, GameObject parentObject)
        {
            Instance = this;
            TaskCompleted = true;
            //позиция последнего элемента по Y
            var lastYPos = -MarginFrame.y;
            
            canDrag = true;
            //инициализация выше описаных переменных
            gameConstructor = constructor;
            this.parentObject = parentObject;
            animatedObjects = new List<Renderer>();
            
            FrameParent = new GameObject("Frames Parent");
            DraggableElemsParent = new GameObject("Draggable Elements Parent");
            FrameParent.transform.SetParent(this.parentObject.transform);
            DraggableElemsParent.transform.SetParent(this.parentObject.transform);
            //создание обьектов в который ложатся перетаскиваемые обьекты 
            foreach (var framesInLine in FramesInLines)
            {
                //Все элементы в одной строке
                var tmpListGo = new List<GameObject>();
                //Начальный отступ в строке
                var startMargin = Vector2.zero;
                //суммарный отступ в строке
                var sumXMargin = .0f;
                //создание обьектов в одной строке
                foreach (var frame in framesInLine)
                {
                    //создание обьекта
                    var tmpGo = new GameObject();
                    //настрой отрисовки спрайта
                    tmpGo.AddComponent<SpriteRenderer>().sprite = frame.FrameSprite;
                    var goSr = tmpGo.GetComponent<SpriteRenderer>();
                    goSr.sortingLayerID = SortingLayer.NameToID("Interactive Shapes");
                    goSr.sortingOrder = 0;
                    goSr.drawMode = SpriteDrawMode.Sliced;
                    //множитель для изменения размера спрайта указанового пользователем с учетом соотношения сторон
                    var ratio = (frame.ResizeFrame)
                        ? Mathf.Min(frame.FrameSize.x / goSr.size.x, frame.FrameSize.y / goSr.size.y)
                        : Mathf.Min(DefaultFrameSize.x / goSr.size.x, DefaultFrameSize.y / goSr.size.y);
                    goSr.size *= frame.UseFrameSpriteSize ? 1f : ratio;
                    frame.FrameSize = goSr.size;
                    if(!CustomPosFrame)
                        frame.FramePos = tmpGo.transform.localPosition;
                    //подсчет начального отступа
                    startMargin.x += goSr.size.x + MarginFrame.x;
                    startMargin.y = Mathf.Max(goSr.size.y, startMargin.y) + MarginFrame.y;
                    tmpListGo.Add(tmpGo);

                    tmpGo.SetActive(false);
                    animatedObjects.Add(goSr);
                }

                lastYPos += startMargin.y;
                //установка позиций созданных обьектов в текущей итерации
                for(var i = 0; i < tmpListGo.Count; ++i)
                {
                    //если позиция обьекта была задана пользователем
                    if (CustomPosFrame)
                    {
                        tmpListGo[i].transform.localPosition = framesInLine[i].FramePos;
                        continue;
                    }
                    //раземр по x + отступ по x
                    var tmp = tmpListGo[i].GetComponent<SpriteRenderer>().size.x + MarginFrame.x;
                    //сложение к суммарному отступу
                    sumXMargin += tmp;
                    //позиция обьекта по x = стартовая позиция - половина от длины всех элементов в строке с отступами + текущий отступ(sumXMargin - tmp / 2f)
                    //позиция по y = стартовая позиция + текущая позиция элементов по Y + половина от высоты максимального элемента в строке с учетом отступа
                    tmpListGo[i].transform.localPosition = (Vector3) FramesStartPos +
                                                      new Vector3(-startMargin.x / 2f + sumXMargin - tmp / 2f,
                                                          -lastYPos + startMargin.y / 2f, 0);
                    framesInLine[i].FramePos = tmpListGo[i].transform.localPosition;
                    tmpListGo[i].transform.SetParent(FrameParent.transform);
                }
            }

            lastYPos = -MarginDragElement.y;
            //создание перетаскиваемых обьектов 
            foreach (var draggableElementInLine in draggableElementsInLine)
            {
                var tmpListGo = new List<GameObject>();
                var startMargin = Vector2.zero;
                var sumXMargin = .0f;
                
                foreach (var draggableElement in draggableElementInLine.draggableElements)
                {
                    GameObject tmpGo;

                    switch (draggableElement.spriteOrText)
                    {
                        case SpriteOrText.Sprite:
                            //создание спрайта
                            tmpGo = new GameObject();
                            //настрой отрисовки спрайта
                            var goSr = tmpGo.AddComponent<SpriteRenderer>();
                            goSr.sortingLayerName = "Interactive Shapes";
                            goSr.sortingOrder = 1;
                            goSr.drawMode = SpriteDrawMode.Sliced;
                            goSr.sprite = draggableElement.SpriteElement;
                            //множитель для изменения размера спрайта указанового пользователем с учетом соотношения сторон
                            var ratio = (draggableElement.Resize)
                                ? Mathf.Min(draggableElement.DeStartSize.x / goSr.size.x, draggableElement.DeStartSize.y / goSr.size.y)
                                : Mathf.Min(DefaultDraggableElemSize.x / goSr.size.x, DefaultDraggableElemSize.y / goSr.size.y);
                            goSr.size *= draggableElement.UseSpriteSize ? 1f : ratio;
                            draggableElement.DeStartSize = goSr.size;
                            //подсчет отступов
                            startMargin.x += goSr.size.x + MarginDragElement.x;
                            startMargin.y = Mathf.Max(goSr.size.y, startMargin.y);
                            tmpGo.AddComponent<BoxCollider>();
                            animatedObjects.Add(goSr);
                            break;
                        case SpriteOrText.Text:
                            //создание текста
                            tmpGo = Object.Instantiate(textMeshProPrefab);
                            //настройки текста
                            var goTmp = tmpGo.GetComponent<TextMeshPro>();
                            var delta = goTmp.rectTransform.sizeDelta;
                            delta = draggableElement.textSize ?? (defaultTextSize ?? delta);
                            goTmp.text = draggableElement.text;
                            goTmp.rectTransform.sizeDelta = delta;
                            goTmp.alignment = TextAlignmentOptions.Midline;
                            goTmp.enableAutoSizing = true;
                            //изменение размера
                            goTmp.fontSizeMax = draggableElement.textCharSize ?? (defaultCharSize ?? goTmp.fontSize);
                            goTmp.fontSizeMin = 0;
                            var sizeDelta = delta;
                            draggableElement.DeStartSize = sizeDelta;
                            //подсчет отступов
                            startMargin.x += sizeDelta.x + MarginDragElement.x;
                            startMargin.y = Mathf.Max(sizeDelta.y, startMargin.y);
                            var collider = tmpGo.AddComponent<BoxCollider>();
                            collider.size = draggableElement.DeStartSize;
                            animatedObjects.Add(goTmp.renderer);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    draggableElement.DeTransform = tmpGo.transform;
                    tmpListGo.Add(tmpGo);
                    tmpGo.AddComponent<GameElementController>().CurrentDe = draggableElement;
                    tmpGo.SetActive(false);
                }

                lastYPos += startMargin.y + MarginDragElement.y;
                //Подсчет позиций для перетаскивыемых обьектов
                for (var i = 0; i < tmpListGo.Count; i++)
                {
                    if (CustomPosDE)
                    {
                        tmpListGo[i].transform.localPosition = draggableElementInLine.draggableElements[i].StartPos;
                        continue;
                    }
                    var tmp = MarginDragElement.x;
                    if (tmpListGo[i].GetComponent<SpriteRenderer>() != null)
                        tmp += tmpListGo[i].GetComponent<SpriteRenderer>().size.x;
                    else
                        tmp += tmpListGo[i].GetComponent<RectTransform>().sizeDelta.x ;
                    
                    sumXMargin += tmp;
                    //подсчет позиции такой же принцип как и для элементов frame в одной строке
                    tmpListGo[i].transform.localPosition = (Vector3) DraggableElemStartPos +
                                                      new Vector3(-startMargin.x / 2f + sumXMargin - tmp / 2f,
                                                          -lastYPos + startMargin.y / 2f, 0);
                    draggableElementInLine.draggableElements[i].StartPos = tmpListGo[i].transform.localPosition;
                    tmpListGo[i].transform.SetParent(DraggableElemsParent.transform);
                }
            }

            return this;
        }
        //todo обьект в который ложатся перетаскиваемые обьекты - frame 
        
        //возвращает индекс где лежит frame - de и удаляет его из списка
        private (int,int) RemoveDe(DraggableElement de)
        {
            for (var i = 0; i < FramesInLines.Count; ++i)
            {
                for (var j = 0; j < FramesInLines[i].Count; ++j)
                {
                    if (!FramesInLines[i][j].DesInFrame.Contains(de)) continue;
                    FramesInLines[i][j].DesInFrame.Remove(de);
                    return (i,j);
                }
            }
            return (-1,-1);
        }

        //Метод который добавляет в frame перетаскиваемый обьект - если drop == true иначе удаляет из frame - a в которм он находился 
        public async Task PlacementOfObjects(DraggableElement de, bool drop)
        {
            TaskCompleted = false;
            //id frame - a в которм находился de 
            var idForSwap = de.CurrentFrameId;
            // индексы frame - a в который был отпущен de
            var lineInFrame = -1;
            var columnInFrame = -1;
            //если обьект ложится в frame
            if (drop)
            {
                //поиск frame - а над которым находится de
                for (var i = 0; i < FramesInLines.Count; ++i)
                {
                    var breakFlag = false;
                    for (var j = 0; j < FramesInLines[i].Count; ++j)
                    {
                        //проверка находится ли de над текущем frame - e
                        if (!(Mathf.Abs(de.DeTransform.position.x - FramesInLines[i][j].FramePos.x) <
                              FramesInLines[i][j].FrameSize.x / 2f) ||
                            !(Mathf.Abs(de.DeTransform.position.y - FramesInLines[i][j].FramePos.y) <
                              FramesInLines[i][j].FrameSize.y / 2f)) continue;
                        FramesInLines[i][j].DesInFrame.Add(de);
                        de.CurrentFrameId = FramesInLines[i][j].CurrentFrameId;
                        lineInFrame = i;
                        columnInFrame = j;
                        breakFlag = true;
                        break;
                    }

                    if (breakFlag)
                        break;
                }

                if (lineInFrame == -1)
                {
                    //если frame не найден de двигается на начальную позицию и проигрывается анимация Bounce
                    ISequentTask task = new Move(de.DeTransform,AnimationCurve.EaseInOut(0,0,.2f,1), de.StartPos);
                    await task.RunTask();
                    task = new Bounce(de.DeTransform,AnimationCurve.EaseInOut(0,0,.2f,1), de.DeStartSize);
                    await task.RunTask();
                    //показывает что de находится не в frame 
                    de.CurrentFrameId = -1;
                    TaskCompleted = true;
                    //проверка на включение кнопки перехода на следующий уровень
                    CheckNSetNextButtonInteractable();
                    return;
                }
            }
            //если обьект берется из frame
            else
            {
                var (item1, item2) = RemoveDe(de);
                lineInFrame = item1;
                columnInFrame = item2;
            }
            //frame в который ложится de
            var frame = FramesInLines[lineInFrame][columnInFrame];
            //список перетаскиваемых обьектов который находятся в frame
            var tmp = frame.DesInFrame;
            // размер и позиция frame - a
            var frameSize = frame.FrameSize * .9f;
            var framePos = frame.FramePos;
            //если количество елементов в frame максимальное которое может вместить frame
            if (frame.MaxElementsInFrame < tmp.Count)
            {
                //поиск позиции frame в которой находился de 
                var tmpPos = tmp[0].StartPos;
                foreach (var t in FramesInLines)
                    foreach (var tp in t)
                        if (tp.CurrentFrameId == idForSwap)
                            tmpPos = tp.FramePos;
               
                //берет первый перетаскиваемый элемент в frame и ложит в frame в которм находился de если de
                //не находился в frame возвращает первый перетаскиваемый элемент в frame На стартовую позицию
                var tmp0 = tmp[0];
                tmp[0].CurrentFrameId = idForSwap;
                ISequentTask task = new Move(tmp[0].DeTransform,AnimationCurve.EaseInOut(0,0,.2f,1), tmpPos);
                await task.RunTask();
                tmp.Remove(tmp[0]);
            
                await PlacementOfObjects(tmp0, true);
            }
            
            if (tmp.Count == 0)
            {
                TaskCompleted = true;
                CheckNSetNextButtonInteractable();
                return;
            }

            //эфект анимации
            foreach (var draggableElement in tmp)
            {
                var sz = frame.SizeOfElementsInFrame;

                sz = frame.DontResizeDE
                    ? draggableElement.DeStartSize
                    : draggableElement.DeStartSize * Mathf.Min(sz.x / draggableElement.DeStartSize.x,sz.y / draggableElement.DeStartSize.y);
                
                ISequentTask task = new Bounce(draggableElement.DeTransform, AnimationCurve.EaseInOut(0,0,.2f,1), sz);
                task.RunTask();
            }
            //если элементы в frame не меняют размер и количество элементов в frame нулевое
            if (frame.DontResizeDE && tmp.Count != 0)
            {
                //посдчет позиций для элементов внутри frame
                var marginX = new List<float>();
                var marginY = 0f;
                var numOfElemInLine = new List<int> {0};
                var currentSum = 0f;
                foreach (var draggableElement in tmp)
                {
                    numOfElemInLine[numOfElemInLine.Count - 1]++;
                    currentSum += draggableElement.DeStartSize.x;
                    if (!(currentSum > frameSize.x)) continue;
                    currentSum = 0;
                    numOfElemInLine.Add(0);
                }

                for (var index = 0; index < numOfElemInLine.Count;)
                {
                    if (numOfElemInLine[index] == 0)
                        numOfElemInLine.Remove(numOfElemInLine[index]);
                    else
                        index++;
                }

                var counter = 0;
                foreach (var i in numOfElemInLine)
                {
                    marginX.Add(0);
                    var maxY = 0f;
                    for (var j = 0; j < i; j++)
                    {
                        marginX[marginX.Count - 1] += tmp[counter].DeStartSize.x;
                        maxY = Mathf.Max(tmp[counter].DeStartSize.y, maxY);
                        counter++;
                    }
                    marginY += maxY;
                }
                counter = 0;
                var sumY = tmp[0].DeStartSize.y / 2f;
                //анимированный сдвиг всех элементов на подсчитанные позиции
                for (var k = 0; k < numOfElemInLine.Count; k++)
                {
                    var i = numOfElemInLine[k];
                    var sumX = tmp[counter].DeStartSize.x / 2f;
                    for (var j = 0; j < i; j++)
                    {
                        ISequentTask task = new Move(tmp[counter].DeTransform, AnimationCurve.EaseInOut(0, 0, .2f, 1),
                            new Vector2(-marginX[k] / 2f + sumX, marginY / 2f - sumY) + framePos);
                        task.RunTask();
                        counter++;
                        if (counter < tmp.Count)
                            sumX += tmp[counter].DeStartSize.x;
                    }
                    sumY += tmp[k].DeStartSize.y;
                }
            }
            else //если элементы меняют размер при добовлении в frame
            {
                //подсчет позиций
                var elemInLine = Mathf.Min((int)(frameSize.x / (frame.SizeOfElementsInFrame.x * 1.05f)), tmp.Count);
                elemInLine = elemInLine <= 0 ? 1 : elemInLine;
                var elemInColumn = (tmp.Count % elemInLine == 0 || elemInLine > tmp.Count) ? tmp.Count / elemInLine : tmp.Count / elemInLine + 1;
                var margin = new Vector2(elemInLine * frame.SizeOfElementsInFrame.x * 1.05f, -elemInColumn * frame.SizeOfElementsInFrame.y * 1.05f);
                var counterX = 0;
                var counterY = frame.SizeOfElementsInFrame.y * 1.05f;
                //анимированный сдвиг всех элементов на подсчитанные позиции
                foreach (var tp in tmp)
                {
                    counterX++;
                    ISequentTask task = new Move(tp.DeTransform, AnimationCurve.EaseInOut(0, 0, .2f, 1),
                        framePos - margin / 2f + new Vector2((counterX - .5f) * (frame.SizeOfElementsInFrame.x * 1.05f),
                            frame.SizeOfElementsInFrame.y * 1.05f / 2f - counterY));
                    task.RunTask();
                    if (counterX != elemInLine) continue;
                    counterY += frame.SizeOfElementsInFrame.y * 1.05f;
                    counterX = 0;
                }
            }
            //ожидание анимаций
            await new WaitForSeconds(.2f);
            TaskCompleted = true;
            
            CheckNSetNextButtonInteractable();
        }

        //проверка сделать ли кнопку перехода к следующему уровню активной
        public void CheckNSetNextButtonInteractable()
        {
            gameConstructor.GetNextButton.interactable = false;
            if(DraggableElemsParent == null || DraggableElemsParent.transform == null) return;
            if (DraggableElemsParent.transform.childCount < FrameParent.transform.childCount)
            {
                foreach (Transform dragElement in DraggableElemsParent.transform)
                {
                    if (dragElement.GetComponent<GameElementController>().CurrentDe.CurrentFrameId == -1)
                        return;
                }
            }
            else
            {
                foreach (List<Frame> frames in FramesInLines)
                {
                    foreach (Frame frame in frames)
                    {
                        if (frame.MaxElementsInFrame != frame.DesInFrame.Count)
                        {
                            gameConstructor.GetNextButton.interactable = false;
                            return;
                        }
                    }
                }
            }
                
            gameConstructor.GetNextButton.interactable = true;
        }
        
        #region Odin Releated

        //установка всем элементам кастомную позицию
        private void SetPosFrame()
        {
            foreach (List<Frame> framesInLine in FramesInLines)
            {
                foreach (Frame frame in framesInLine)
                {
                    frame.customFramePos = CustomPosFrame;
                }
            }
        }

        private void SetPosDe()
        {
            foreach (DraggableElementInLine framesInLine in draggableElementsInLine)
            {
                foreach (DraggableElement draggableElement in framesInLine.draggableElements)
                {
                    draggableElement.customStartPos = CustomPosDE;
                }
            }
        }

        //установить всем frame id 0 до количество frame - 1
        private void SetFrameIDs()
        {
            for (int i = 0, id = 0; i < FramesInLines.Count; i++)
            {
                foreach (Frame frame in FramesInLines[i])
                {
                    frame.CurrentFrameId = id++;
                }
            }
        }
        
        #endregion

        #region IGame
    
        //проверка ответа совпадают ли у всех de текущий id frame и правильный id frame 
        public bool CheckAnswer()
        {
            foreach (var draggableElementInLine in draggableElementsInLine)
                foreach (var draggableElement in draggableElementInLine.draggableElements)    
                    if (draggableElement.CurrentFrameId != draggableElement.CorrectFrameId)
                        return false;
        
            return true;
        }

        //появление на сцену
        public async Task SceneIn()
        {
            foreach (Renderer animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(true);
                AnimationUtility.FadeIn(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        //исчезновение из сцены
        public async Task SceneOut()
        {
            foreach (Renderer animatedObject in animatedObjects)
            {
                AnimationUtility.FadeOut(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        //сделать перетаскиваемые обьекты не интерактивными после прохода уровня
        public void ChangeStateAfterAnswer() => canDrag = false;

        #endregion
    }

    public enum SpriteOrText
    {
        Sprite, Text
    }
    
    [Serializable]
    public class Frame
    {
        [FormerlySerializedAs("Resize"), HorizontalGroup("bools")] public bool ResizeFrame;
        [HorizontalGroup("bools")]public bool DontResizeDE;
        [HorizontalGroup("bools2"), ShowIf(nameof(ResizeFrame))] public bool UseFrameSpriteSize;
        
        [HideIf(nameof(DontResizeDE))]public Vector2 SizeOfElementsInFrame;
        [HideInInspector] public List<DraggableElement> DesInFrame = new List<DraggableElement>();
        
        //public List<int> CorrectFramed;
        
        [ShowIf(nameof(ResizeFrame)), HideIf(nameof(UseFrameSpriteSize))]
        public Vector2 FrameSize;
        
        [ShowIf(nameof(customFramePos))]
        public Vector2 FramePos;
        [PreviewField(50f)]
        public Sprite FrameSprite;
        public int MaxElementsInFrame;
        
        public int CurrentFrameId;

        #region Odin Related
        
        [NonSerialized] public bool customFramePos;

        #endregion
    }

    [Serializable]
    public class DraggableElementInLine
    {
        [HideReferenceObjectPicker]
        public List<DraggableElement> draggableElements = new List<DraggableElement>();
    }

    [Serializable]
    public class DraggableElement
    {
        public SpriteOrText spriteOrText;
        [HorizontalGroup("bools")]
        public bool Resize;
        [HorizontalGroup("bools")]
        public bool UseSpriteSize;
        
        [HideInInspector] public Transform DeTransform;
        [ShowIf(nameof(Resize)), HideIf(nameof(UseSpriteSize))] public Vector2 DeStartSize;
        [ShowIf(nameof(customStartPos))] public Vector2 StartPos;
        [ShowIf(nameof(spriteOrText), SpriteOrText.Sprite), PreviewField(50f)]
        public Sprite SpriteElement;

        [ShowIf(nameof(spriteOrText), SpriteOrText.Text)]
        public string text;
        
        [HideInInspector] public int CurrentFrameId;
        
     
        [ShowIf(nameof(spriteOrText), SpriteOrText.Text)]
        public Vector2? textPosition;
        [ShowIf(nameof(spriteOrText), SpriteOrText.Text)]
        public Vector2? textSize;
        [ShowIf(nameof(spriteOrText), SpriteOrText.Text)]
        public float? textCharSize;
        
        public int CorrectFrameId;
        
        #region Odin Related
        
        [NonSerialized] public bool customStartPos;

        #endregion
    }
}