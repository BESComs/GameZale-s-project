using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match
{
    [Serializable]
    public class CardMatch : IGame
    {
        //проверяет 
        [NonSerialized]public bool Checking;
        
        [TitleGroup("Card Match game properties", GroupID = "Card Match"), HideReferenceObjectPicker]
        //карты по линиям
        public List<CellInLine> cells = new List<CellInLine>();
        
        [HorizontalGroup("Card Match/BackSprite"), PropertyOrder(0)]
        //время поворота карты
        public float turningTime = .5f;
        [PreviewField(50f), HorizontalGroup("Card Match/BackSprite"), PropertyOrder(1)]
        //задний спрайт карты
        public Sprite backSprite;
        //центральная точка расположения всех карт
        public Vector2 center;
        //отступы между картами
        public Vector2 margin;
        //максимальный размер карт
        public Vector2 size;
        //все карты
        private List<Cell> _cacheCells;
        //индексы последних перевернутых карт
        [NonSerialized]public List<int> LastTwoCellsIndex;

        //игровой констркуктор
        private GameConstructor _gameConstructor;
        //родительский обьект
        private GameObject _parentObject;
        //анимированные обьекты
        private List<Renderer> _animatedObjects;
        //метод срабатывающий в начале игры
        public IGame Init(GameConstructor constructor, GameObject parentObject)
        {
            _gameConstructor = constructor;
            _parentObject = parentObject;
            
            _cacheCells = new List<Cell>();
            LastTwoCellsIndex = new List<int>();
            _animatedObjects = new List<Renderer>();
            CreateStartScene();

            return this;
        }

        //создание элементов игровой сцены
        private void CreateStartScene()
        {
            //отступы для всех карт
            var marginsX = new List<List<float>>();
            var marginsY = new List<float>();
            //созданные карты
            var tmpGos = new List<List<GameObject>>();
            //создание карт
            foreach (var cellInLine in cells)
            {
                marginsY.Add(0);
                marginsX.Add(new List<float>());
                var tmpGoInLine = new List<GameObject>();
                foreach (var cell in cellInLine.line)
                {
                    //создание одной карты
                    var tmpGo = new GameObject("Card");
                    tmpGo.gameObject.SetActive(false);
                    tmpGo.transform.SetParent(_parentObject.transform);
                    var sr = tmpGo.AddComponent<SpriteRenderer>();
                    _animatedObjects.Add(sr);
                    //настройки српайта
                    sr.sprite = backSprite;
                    sr.drawMode = SpriteDrawMode.Sliced;
                    //растягивание спрайта до size с учетом соотношения сторон
                    var size1 = sr.size;
                    size1 *= (!cell.customSize)
                        ? Mathf.Min(size.x / size1.x, size.y / size1.y)
                        : Mathf.Min(cell.cellSize.x / size1.x, cell.cellSize.y / size1.y);
                    sr.size = size1;
                    //подсчет отступов для данной карты
                    marginsY[marginsY.Count - 1] = Mathf.Max(marginsY[marginsY.Count - 1], size1.y);
                    marginsX[marginsX.Count - 1].Add(margin.x + size1.x + ((marginsX[marginsX.Count - 1].Count == 0)
                                                         ? 0 : marginsX[marginsX.Count - 1][marginsX[marginsX.Count - 1].Count - 1]));
                    tmpGoInLine.Add(tmpGo);
                    var cc = tmpGo.AddComponent<CellController>();
                    //сохранение всей информации о созданной карте
                    cc.currentCell = new Cell
                    {
                        index = _cacheCells.Count,
                        BackSprite = backSprite,
                        frontSprite = cell.frontSprite,
                        cellTransform = tmpGo.transform,
                        id = cell.id,
                        cellSize = sr.size,
                        cellPos = cell.cellPos,
                        customPos = cell.customPos
                    };
                    
                    cc.mainScript = this;
                    
                    _cacheCells.Add(cc.currentCell);
                    
                }
                marginsY[marginsY.Count - 1] += margin.y + ((marginsY.Count == 1) ? 0 : marginsY[marginsY.Count - 2]);
                tmpGos.Add(tmpGoInLine);
            }
            //расстановка всех карт на свои позиции
            var count = 0;
            for (var i = 0; i < tmpGos.Count; i++)
            {
                for (var j = 0; j < tmpGos[i].Count; j++)
                {
                    var cl = _cacheCells[count];
                    count++;
                    //если позиция задается вручную
                    if (cl.customPos)
                    {
                        tmpGos[i][j].transform.position = cl.cellPos;
                        continue;
                    }
                    //установка позиции в зависимости от высчетанных отступов и размера карты
                    tmpGos[i][j].transform.position = new Vector3(
                        marginsX[i][tmpGos[i].Count - 1] / 2f - (marginsX[i][j] -
                                                                 (cl.cellSize.x + margin.x) / 2f) + center.x,
                        marginsY[marginsY.Count - 1] / 2f -
                        (marginsY[i] - ((i == 0) ? marginsY[i] : marginsY[i] - marginsY[i - 1]) / 2f) + center.y, 0);
                    cl.cellPos = tmpGos[i][j].transform.position;
                }
            }
        }
        
        //проверка повернутых карт
        private async Task CheckTwoCells()
        {
            Checking = true;
            //две повернутые карты
            Cell cell1 = _cacheCells[LastTwoCellsIndex[0]], cell2 = _cacheCells[LastTwoCellsIndex[1]];
            cell1.cellInTask = cell2.cellInTask = true;
            
            //если повернуты парные карты
            if (cell1.id == cell2.id)
            {
                cell1.cellInTask = cell2.cellInTask = false;
                cell1.foundAPair = cell2.foundAPair = true;
                //проверка ответа
                if (CheckAnswer())
                {
                    //Активировать кнопку перехода на следующий уровень
                    _gameConstructor.GetNextButton.interactable = true;
                }
                
                Checking = false;
                return;
            }
            await new WaitForSeconds(1);
            //если повернуты не парные карты
            //поворачивает обе карты на 180 градусов
            (new Turn(cell1.cellTransform,AnimationCurve.EaseInOut(0,0,turningTime,1))).RunTask();
            (new Turn(cell2.cellTransform,AnimationCurve.EaseInOut(0,0,turningTime,1))).RunTask();
            await new WaitForSeconds(turningTime);
            cell1.cellTransform.GetComponent<SpriteRenderer>().sprite = cell1.BackSprite;
            cell2.cellTransform.GetComponent<SpriteRenderer>().sprite = cell1.BackSprite;
            await new WaitForSeconds(turningTime);
            cell1.inverted = cell2.inverted = false;
            cell1.cellInTask = cell2.cellInTask = false;
            Checking = false;
        }

        //развернуть карту
        public async Task RotateCell(Cell cell)
        {
            LastTwoCellsIndex.Add(cell.index);
            cell.cellInTask = true;
            //анимация поворота на 180 градусов
            new Turn(cell.cellTransform, AnimationCurve.EaseInOut(0, 0, turningTime, 1)).RunTask();
            await new WaitForSeconds(turningTime);
            //изменить задний спрайт на передний после поворота на 90
            cell.cellTransform.GetComponent<SpriteRenderer>().sprite = cell.frontSprite;
            await new WaitForSeconds(turningTime);
            //пометить как повернутый
            cell.inverted = true;
            cell.cellInTask = false;
            if (LastTwoCellsIndex.Count != 2 || Checking) return;
            //если повернуты две карты
            await CheckTwoCells();
            LastTwoCellsIndex.Clear();
        }

        #region Odin Related

        
        #endregion
        
        #region IGame
        //проверка все ли пары найдены
        public bool CheckAnswer()
        {
            foreach (var cacheCell in _cacheCells)
                if (!cacheCell.foundAPair)
                    return false;
        
            return true;
        }

        //появление на сцене
        public async Task SceneIn()
        {
            foreach (var animatedObject in _animatedObjects)
            {
                GameObject gameObject;
                (gameObject = animatedObject.gameObject).SetActive(true);
                AnimationUtility.ScaleIn(gameObject, 1.5f);
                await new WaitForSeconds(0.1f);
            }

            await new WaitForSeconds(1f);
        }

        //исчезнавение из сцене
        public async Task SceneOut()
        {
            foreach (var animatedObject in _animatedObjects)
            {
                AnimationUtility.ScaleOut(animatedObject.gameObject, speed: 1.5f);
                await new WaitForSeconds(0.1f);
            }
            
            await new WaitForSeconds(1f);
        }

        public void ChangeStateAfterAnswer()
        {
            // This game does not need to change state
            // after game finished
        }
        #endregion
    }

    [Serializable]
    public class CellInLine
    {
        [HideReferenceObjectPicker, LabelText("Line")]
        public List<Cell> line = new List<Cell>();

    }

    [Serializable]
    public class Cell
    {
        [HideInInspector]public int index;
        [HideInInspector]public Transform cellTransform;
        [HideInInspector]public bool foundAPair, cellInTask;
        
        [FoldoutGroup("Card", GroupID = "Card")]
        [HorizontalGroup("Card/Bools")]
        public bool customSize;
        [HorizontalGroup("Card/Bools")]
        public bool customPos;
        [HideInInspector]public bool inverted;
        
        [ShowIf(nameof(customSize)), FoldoutGroup("Card", GroupID = "Card")]
        public Vector2 cellSize;
        [ShowIf(nameof(customPos)), FoldoutGroup("Card", GroupID = "Card")]
        public Vector2 cellPos;
        [FoldoutGroup("Card", GroupID = "Card")]
        public int id;
        [HorizontalGroup("Card/Sprites"), PreviewField(50f)]
        public Sprite frontSprite;
        
        [NonSerialized] public Sprite BackSprite;
        
        
    }
}