using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Work_Directory.Denis.Scripts.Tasks;

namespace Work_Directory.Denis.Mozaika
{
    public class MazaikaManager : TaskParent,ILessonStatsObservable
    {
        //для проверки на конец игры
        public bool gameIsEnd;
        //объекты хранящие закрашиваемые и исходные элементы мозаики
        public Transform t1, t2;
        public static MazaikaManager Instance;
        //Выбранный игроком цвет
        private Color selectedColor;
        //лист окрашиваемых объектов
        private readonly List<MazaikaCellController> mazaikaCellControllers1 = new List<MazaikaCellController>();
        //лист закрашенных объектов используется для проверки
        private readonly List<MazaikaCellController> mazaikaCellControllers2 = new List<MazaikaCellController>();
        public Transform wrong;

        private void Awake()
        {
            for (var i = 0; i < t1.childCount; i++)
                mazaikaCellControllers1.Add(t1.GetChild(i).GetComponent<MazaikaCellController>());
            
            for (var i = 0; i < t2.childCount; i++)
                mazaikaCellControllers2.Add(t2.GetChild(i).GetComponent<MazaikaCellController>());
            RegisterLessonStart();
            Instance = this;
            selectedColor = Color.white;
        }

        //покраска элемента мозаики в выбранный цвет
        public void SetColor(MazaikaCellController mazaikaCellController)
        {
            mazaikaCellController.currentColor = selectedColor;
        }

        //установка выбранного цвета
        public void GetColor(MazaikaColorCellController mazaikaColorCellController)
        {
            selectedColor = mazaikaColorCellController.cellColor;
        }

        //проверка ответа игрогка
        public void CheckAnswer()
        {
            //true если все элементы в правой таблицы такого же цвета как в левой таблице
            gameIsEnd = !mazaikaCellControllers1.Where((t, i) =>
                ((Vector4) t.currentColor - (Vector4) mazaikaCellControllers2[i].currentColor).magnitude > .1f).Any();
        }

        //если задание выполнено и нажата кнопка проверки то загружается финальная сцена
        public void GameEnd()
        {
            RegisterLessonEnd();
            RegisterAnswer(true);
            transform.parent.GetComponent<TaskParent>().LoadFinalScene();
        }
        
        public int MaxScore { get => 1; set {  } }
        public void RegisterAnswer(bool isAnswerRight){
            if (!isAnswerRight) return;
            LessonStatistic.SetScore(1);
            LessonStatistic.SetRight(isAnswerRight);
		}

		public void RegisterLessonStart(){
			LessonStatistic.SetStartLessonTime();
		}
		
		public void RegisterLessonStart(int lessonNumber)  //?
		{
			throw new System.NotImplementedException();
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
//использовалась для автоматического создания объектов на сцене
/*[Button]
        public Vector2 startPos;
        public Vector2 margin;
        public Sprite sprite;
        public int columnCount;
        public int lineCount;
        public float evenMargin;
        public void DestroyObjects()
        {
            var count = transform.childCount;
            for (var i = 0; i < count; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        [Button]
        public void CreateObjects()
        {
            
            var currentPos = startPos;
            for (var i = 0; i < lineCount; i++)
            {
                for (var j = 0; j < columnCount; ++j)
                {
                    var tmp = new GameObject();
                    tmp.AddComponent<SpriteRenderer>().sprite = sprite;
                    tmp.transform.SetParent(transform);
                    tmp.transform.localPosition = currentPos;
                    if (j % 2 == 0)
                        tmp.transform.localPosition += new Vector3(0, evenMargin,0);
                    currentPos.x += margin.x;
                }
                currentPos.y += margin.y;
                currentPos.x = 0;
            }
        }*/