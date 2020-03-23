using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Object = UnityEngine.Object;
#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Input_Answer
{
    [Serializable]
    public class InputAnswer : IGame
    {
        [TitleGroup("Input Answer game properties", GroupID = "InputAnswer")]
        //позиция слева справа по центру
        public TextInInputFieldPos TextInInputFieldPosition;
        //цвет текста
        public Color DefaultInputColor = Color.black;
        //размер текста
        public int DefaultInputCharSize = 12;
        //стартовая позиция всех элементов
        public Vector2 StartPosition;
        //отступ между элементами
        public Vector2 Margin = new Vector2(20f, 20f);
        //отступ между спрайтом / текстом и боксом для ввода
        public float MarginBetweenSpriteOrTextAndInput = 10f;
        //стандартный размер текстового бокса
        public Vector2 DefaultInputFieldSize = new Vector2(160f, 30f);
        //стандартный размер спрайта / текста
        public Vector2 DefaultSpriteOrTextSize = new Vector2(100f,100f);
        //канвас на сцене
        public Canvas Canvas;
        //все поля ввода текста
        private List<TMP_InputField> _playerInputs;
        //стандартное поля ввод текса
        public TMP_InputField DefaultInput;
        //стандартный текст
        public TextMeshProUGUI DefaultTextPrefab;
        [HideReferenceObjectPicker]
        //cписок строк игровых элементов
        //GameElements - игровые элементы расположенные в одной строке
        public List<GameElements> GameElementsInLine = new List<GameElements>();
        //Transform всех игровых элементов
        [HideInInspector]public List<Transform> GameElementTransforms;
        //родительский обьект канваса
        [HideInInspector]public Transform ParentOfCanvasElements;
        
        private GameConstructor gameConstructor;
        private List<GameObject> animatedObjects;
        
        //метод срабатывующий в начале игры
        public IGame Init(GameConstructor constructor, GameObject parent)
        {
            gameConstructor = constructor;
            animatedObjects = new List<GameObject>();
            
            CreateStartScene();

            return this;
        }

        //изменяет размер ui обьектов в ratio раз которые находятся ниже по иерархии чем input
        private static void Resizing(RectTransform input, float ratio)
        {
            for (var i = 0; i < input.childCount; ++i)
            {
                input.sizeDelta *= ratio;
                Resizing(input.GetChild(i).GetComponent<RectTransform>(),ratio);
            }
        }

        //создание начальной сцены
        private void CreateStartScene()
        {
            _playerInputs = new List<TMP_InputField>();
            GameElementTransforms = new List<Transform>();
            //отступы от обьектов по горизонтали
            var marginsX = new List<List<float>>();
            //отступы между строками по вертикали
            var marginsY = new List<float>();
            //назначение родительского обьекта канвасу
            ParentOfCanvasElements = (new GameObject()).transform;
            ParentOfCanvasElements.SetParent(Canvas.transform, false);
            ParentOfCanvasElements.localPosition = Vector3.zero;
            ParentOfCanvasElements.localScale = Vector3.one;
            //создание всех ировых элементов
            foreach (var gameElements in GameElementsInLine)
            {
                //отступы по x в одной стоке
                var tmpList = new List<float>();
                //максимальная высота элементов в одной строке с учетом отступа
                var maxY = .0f;
                foreach (var elementWithInput in gameElements.Elements)
                {
                    //поле ввода текста
                    RectTransform tmpGo;
                    if (elementWithInput.AdvancedOption && elementWithInput.InputField != null)
                        tmpGo = (RectTransform)Object.Instantiate(elementWithInput.InputField.transform);
                    else
                        tmpGo = (RectTransform)Object.Instantiate(DefaultInput.transform);
                    tmpGo.SetParent(ParentOfCanvasElements.transform, false);
                    Vector2 sizeDelta2;
                    //размер поля ввода
                    var delta2 = tmpGo.sizeDelta;
                    //множитель для изменения размера
                    var ratio = elementWithInput.AdvancedOption
                        ? Math.Min(elementWithInput.InputSize.x / delta2.x,
                            elementWithInput.InputSize.y / delta2.y)
                        : Mathf.Min(DefaultInputFieldSize.x /
                                    (sizeDelta2 = tmpGo.sizeDelta).x, DefaultInputFieldSize.y / sizeDelta2.y);
                    Resizing(tmpGo, ratio);
                    _playerInputs.Add(tmpGo.GetComponent<TMP_InputField>());
                    //если все поля для ввода текста не пусты вкл кнопку проверки
                    tmpGo.GetComponent<TMP_InputField>().onValueChanged.AddListener(str =>
                    {
                        foreach (var playerInput in _playerInputs)
                        {
                            if (playerInput.text.Trim() != string.Empty) continue;
                            gameConstructor.GetNextButton.interactable = false;
                            return;
                        }
                        gameConstructor.GetNextButton.interactable = true;
                    });
                    //настройки текста
                    var tmp = tmpGo.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                    tmp.color = DefaultInputColor;
                    tmp.fontSize = DefaultInputCharSize;
                
                    switch (TextInInputFieldPosition)
                    {
                        case TextInInputFieldPos.Middle:
                            tmp.alignment = TextAlignmentOptions.Center;
                            break;
                        case TextInInputFieldPos.Right:
                            tmp.alignment = TextAlignmentOptions.Right;
                            break;
                        case TextInInputFieldPos.Left:
                            tmp.alignment = TextAlignmentOptions.Left;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //создание спрайта или текста
                    switch (elementWithInput.SpriteOrText)
                    {
                        case SpriteOrText.Sprite:
                            //создание спрайта
                            var image = new GameObject().AddComponent<RectTransform>();
                            var sprite = image.gameObject.AddComponent<Image>();
                            sprite.sprite = elementWithInput.Sprite;
                            image.transform.SetParent(ParentOfCanvasElements, false);
                            //размер
                            var delta1 = new Vector2(sprite.preferredWidth, sprite.preferredHeight);
                            image.sizeDelta = delta1;
                            Vector2 sizeDelta1;
                            //ресайз спрайта до размера введнего пользователем с учетом соотношения сторон
                            image.sizeDelta *= elementWithInput.AdvancedOption
                                ? Mathf.Min(elementWithInput.SpriteOrTextSize.x / delta1.x,
                                    elementWithInput.SpriteOrTextSize.y / delta1.y)
                                : Mathf.Min(DefaultSpriteOrTextSize.x / (sizeDelta1 = image.sizeDelta).x,
                                    DefaultSpriteOrTextSize.y / sizeDelta1.y);
                            GameObject o;
                            (o = image.gameObject).SetActive(false);
                            //Добавить обьект в список анимированных обьектов
                            animatedObjects.Add(o);
                            break;
                        case SpriteOrText.Text:
                            //создание текста
                            var text = (RectTransform) Object.Instantiate(DefaultTextPrefab.transform,
                                ParentOfCanvasElements, false);
                            text.GetComponent<TextMeshProUGUI>().text = elementWithInput.text;
                            Vector2 sizeDelta;
                            var delta = text.sizeDelta;
                            //ресайз текста до размера введнего пользователем с учетом соотношения сторон
                            text.sizeDelta *= elementWithInput.AdvancedOption
                                ? Mathf.Min(elementWithInput.SpriteOrTextSize.x / delta.x,
                                    elementWithInput.SpriteOrTextSize.y / delta.y)
                                : Mathf.Min(DefaultSpriteOrTextSize.x / (sizeDelta = text.sizeDelta).x,
                                    DefaultSpriteOrTextSize.y / sizeDelta.y);
                            GameObject gameObject;
                            (gameObject = text.gameObject).SetActive(false);
                            animatedObjects.Add(gameObject);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //подсчет отступа по Y в зависимости от того расположен спрайт\текст с верху(снизу) или справа(слева)
                    switch (elementWithInput.Direction)
                    {
                        case Direction.Down:
                        case Direction.Up:
                            tmpList.Add((elementWithInput.AdvancedOption
                                            ? 0
                                            : Mathf.Max(DefaultInputFieldSize.x,
                                                  DefaultSpriteOrTextSize.x) + Margin.x) +
                                        (tmpList.Count == 0 ? 0 : tmpList[tmpList.Count - 1]));
                            maxY = elementWithInput.AdvancedOption
                                ? 0
                                : Mathf.Max(DefaultInputFieldSize.y +
                                            DefaultSpriteOrTextSize.y + Margin.y + MarginBetweenSpriteOrTextAndInput,
                                    maxY);
                            break;
                        case Direction.Right:
                        case Direction.Left:
                            tmpList.Add(((elementWithInput.AdvancedOption)
                                            ? 0
                                            : DefaultInputFieldSize.x + DefaultSpriteOrTextSize.x +
                                              MarginBetweenSpriteOrTextAndInput +
                                              Margin.x) + (tmpList.Count == 0 ? 0 : tmpList[tmpList.Count - 1]));
                            maxY = (elementWithInput.AdvancedOption)
                                ? maxY : Mathf.Max( Math.Max(DefaultInputFieldSize.y,
                                                        DefaultSpriteOrTextSize.y) + Margin.y, maxY);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                marginsY.Add(maxY + (marginsY.Count == 0 ? 0 : marginsY[marginsY.Count - 1]));
                marginsX.Add(tmpList);
            }

            for (var i = 0; i < ParentOfCanvasElements.childCount; ++i)
                GameElementTransforms.Add(ParentOfCanvasElements.GetChild(i));

            var counter = 0;
            //расстановка элементов на сцене
            for (var i = 0; i < marginsX.Count; ++i)
            {
                for (var j = 0; j < marginsX[i].Count; ++j)
                {
                    var ge = GameElementsInLine[i].Elements[j];
                    //поле для ввода текста
                    var tmpInput = ParentOfCanvasElements.GetChild(counter * 2).GetComponent<RectTransform>();
                    //Спрайт или текст
                    var tmpSpriteOrText = ParentOfCanvasElements.GetChild(counter * 2 + 1).GetComponent<RectTransform>();

                    GameObject gameObject;
                    (gameObject = tmpInput.gameObject).SetActive(false);
                    animatedObjects.Add(gameObject);

                    switch (ge.Direction)
                    {
                        //если поле для ввода текста находится справа от текста(спрайта)
                        case Direction.Right:
                            //размер спрайта(текста)
                            var sizeDelta3 = tmpSpriteOrText.sizeDelta;
                            /*
                             * позиция спрайта(текста)
                             * marginsX[i][marginsX[i].Count - 1] / -2f - половина длины всех элементов в текущей строке
                             * стартовая позиция - половина длины всех элементов в текущей строке +  просчитанный отступ для данного элемента + половина размера элемента
                             * либо позиция введеная пользователем если оно указана 
                            */
                            Vector3 position1 = ge.AdvancedOption
                                ? ge.Position
                                : StartPosition + new Vector2(marginsX[i][marginsX[i].Count - 1] / -2f + (j == 0 
                                                                  ? 0
                                                                  : marginsX[i][j - 1]) + sizeDelta3.x / 2f,i == 0 
                                      ? marginsY[0] / -2f 
                                      : (marginsY[i] + marginsY[i - 1]) / -2f);
                            tmpSpriteOrText.localPosition = position1;
                            // Размер текстового поля
                            var delta3 = tmpInput.sizeDelta;
                            /*
                             * позиция текстового поля = позиция спрайта(текста) + отступ между текстовым полем и текстом(спрайтом) +
                             * половина размера текстового поля + половина размера текста(спрайта)
                             * либо позиция введеная пользователем если она указана
                            */
                            var advancedPos = new Vector3(position1.x + ge.Margin + sizeDelta3.x / 2f + delta3.x / 2f,
                                position1.y);
                            tmpInput.localPosition = ge.AdvancedOption
                                ? advancedPos
                                : position1 + new Vector3(delta3.x / 2f + sizeDelta3.x / 2f + (ge.AdvancedOption
                                          ? ge.Margin
                                          : MarginBetweenSpriteOrTextAndInput), 0, 0);
                            //расстановка в других случаех основывается на таком же принципе
                            break;
                        case Direction.Left:
                            var sizeDelta2 = tmpInput.sizeDelta;
                            Vector3 localPosition1 = ge.AdvancedOption
                                ? ge.Position
                                : StartPosition +
                                  new Vector2(
                                      marginsX[i][marginsX[i].Count - 1] / -2f + (j == 0 ? 0 : marginsX[i][j - 1]) +
                                      sizeDelta2.x / 2f,
                                      i == 0 ? marginsY[0] / -2f : (marginsY[i] + marginsY[i - 1]) / -2f);
                            tmpInput.localPosition = localPosition1;
                            var delta2 = tmpSpriteOrText.sizeDelta;
                            var advancedPos2 =
                                new Vector3(
                                    localPosition1.x + ge.Margin + delta2.x / 2f +
                                    sizeDelta2.x / 2f, localPosition1.y);
                            tmpSpriteOrText.localPosition = ge.AdvancedOption
                                ? advancedPos2
                                : localPosition1
                                  + new Vector3(delta2.x / 2f + sizeDelta2.x / 2f +
                                                (ge.AdvancedOption ? ge.Margin : MarginBetweenSpriteOrTextAndInput), 0,
                                      0);
                            break;
                        case Direction.Up:
                            var sizeDelta1 = tmpInput.sizeDelta;
                            var delta1 = tmpSpriteOrText.sizeDelta;
                            Vector3 position = ge.AdvancedOption
                                ? ge.Position
                                : StartPosition + new Vector2(
                                      marginsX[i][marginsX[i].Count - 1] / -2f + (j == 0
                                          ? marginsX[i][0] / 2f
                                          : (marginsX[i][j] + marginsX[i][j - 1]) / 2f), i == 0
                                          ? -sizeDelta1.y / 2f
                                          : (marginsY[i - 1] + marginsY[i]) / -2f -
                                            delta1.y / -2f);
                            tmpInput.localPosition = position;
                            tmpSpriteOrText.localPosition = ge.AdvancedOption
                                ? (Vector3) ge.Position
                                : position
                                  - new Vector3(0, delta1.y / 2f + sizeDelta1.y / 2f +
                                                   (ge.AdvancedOption ? ge.Margin : MarginBetweenSpriteOrTextAndInput),
                                      0);
                            break;
                        case Direction.Down:
                            var delta = tmpSpriteOrText.sizeDelta;
                            Vector3 localPosition = ge.AdvancedOption
                                ? ge.Position
                                : StartPosition + new Vector2(
                                      marginsX[i][marginsX[i].Count - 1] / -2f + (j == 0
                                          ? marginsX[i][0] / 2f
                                          : (marginsX[i][j] + marginsX[i][j - 1]) / 2f), i == 0
                                          ? -delta.y / 2f
                                          : -marginsY[i - 1] -
                                            delta.y / 2f);
                            tmpSpriteOrText.localPosition = localPosition;
                            var sizeDelta = tmpInput.sizeDelta;
                            var advancedPos4 = new Vector3(localPosition.x,
                                localPosition.y - ge.Margin - delta.y / 2f -
                                sizeDelta.y / 2f);

                            tmpInput.localPosition = ge.AdvancedOption
                                ? advancedPos4
                                : localPosition
                                  - new Vector3(0, delta.y / 2f + sizeDelta.y / 2f +
                                                   (ge.AdvancedOption ? ge.Margin : MarginBetweenSpriteOrTextAndInput),
                                      0);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    counter++;
                }
            }
        }

        #region IGame
        //проверка ответа
        //сравнение всех введеных ответов с правильными
        public bool CheckAnswer()
        {
            var correctAns = 0;
            var counter = 0;
            foreach (var gameElements in GameElementsInLine)
            {
                foreach (var gameElement in gameElements.Elements)
                {
                    if (gameElement.CorrectAnswer.ToLower().Trim() == _playerInputs[counter].text.ToLower().Trim())
                        correctAns++;           
                    counter++;
                }
            }

            return correctAns == counter;
        }

        //появление сцены
        public async Task SceneIn()
        {
            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.SetActive(true);
                AnimationUtility.ScaleIn(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        //исчезновение сцены
        public async Task SceneOut()
        {
            foreach (var animatedObject in animatedObjects)
            {
                AnimationUtility.ScaleOut(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        public void ChangeStateAfterAnswer()
        {
        }
        #endregion
    }

    [Serializable]
    public enum TextInInputFieldPos
    {
        Right, Left, Middle   
    }

    [Serializable]
    public class GameElements
    {
        [HideReferenceObjectPicker]
        public List<ElementWithInput> Elements = new List<ElementWithInput>();
    }

    [Serializable]
    public enum SpriteOrText
    {
        Sprite, Text
    }

    [Serializable]
    public enum Direction
    {
        Right, Left, Down, Up
    }

    [Serializable]
    public class ElementWithInput
    {
        [FoldoutGroup("Input Answer")]
        public string CorrectAnswer;
        [FoldoutGroup("Input Answer")]
        public bool AdvancedOption;
        
        [ShowIf(nameof(AdvancedOption)), FoldoutGroup("Input Answer")]
        public Vector2 InputSize;
        [ShowIf(nameof(AdvancedOption)), FoldoutGroup("Input Answer")]
        public Vector2 SpriteOrTextSize;
        [ShowIf(nameof(AdvancedOption)), FoldoutGroup("Input Answer")]
        public Vector2 Position;
        [ShowIf(nameof(AdvancedOption)), FoldoutGroup("Input Answer")]
        public float Margin;
        [FoldoutGroup("Input Answer")]
        public Direction Direction = Direction.Right;
        [FoldoutGroup("Input Answer")]
        public SpriteOrText SpriteOrText;
        
        [ShowIf(nameof(AdvancedOption)), FoldoutGroup("Input Answer")]
        public TMP_InputField InputField;
        
        [ShowIf(nameof(SpriteOrText), SpriteOrText.Sprite), FoldoutGroup("Input Answer")]
        public Sprite Sprite;

        [ShowIf(nameof(SpriteOrText), SpriteOrText.Text), FoldoutGroup("Input Answer")]
        public string text;
        
    }
}