using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Random = UnityEngine.Random;
#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Puzzle
{
    [Serializable]
    public class Puzzle : IGame
    {
        //главная камера
        private Camera _mainCamera;
        
        //дистанция при которой происходит соединение элементов мозаики
        [TitleGroup("Puzzle game properties", GroupID = "Puzzle"), PropertyRange(0.05f, 0.5f)]
        public float epsilonJoiningDistance = 0.22f;
        
        //количество столбцов
        [OdinSerialize, HorizontalGroup("Puzzle/HorizontalVertical"), ValidateInput(nameof(ValidateGreaterThanZero)), OnValueChanged(nameof(SetCellsListSizeProperly))] 
        private int _columns = 1;
        
        //количество строк
        [OdinSerialize, HorizontalGroup("Puzzle/HorizontalVertical"), ValidateInput(nameof(ValidateGreaterThanZero)), OnValueChanged(nameof(SetCellsListSizeProperly))]
        private int _lines = 1;
        
        //список контроллеров мозаики
        public List<CellMoveController> cellMoveControllers;
        [OdinSerialize, BoxGroup("Puzzle Cells", GroupID = "Puzzle/Cells")]
        //матрица элементов мозаики
        public List<List<Cell>> puzzleCells;

        [Button("Generate IDs", ButtonSizes.Medium), BoxGroup(GroupID = "Puzzle/Cells")]
        
        //Генерация Id для элементов мозаики
        private void GenerateID()
        {
            for (int i = 0, id = 0; i < puzzleCells.Count; i++)
            {
                for (var j = 0; j < puzzleCells[i].Count; j++)
                {
                    puzzleCells[i][j].CellId = id++;
                }
            }
        }

        //Конечный спрайт
        [OdinSerialize] private Sprite completePuzzlePicture;
        /*
         *    список списков элементов пазла в одном списке хранится соединенные элементы пазла
         *    в начале каждый список содержит один элемент
         */
        private List<List<Cell>> _connectedCells;

        //центральная точка относительно которой происходит разброс элементов   
        [OdinSerialize]
        private Vector2 shuffleCenter;
        //максимальное расстояние от центра во время разброса
        [OdinSerialize]
        private Vector2 shuffleArea;
        
        //Время разборса
        [OdinSerialize, HorizontalGroup("Puzzle/Shuffle")]
        private int shuffleTimes = 5;
        //Скорость движения элементов во время разброса
        [OdinSerialize, HorizontalGroup("Puzzle/Shuffle")]
        private float shuffleSpeed = 1.5f;
        
        //родительский обьект 
        private GameObject parentObject;
        //родительский обьект к элементом пазла
        private GameObject piecesParent;
        private GameConstructor gameConstructor;
        //обьект на сцене который хранит sprite готового пазла 
        private GameObject completePuzzlePictureObject;

        //начальные позиции элементов пазла
        private List<Vector3> puzzlesOriginalCorrectPositions;
        public bool canMove { get; private set; }
        //переменная отвечающая за приоритет отрисовки спрайтов 
        //используется для того что бы последний взятый элемент имел самый большое значение SortingOrder
        [HideInInspector] public int lastSortingOrder = 1;
        //скрипт определяющий самый верхний элемент пазла на позиции курсора
        private CellRaycast cellRaycast;
        //Все предпочеты, создание необходимых элементов, добавление компонентов и разбрасывание по сцене элементов пазла
        public IGame Init(GameConstructor constructor, GameObject questionHolder)
        {
            //переменные для определения позиции piecesParent
            float deltaSizeX = 0;
            float deltaSizeY = 0;
            //инициализация переменных описанных выше
            _mainCamera = Camera.main;
            piecesParent = new GameObject("Puzzle Pieces");
            gameConstructor = constructor;
            parentObject = questionHolder;
            puzzlesOriginalCorrectPositions = new List<Vector3>();
            canMove = false;
            gameConstructor.GetNextButton.interactable = false;
            _connectedCells = new List<List<Cell>>();
            cellMoveControllers = new List<CellMoveController>();
            piecesParent.transform.SetParent(parentObject.transform, false);
            cellRaycast = piecesParent.AddComponent<CellRaycast>();
            cellRaycast.puzzle = this;
            //создание элементов пазла
            for (var i = 0; i < _lines; ++i)
            {
                for (var j = 0; j < _columns; ++j)
                {
                    var tmpGo = new GameObject();
                    tmpGo.AddComponent<SpriteRenderer>();
                    tmpGo.transform.SetParent(piecesParent.transform, false);
                    var sr = tmpGo.GetComponent<SpriteRenderer>();
                    sr.sprite = puzzleCells[i][j].CellSprite;
                    sr.drawMode = SpriteDrawMode.Sliced;
                    sr.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
                    sr.sortingOrder = lastSortingOrder++;
                    tmpGo.AddComponent<BoxCollider2D>();
                    var localScale = tmpGo.transform.localScale;
                    tmpGo.transform.localPosition = new Vector3(j, -i, 0) * sr.size * localScale;
                    var cell = new Cell
                    {
                        CellSize = sr.size * localScale,
                        CellTransform = tmpGo.transform,
                        CellSprite = puzzleCells[i][j].CellSprite,
                        CellId = puzzleCells[i][j].CellId,
                        DownCellId = (i != _lines - 1) ? (i + 1) * _columns + j : -1,
                        UpCellId = (i != 0) ? (i - 1) * _columns + j : -1,
                        RightCellId = (j != _columns - 1) ? i * _columns + j + 1 : -1,
                        LeftCellId = (j != 0) ? i * _columns + j - 1 : -1
                    };
                    puzzlesOriginalCorrectPositions.Add(tmpGo.transform.localPosition);
                    deltaSizeX = cell.CellSize.x;
                    deltaSizeY = cell.CellSize.y;
                    var cc = tmpGo.AddComponent<CellMoveController>();
                    cc.CurrentCell = cell;
                    cc.puzzle = this;
                    cellMoveControllers.Add(cc);
                    tmpGo.SetActive(false);
                    _connectedCells.Add(new List<Cell>(){cell});
                    cellRaycast.Cells.Add(cell);
                }
            }
            //Определение позиции родительского обьекта элементов пазла
            piecesParent.transform.localPosition = new Vector3(shuffleCenter.x - deltaSizeX * (_columns - 1 ) / 2f, shuffleCenter.y);
            if (completePuzzlePicture == null) return this;
            //инициализация обьекта конечный спрайт к пазлу
            completePuzzlePictureObject = new GameObject("Complete Puzzle Picture");
            completePuzzlePictureObject.SetActive(false);
            completePuzzlePictureObject.AddComponent<SpriteRenderer>().sprite = completePuzzlePicture;
            var spriteRenderer = completePuzzlePictureObject.GetComponent<SpriteRenderer>();
            spriteRenderer.size = completePuzzlePicture.bounds.size;
            spriteRenderer.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
            spriteRenderer.sortingOrder = 1000;
            completePuzzlePictureObject.transform.SetParent(parentObject.transform);
            completePuzzlePictureObject.transform.localPosition =
                new Vector3(shuffleCenter.x, shuffleCenter.y - deltaSizeY * (_lines - 1) / 2f);

            return this;
        }
        
        //установка всех взятыч элементов пазла на топ
        public void CellsOnTop(Cell cell)
        {
            var tmpCells = new List<Cell>();
            //поиск списка соединенных элементов содержащий элемент cell 
            foreach (var connectedCell in _connectedCells)
            {
                if (!connectedCell.Contains(cell)) continue;
                tmpCells = connectedCell;
                break;
            }
            //установка sortingOrder всех элементов в найденом списке наибольшим  
            foreach (var cellMoveController in cellMoveControllers)
            {
                if (tmpCells.Contains(cellMoveController.CurrentCell))
                {
                    cellMoveController.spriteRenderer.sortingOrder = lastSortingOrder;
                }
            }
        }

        //разброс элементов пазла в начале игры
        private async Task Shuffle()
        {
            //каждую секунду все элементы меняют позицию на рандомную позицию с учетом позиции родительского обьекта (зависит от shuffleCenter) и shuffleArea
            
            for (var i = 0; i < shuffleTimes; i++)
            {
                foreach (var connectedCell in _connectedCells)
                {
                    foreach (var cell in connectedCell)
                    {
                        AnimationUtility.MoveTo(cell.CellTransform.gameObject,
                            new Vector2(shuffleArea.x * (Random.value - .5f), shuffleArea.y * (Random.value - .5f)) - (Vector2) piecesParent.transform.localPosition, shuffleSpeed);
                    }
                }

                await new WaitForSeconds(1f / shuffleSpeed);
            }
        }
        //Движение элементов пазла
        public void MoveCells(CellMoveController cell, Vector3 previousMousePosition)
        {
            //cell - элемент пазла на который наведен курсор
            //previousMousePosition - позиция мышки в предыдущем кадре
            //соединённые элементы пазла к элементу cell
            var cells = _connectedCells[FindComponent(cell.CurrentCell)];
            //cдвиг за кадр
            var tmp = _mainCamera.ScreenToWorldPoint((Vector2)Input.mousePosition) - _mainCamera.ScreenToWorldPoint((Vector2)previousMousePosition);
            //cдвиг элемента cell
            cell.CurrentCell.CellTransform.position += tmp;
            //cдвиг остальных соединённых элементов пазла
            foreach (var cell1 in cells)
            {
                if (cell.CurrentCell == cell1) continue;
                cell1.CellInMove = 1;
                cell1.CellTransform.position += tmp;
            }
        }
        
        //Слияние двух списков
        //каждый список содержит соединенные элементы пазла
        private static List<Cell> MergeTwoComponents(IEnumerable<Cell> comp1, IEnumerable<Cell> comp2)
        {
            var mergeList = comp1.ToList();
            mergeList.AddRange(comp2);
            var correctMergeList = new List<Cell>();
            foreach (var cell in mergeList)
            {
                foreach (var cell1 in mergeList)
                {
                    if(cell.CellId != cell1.CellId && !correctMergeList.Contains(cell))
                        correctMergeList.Add(cell);
                }
            }
            return correctMergeList; 
        }

        //cell элемент пазла на котором в момент отжатия был курсор
        //поиск списка соединенных между собой элементов пазла к которому можно присоединить список содержащий cell
        public async void ConnectCell(Cell cell)
        {
            canMove = false;
            //индекс листа содержащий cell
            var k = FindComponent(cell);
            //лист содержащий cell
            var tmpList = _connectedCells[k];
            _connectedCells.Remove(_connectedCells[k]);
            var indexList = new List<Cell>();
            /*
              поиск элементов пазла соседних ко всем элементам из списка содержащего cell
              первый цикл проход по всем элементам из списка содержащего cell
              второй цикд проход по всем спискам (*) каждый из списков содержит соединенные между собой элементы пазла
              третий список  проход по списку (*) 
              тело самого внутреннего цикла - проверка является ли элемент из (*) правым, левым верхним или нижним соседом элемента из списка содержащего cell 
            */
            foreach (var cell1 in tmpList)
            {
                foreach (var connectedCell in _connectedCells)
                {
                    foreach (var cell2 in connectedCell)
                    {
                        var tmpDist = cell1.CellTransform.position - cell2.CellTransform.position;
                    
                        if (cell1.LeftCellId == cell2.CellId)
                        {
                            if (!(Mathf.Abs(tmpDist.x - cell1.CellSize.x) < epsilonJoiningDistance) ||
                                !(Mathf.Abs(tmpDist.y) < epsilonJoiningDistance)) continue;
                            if(!indexList.Contains(cell2))
                                indexList.Add(cell2);
                        }

                        if (cell1.RightCellId == cell2.CellId)
                        {
                            if (!(Mathf.Abs(-tmpDist.x - cell1.CellSize.x) < epsilonJoiningDistance) ||
                                !(Mathf.Abs(tmpDist.y) < epsilonJoiningDistance)) continue;
                            if(!indexList.Contains(cell2))
                                indexList.Add(cell2);
                        }

                        if (cell1.UpCellId == cell2.CellId)
                        {
                            if (!(Mathf.Abs(-tmpDist.y - cell1.CellSize.y) < epsilonJoiningDistance) ||
                                !(Mathf.Abs(tmpDist.x) < epsilonJoiningDistance)) continue;
                            if(!indexList.Contains(cell2))
                                indexList.Add(cell2);
                        }

                        if (cell1.DownCellId != cell2.CellId) continue;
                        if (!(Mathf.Abs(tmpDist.y - cell1.CellSize.y) < epsilonJoiningDistance) ||
                            !(Mathf.Abs(tmpDist.x) < epsilonJoiningDistance)) continue;
                        if (!indexList.Contains(cell2))
                            indexList.Add(cell2);
                    }
                } 
            }
            //лист листов каждый лист хранит все связанные между собой элементы у которых есть хотя бы один соседний элемент к cell или списку содержащего cell
            var tmpList2 = new List<List<Cell>>();
            
            foreach (var cell1 in indexList)
            {
                if(!tmpList2.Contains(_connectedCells[FindComponent(cell1)]))
                    tmpList2.Add(_connectedCells[FindComponent(cell1)]);
            }
            //список соединенных листов
            var removeList = new List<List<Cell>>();
            //Время затраченное на соединение
            float awaitTime = 0;
            //соединение всех найденных списков
            foreach (var variable in tmpList2)
            {
                if(variable.Count != 0 && tmpList2.Count != 0)
                    awaitTime += .1f;
                SmoothMove(variable, tmpList);
                tmpList = MergeTwoComponents(tmpList, variable);
                removeList.Add(variable);
            }

            //удаление листов которые были слиты с листом содержащий cell
            await new WaitForSeconds(awaitTime);
            foreach (var variable in removeList)
                _connectedCells.Remove(variable);
        
            foreach (var connectedCell in _connectedCells)
                if (connectedCell.Count == 0)
                    _connectedCells.Remove(connectedCell);
        
            //добовление слитого листа 
            _connectedCells.Add(tmpList);
            canMove = true;
            //проверка ответа если ответ правильный все элементы двигаются в начальную позицию
            if (!CheckAnswer()) return;
            canMove = false;
            await new WaitForSeconds(1f);
                
            for (var i = 0; i < piecesParent.transform.childCount; i++)
            {
                AnimationUtility.MoveTo(piecesParent.transform.GetChild(i).gameObject, puzzlesOriginalCorrectPositions[i]);
            }

            await new WaitForSeconds(1f);
                
            if (completePuzzlePicture != null)
            {
                completePuzzlePictureObject.SetActive(true);
                AnimationUtility.FadeIn(completePuzzlePictureObject.GetComponent<SpriteRenderer>(), 0.8f);
            }
                
            gameConstructor.GetNextButton.interactable = true;
        }

        //плавное движение при присоединении
        private static async void SmoothMove(List<Cell> tmpCells1, List<Cell> tmpCells2)
        {
            var cells1 = tmpCells1.GetRange(0, tmpCells1.Count);
            var cells2 = tmpCells2.GetRange(0, tmpCells2.Count);
        
            if(cells1.Count == 0 || cells2.Count == 0) return;
            
            var breakFlag = false;
            var shift = new Vector3();
            foreach (var cell1 in cells1)
            {
                foreach (var cell2 in cells2)
                {
                    if (cell1.CellId == cell2.RightCellId)
                    {
                        shift = -cell1.CellTransform.localPosition + cell2.CellTransform.localPosition + new Vector3(cell1.CellSize.x,0,0);
                    }
                    else if (cell1.CellId == cell2.LeftCellId)
                    {
                        shift = -cell1.CellTransform.localPosition + cell2.CellTransform.localPosition - new Vector3(cell1.CellSize.x,0,0);
                    }
                    else if(cell1.CellId == cell2.UpCellId)
                    {
                        shift = -cell1.CellTransform.localPosition + cell2.CellTransform.localPosition + new Vector3(0,cell1.CellSize.y,0);
                    }
                    else if (cell1.CellId == cell2.DownCellId)
                    {
                        shift = -cell1.CellTransform.localPosition + cell2.CellTransform.localPosition - new Vector3(0,cell1.CellSize.y,0);
                    }
                    else
                        continue;

                    breakFlag = true;
                    break;
                }

                if (breakFlag)
                    break;
            }

            const float speed = 10f;
        
            foreach (var cell in cells1)
            {
                AnimationUtility.MoveTo(cell.CellTransform.gameObject, cell.CellTransform.localPosition + shift, speed);
            }

            await new WaitForSeconds(1f / speed);
        }

        //индекс листа в котором содержится cell
        private int FindComponent(Cell cell)
        {
            var i = 0;
            for (; i < _connectedCells.Count; ++i)
            {
                if(_connectedCells[i].Contains(cell))
                    break;
            }
            return i;
        }

        
        #region Odin Related

        private static bool ValidateGreaterThanZero(int value) => value > 0;

        //метод добавления/удаления строк/столбцов если были не правильно указанны lines columns
        private void SetCellsListSizeProperly()
        {           
            if (puzzleCells.Count > _lines)
            {
                while (puzzleCells.Count != _lines)
                {
                    puzzleCells.RemoveAt(puzzleCells.Count - 1);
                }
            }
            else if (puzzleCells.Count < _lines)
            {
                while (puzzleCells.Count != _lines)
                {
                    puzzleCells.Add(new List<Cell>());
                }
            }

            foreach (var columnsList in puzzleCells)
            {
                if (columnsList.Count > _columns)
                {
                    while (columnsList.Count != _columns)
                    {
                        columnsList.RemoveAt(columnsList.Count - 1);
                    }
                }
                else if(columnsList.Count < _columns)
                {
                    while (columnsList.Count != _columns)
                    {
                        columnsList.Add(new Cell());
                    }
                }
            }
            
            GenerateID();
        }

        #endregion

        #region IGame methods

        //так как список содержит списки элементов соединенных между собой то если список один значит все элементы пазла соедененны между собой
        public bool CheckAnswer() => _connectedCells.Count == 1;
        
        //появление сцены в начале игры
        public async Task SceneIn()
        {
            foreach (Transform cell in piecesParent.transform)
            {
                cell.gameObject.SetActive(true);
                AnimationUtility.FadeIn(cell.GetComponent<SpriteRenderer>());
            }

            await new WaitForSeconds(2f);
            
            await Shuffle();
            canMove = true;
        }

        //уход сцены в конце игры
#pragma warning disable 1998
        public async Task SceneOut()
#pragma warning restore 1998
        {
            if (completePuzzlePicture != null)
            {
                AnimationUtility.FadeOut(completePuzzlePictureObject.GetComponent<SpriteRenderer>());
                piecesParent.SetActive(false);
            }
            else
            {
                foreach (Transform cell in piecesParent.transform)
                {
                    AnimationUtility.FadeOut(cell.GetComponent<SpriteRenderer>());
                }
            }
        }

        public void ChangeStateAfterAnswer()
        {
            
        }
        
        #endregion
    }

    
    [Serializable]
    public class Cell
    {
        [HideInInspector] public Vector3 CellSize;
        [HideInInspector] public Transform CellTransform;

        [HideInInspector] public int CellInMove;
        
        [ReadOnly]
        public int CellId;
        //id соседних элементов
        [HideInInspector] public int RightCellId, LeftCellId, UpCellId, DownCellId;
        [PreviewField(50f)]
        public Sprite CellSprite;
        
    }
}