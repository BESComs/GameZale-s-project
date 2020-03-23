using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Work_Directory.Denis.Scripts.Tasks;

namespace Work_Directory.Denis.Scenes.Pazzle
{
    public class SeaPuzzleManager : TaskParent, ILessonStatsObservable
    {
        public static SeaPuzzleManager Instance;
        //объект содержащий все элементы пазла
        public Transform puzzleCellHolder;
        //объект содержащий все правильные позиции соответствующих элементов пазла
        public Transform correctPosHolder;
        [SerializeField] private List<PuzzleCellController> cells;
        
        private void Awake()
        {
            Instance = this;
            cells = new List<PuzzleCellController>();
            for (var i = 0; i < puzzleCellHolder.childCount; i++)
            {
                var cell = puzzleCellHolder.GetChild(i).GetComponent<PuzzleCellController>();
                cell.id = i;
                cell.correctPos = correctPosHolder.GetChild(i).position;
                cells.Add(cell);
            }
            RegisterLessonStart();
        }

        //проверка ответа вызывается когда ставится очередной элемент пазла
        private bool CheckAnswer()
        {
            //все ли элементы пазла лежат на своих позициях
            return cells.All(puzzleCellController => puzzleCellController.inCorrectPos);
        }
     
        //проверка позиции вызывается тогда когда игрок отпустил элемент пазла
        public void CheckPos(PuzzleCellController puzzleCellController)
        {
            puzzleCellController.inMove = false;
            puzzleCellController.moveToStartPos = false;
            //если перетаскиваемый элемент находится на расстоянии меньше чем 1/2 то он ставится на правильное место 
            //иначе ставится на начальную позицию
            if ((puzzleCellController.transform.position - correctPosHolder.GetChild(puzzleCellController.id).position)
                .magnitude < .5f)
            {
                puzzleCellController.inCorrectPos = true;
                puzzleCellController.moveToCorrectPos = true;
                if (!CheckAnswer()) return;
                RegisterAnswer(true);
                RegisterLessonEnd();
                transform.parent.GetComponent<TaskParent>().LoadFinalScene();
            }
            else
            {
                puzzleCellController.moveToStartPos = true;
            }
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
