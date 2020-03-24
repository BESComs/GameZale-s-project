using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Work_Directory.Denis.Scripts;
using Work_Directory.Denis.Scripts.Tasks;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scenes.maze.Scripts
{
    public class MazeGameManager : TaskParent,ILessonStatsObservable
    {
        //стрелки указывающие на начало и конец лабиринта
        public Transform startArrow, finalArrow;
        public bool inTask;
        //объект загораживающий выход из лабиринта
        public Transform finalDoor;
        //объект которым управляет игрок
        public Transform bee;
        public static MazeGameManager Instance;
        //все ячейки по которым строятся пути в лабиринте
        public List<List<PathPointController>> pathPointControllers;
        //количество ячеек по вертикали и по горизонтали
        public int lineNumber, columnNumber;
        //ячейка в которой находится игрок в данный момент времени
        public PathPointController currentCell;
        //обьект содержащий все клетки
        public Transform cellHolder;
        //монеты которые нужно собрать во время прохождения лабиринта
        public List<Transform> coins;
        //количество собранных монет
        public int coinCounter;
        public bool gameStart;
        //используется для анимации стрелок
        private float timer = .5f;
        private float sign = -1f;
        private Vector2 startArrowScale, finalArrowScale;
        
        private void Awake()
        {
            Instance = this;
            finalArrowScale = finalArrow.localScale;
            startArrowScale = startArrow.localScale;
            //добавление в список всех ячеек с ее индексом
            pathPointControllers = new List<List<PathPointController>>();
            var counter = 0;
            for (var i = 0; i < lineNumber; i++)
            {
                var tmpList = new List<PathPointController>();
                for (var j = 0; j < columnNumber; j++)
                {
                    var tmpCell = cellHolder.GetChild(counter).GetComponent<PathPointController>();
                    tmpCell.currentCellIndex = (i, j);
                    tmpList.Add(tmpCell);
                    counter++;
                }
                pathPointControllers.Add(tmpList);
            }
            //поиск стартовой ячейки
            foreach (var pathPointController in pathPointControllers)
            {
                foreach (var pointController in pathPointController)
                {
                    if (!pointController.startCell) continue;
                    currentCell = pointController;
                    currentCell.canMove = true;
                    break;
                }
            }
        }
        //анимирование стрелок
        private void Update()
        {         
            timer += sign * Time.deltaTime;
            if (timer < 0)
                sign = 1f;
            else if (timer > .4)
                sign = -1f;
            startArrow.localScale = startArrowScale * (1f - timer);
            finalArrow.localScale = finalArrowScale * (1f - timer);
        }

        private async void Start()
        {
            inTask = true;
            //активация ячеек в которые игрок может пойти в начале игры
            await ActivateCell(currentCell);
            RegisterLessonStart();
            inTask = false;
        }
        
        //Движение из текущей ячейки в выбранную
        private async Task MoveToCell(PathPointController cell)
        {
            var position = bee.position;
            var startPos = position;
            var t = 0f;
            //поворот в сторону движения
            var localScale = bee.localScale;
            localScale = (position - cell.transform.position).x < -.1f
                ? new Vector3( Mathf.Abs(localScale.x), localScale.y, 1)
                : new Vector3(-1 * Mathf.Abs(localScale.x), localScale.y, 1);
            bee.localScale = localScale;
            //движение
            while ((bee.position - cell.transform.position).magnitude > .1f)
            {
                bee.position = startPos + (cell.transform.position - startPos).normalized * t;
                t += .15f;
                await new WaitForUpdate();
            }
            bee.position = cell.transform.position;
            //подобрать монету если рядом
            foreach (var coin in coins)
            {
                if (!((coin.position - bee.position).magnitude < .25f)) continue;
                coinCounter += 1;
                coin.position = new Vector3(coinCounter/1.5f - 5/3f, 4.075f);
                break;
            }
            //если собрана последняя монета то открыть выход
            if (coinCounter == 5 && !finalArrow.gameObject.activeSelf)
            {
                new Fade(finalDoor).RunTask();
                finalArrow.gameObject.SetActive(true);
                new Fade(finalArrow, Mode.In).RunTask();
                foreach (var pathPointController in pathPointControllers)
                {
                    foreach (var pointController in pathPointController)
                    {
                        if (!pointController.finalCell) continue;
                        pointController.activeCell = true;
                        break;
                    }
                }
            }
            //если выбранная ячейка является финальной то загрузить конец игры
            if(cell.finalCell)
            {
                RegisterAnswer(true);
                RegisterLessonEnd();
                transform.parent.GetComponent<TaskParent>().LoadFinalScene();
                return;
            }
            await ActivateCell(cell);
        }

        //поиск ячеек в которые может пойти игрок после того как он пришел в определенную ячейку
        private async Task ActivateCell(PathPointController cell)
        {
            var activateCell = new  List<List<PathPointController>>();
            currentCell = cell;
            var (item1, item2) = cell.currentCellIndex;
            var tmpList =  new List<PathPointController>();
            //поиск ячеек сверху от текущей позиции
            for (var i = item1; i < pathPointControllers.Count; i++)
            {
                var tmp = pathPointControllers[i][item2];
                if(!tmp.activeCell)
                    break;
                tmpList.Add(tmp);
            }
            activateCell.Add(tmpList.GetRange(0,tmpList.Count));
            tmpList.Clear();
            //поиск ячеек снизу от текущей позиции
            for (var i = item1; i >= 0; i--)
            {
                var tmp = pathPointControllers[i][item2];
                if(!tmp.activeCell)
                    break;
                tmpList.Add(tmp);
            }
            activateCell.Add(tmpList.GetRange(0,tmpList.Count));
            tmpList.Clear();
            
            //поиск ячеек справа от текущей позиции
            for (var i = item2; i < pathPointControllers[item1].Count; i++)
            {
                var tmp = pathPointControllers[item1][i];
                if(!tmp.activeCell)
                    break;
                tmpList.Add(tmp);
            }
            activateCell.Add(tmpList.GetRange(0,tmpList.Count));
            tmpList.Clear();
            //поиск ячеек слева от текущей позиции
            for (var i = item2; i >= 0; i--)
            {
                var tmp = pathPointControllers[item1][i];
                if(!tmp.activeCell)
                    break;
                tmpList.Add(tmp);
            }
            activateCell.Add(tmpList.GetRange(0,tmpList.Count));
            //активирование всех найденных ячеек
            for (var i = 0; i < Mathf.Max(activateCell[0].Count,activateCell[1].Count,activateCell[2].Count,activateCell[3].Count); i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if(this == null) return;
                    if (activateCell[j].Count <= i) continue;
                    activateCell[j][i].gameObject.SetActive(true);
                    activateCell[j][i].playAnimation = true;
                    activateCell[j][i].canMove = true;
                    await new WaitForSeconds(Time.deltaTime);
                    activateCell[j][i].PlayAnimation();
                }
            }
        }

        //диактивация всех ячеек
        public async void DisableAllCells(PathPointController cell)
        {
            inTask = true;
            foreach (var pathPointController in pathPointControllers)
            {
                foreach (var pointController in pathPointController)
                {
                    pointController.playAnimation = false;
                    pointController.canMove = false;
                    pointController.gameObject.SetActive(false);
                }
            }
            await MoveToCell(cell);
            inTask = false;
        }

        public int MaxScore
        {
            get => 1;
            set {  }
        }

        public void RegisterAnswer(bool isAnswerRight){
            if (!isAnswerRight) return;
            LessonStatistic.SetScore(1);
            LessonStatistic.SetRight(isAnswerRight);
		}

		public void RegisterLessonStart(){
			LessonStatistic.SetStartLessonTime();
		}
		

		public void RegisterLessonEnd(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
			ServerRequests.PostStatistics();
		}
		public void OnApplicationPause(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
		}
    }
}