using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Object = UnityEngine.Object;
#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Pair
{
    [Serializable]
    public class FindPair : IGame
    {
        [TitleGroup("Find Pair game properties", GroupID = "FindPair")]
        //вертикальная или горизонтальная ориентация
        public VorH VerticalOrHorizontal;
        //отступ между обьектами начальная позиция элементов размер
        public Vector2 Margin, StartPos, DefaultImageSize;
        
        [HideReferenceObjectPicker, OnValueChanged(nameof(SetElementsID))]
        //cпрайты с одной стороны с другой стороны
        public List<Img> Images1 = new List<Img>(), Images2 = new List<Img>();
        //соединяющая линия
        public Sprite LineSprite;
        private Transform _line;
        //задний фон обьектов
        public GameObject ImageBackFon;
        //выбран ли какой нибудь элемент
        [HideInInspector] public bool SelectImg;
        //последний выбранный элемент
        [HideInInspector] public Img LastClickedImage;
        public Color LineColor = Color.white;
        //игровой конструктор
        private GameConstructor gameConstructor;
        //родительский обьект
        private GameObject parentObject;
        //Обьекты который будут анимированны
        private List<Renderer> animatedObjects;
        [HideInInspector] public bool canMakeLine;
        //родительский обьект для линий
        private GameObject miscellaneousObjects;
        
        //стартовый метод
        public IGame Init(GameConstructor constructor, GameObject parentObject)
        {
            gameConstructor = constructor;
            this.parentObject = parentObject;
            animatedObjects = new List<Renderer>();
            canMakeLine = true;
            miscellaneousObjects = new GameObject("Miscellaneous Objects");
            miscellaneousObjects.transform.SetParent(parentObject.transform);
            
            CreateStartScene();

            return this;
        }
    
        //создание начальной сцены
        private void CreateStartScene()
        {
            //создание линии
            _line = new GameObject("Line Sprite").transform;
            var lr = _line.gameObject.AddComponent<SpriteRenderer>();
            lr.sprite = LineSprite;
            lr.color = LineColor; 
            lr.drawMode = SpriteDrawMode.Sliced;
            _line.localScale = Vector3.one;
            lr.size = Vector2.one;
            lr.sortingOrder = 1;
            lr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            _line.gameObject.SetActive(false);
            
            _line.SetParent(miscellaneousObjects.transform);
            
            var counter = 0;
            //создание обьектов с одной стороны
            CreateHalfScene(Images1, ++counter);
            //создание обьектов с другой стороны
            CreateHalfScene(Images2, ++counter);
        }

        private void CreateHalfScene(IReadOnlyList<Img> images, int lineNumber)
        {
            //родительский обьект для линий
            var lineParent = new GameObject("Line Parent");
            lineParent.transform.SetParent(parentObject.transform);
            for (var i = 0; i < images.Count; ++i)
            {
                //определение стороны
                images[i].InFirstColumn = (lineNumber == 1);
                images[i].Index = i;
                
                //создание заднего фона к выбераемым обьектам
                var tmpBackFon = Object.Instantiate(ImageBackFon.transform);
                tmpBackFon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                var color = Color.white;
                color.a = 0.2f;
                tmpBackFon.GetComponent<SpriteRenderer>().color = color;
                images[i].BackFon = tmpBackFon;
                
                //создание выбераемого обьекта
                var tmpGo = new GameObject();
                tmpGo.transform.position = Vector3.zero;
                var sr = tmpGo.AddComponent<SpriteRenderer>();
                tmpGo.AddComponent<SelectImage>();
                sr.sortingLayerName = "Top Layer";
                
                //маска для выбераемого обьекта
                var spriteMask = new GameObject();
                spriteMask.AddComponent<SpriteMask>().sprite = tmpBackFon.GetComponent<SpriteRenderer>().sprite;
                spriteMask.transform.SetParent(tmpGo.transform);
                tmpGo.SetActive(false);
                animatedObjects.Add(sr);
                
                //Настройки спрайта
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.sortingOrder = 1;
                sr.sprite = images[i].Sprite;
                Vector2 size;
                //изменение размера до указанного с учетом соотношения сторон
                var size1 = sr.size;
                sr.size *= images[i].Resize
                    ? Mathf.Min(images[i].ImgSize.x / size1.x, images[i].ImgSize.y / size1.y)
                    : Mathf.Min(DefaultImageSize.x / (size = sr.size).x, DefaultImageSize.y / size.y);

                //изменение размера заднего фона к выбераемым обьектам
                Vector3 localScale = (sr.size * sr.transform.localScale) / tmpBackFon.GetComponent<SpriteRenderer>().size;
                localScale = new Vector3(Mathf.Max(localScale.x, localScale.y), Mathf.Max(localScale.x, localScale.y));
                localScale += 0.2f * localScale;
                tmpBackFon.localScale = localScale;
                var si = tmpGo.GetComponent<SelectImage>();
                si.GameMainScript = this;
                tmpGo.transform.SetParent(lineParent.transform);
                //утсановка позиции в зависимости от вертикальной или горизонтальной ориентации
                switch (VerticalOrHorizontal)
                {
                    case VorH.Horizontal:
                        tmpGo.transform.position =
                            StartPos + new Vector2((images[i].Resize ? images[i].ImgSize.x: DefaultImageSize.x + Margin.x) * (i - images.Count / 2f + .5f), -Mathf.Pow(-1,lineNumber) * Margin.y / 2f);
                        break;
                    case VorH.Vertical:
                        tmpGo.transform.position = StartPos + new Vector2(Mathf.Pow(-1,lineNumber) *  Margin.x / 2f,
                                                       (images[i].Resize ? images[i].ImgSize.y: DefaultImageSize.y + Margin.y) * (i - images.Count / 2f + .5f));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                //сохранение информации в скрипт весящий на выбираемых элементах 
                images[i].BackFon.position = tmpGo.transform.position;
                images[i].BackFon.SetParent(tmpGo.transform);
                si.CurImg = images[i];
                si.CurImg.Position = tmpGo.transform.position;
                si.StartFunc();
                //добовление коллайдера и изменение его размера
                var collider = tmpGo.AddComponent<BoxCollider>();
                var scale = tmpBackFon.localScale;
                collider.size = tmpBackFon.GetComponent<SpriteRenderer>().size * scale;
                spriteMask.transform.localScale = scale;
            }
        }

        //эффект нажатия на выбираемые элементы
        public void ChildButton(Img img)
        {
            var tp = img.BackFon.GetComponent<SpriteRenderer>().color != Color.green;
            var color = Color.white;
            color.a = 0.2f;
            
            foreach (var img1 in Images1)
                if (!img1.HaveTransaction)
                    img1.BackFon.GetComponent<SpriteRenderer>().color = color;
        

            foreach (var img1 in Images2)
                if(!img1.HaveTransaction)
                    img1.BackFon.GetComponent<SpriteRenderer>().color = color;
        
            img.BackFon.GetComponent<SpriteRenderer>().color = tp ? Color.green : img.BackFon.GetComponent<SpriteRenderer>().color;
        }

        //проверка имеет ли img пару в момент нажатия на него
        public void CheckTransaction(Img img)
        {
            // если имеет пару удалить
            if (img.HaveTransaction)
                RemoveTransaction(img);
            //если не имеет пару и выбран элемент с другого столбца соеденить
            else if (SelectImg && LastClickedImage.InFirstColumn != img.InFirstColumn &&
                     !LastClickedImage.HaveTransaction && !img.HaveTransaction)
                MakeTransaction(img, LastClickedImage);
            else
            {
                SelectImg = true;
                LastClickedImage = img;
            }

            CheckAllHavePairs();
        }

        //создание соединения между элементами firstImg и secondImg
        private void MakeTransaction(Img firstImg, Img secondImg)
        {
            //изменить цвет заднего фона
            secondImg.BackFon.GetComponent<SpriteRenderer>().color = Color.green;
            firstImg.BackFon.GetComponent<SpriteRenderer>().color = Color.green;
            //создание линии
            var tmpLine = Object.Instantiate(_line, miscellaneousObjects.transform, true);
            tmpLine.gameObject.SetActive(true);
            //вычисление позиции угла и размера лини
            tmpLine.position = (firstImg.Position + secondImg.Position ) / 2f;
            var linePos = firstImg.Position - secondImg.Position;
            tmpLine.localScale = new Vector3(linePos.magnitude, 0.17f);
            tmpLine.rotation = Quaternion.Euler(0,0, Mathf.Atan(linePos.y / linePos.x) * Mathf.Rad2Deg);
            //назначение необходимых переменных для определения пары
            firstImg.Line = secondImg.Line = tmpLine;
            firstImg.HaveTransaction = secondImg.HaveTransaction = true;
            firstImg.CurrentPairIndex = secondImg.Index;
            secondImg.CurrentPairIndex = firstImg.Index;
            SelectImg = false;
        }

        //удалить соединение между двумя элементами
        private  void RemoveTransaction(Img firstImg)
        {
            //определение пары для firstImg
            var secondImg = (firstImg.InFirstColumn) ? Images2[firstImg.CurrentPairIndex] : Images1[firstImg.CurrentPairIndex];
            //пометить как элементы без пары
            firstImg.HaveTransaction = secondImg.HaveTransaction = false;
            //удаление линии между двумя обьектами
            Object.Destroy(firstImg.Line.gameObject);
            SelectImg = false;
            var color = Color.white;
            color.a = 0.2f; 
            
            firstImg.BackFon.GetComponent<SpriteRenderer>().color = color;
            secondImg.BackFon.GetComponent<SpriteRenderer>().color = color;
        }
        //проверка у всех ли элементов есть пара что бы была доступна кнопка проверки ответа
        private void CheckAllHavePairs()
        {
            if (Images1.Count < Images2.Count)
            {
                foreach (var img in Images1)
                    if (!img.HaveTransaction)
                    {
                        gameConstructor.GetNextButton.interactable = false;
                        return;
                    }
            }
            else
            {
                foreach (var img in Images2)
                    if (!img.HaveTransaction)
                    {
                        gameConstructor.GetNextButton.interactable = false;
                        return;
                    }
            }
            
            gameConstructor.GetNextButton.interactable = true;
        }

        #region IGame
        //проверка ответа
        public bool CheckAnswer()
        {
            //если у всех элементов которых должна быть пара она есть
            //и id пары совпадает с правильным
            foreach (var img in Images1)
            {
                if (img.DoNotHavePair)
                {
                    if (img.HaveTransaction)
                        return false;
                    
                    continue;
                }
                
                if (img.HaveTransaction &&
                    Images2[img.CurrentPairIndex].Id == img.CorrectPairId)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        //появление на сцену
        public async Task SceneIn()
        {
            foreach (var animatedObject in animatedObjects)
            {
                GameObject gameObject;
                (gameObject = animatedObject.gameObject).SetActive(true);
                AnimationUtility.ScaleIn(gameObject);
            }

            await new WaitForSeconds(1f);
        }

        //исчезновение из сцены
        public async Task SceneOut()
        {
            foreach (Transform transform in miscellaneousObjects.transform)
            {
                if (transform.gameObject.activeSelf && transform.GetComponent<Renderer>())
                {
                    AnimationUtility.FadeOut(transform.GetComponent<Renderer>());
                }
            }
            
            foreach (var animatedObject in animatedObjects)
            {
                AnimationUtility.ScaleOut(animatedObject.gameObject);
            }
            
            await new WaitForSeconds(1f);
        }

        public void ChangeStateAfterAnswer() => canMakeLine = false;
        
        #endregion
        
        #region Odin Related

        //установка id
        private void SetElementsID()
        {
            var id = 0;

            foreach (var img in Images1)
            {
                img.Id = id++;
            }
            foreach (var img in Images2)
            {
                img.Id = id++;
            }
        }
        
        #endregion
    }

    [Serializable]
    public class Img
    {
        [HideInInspector] public bool HaveTransaction;
        [HideInInspector] public int CurrentPairIndex, Index;
        [HideInInspector] public Transform Line, BackFon;
        [HideInInspector] public bool InFirstColumn;
        [HideInInspector] public Vector2 Position;
        [FoldoutGroup("Img Object", GroupID = "Img")]
        [HorizontalGroup("Img/bools")]
        public bool DoNotHavePair;
        [HorizontalGroup("Img/bools")]
        public bool Resize;
        [HorizontalGroup("Img/Ints"), HideIf(nameof(DoNotHavePair))]
        public int CorrectPairId;
        [HorizontalGroup("Img/Ints"), HideIf(nameof(DoNotHavePair)), ReadOnly]
        public int  Id;
        [FoldoutGroup("Img"), PreviewField(height: 50f, Alignment = ObjectFieldAlignment.Center)]
        public Sprite Sprite;
        [FoldoutGroup("Img"), ShowIf(nameof(Resize))]
        public Vector2 ImgSize;
    }

    public enum VorH
    {
        Vertical,
        Horizontal
    }
}