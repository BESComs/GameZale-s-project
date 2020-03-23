using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Mosaic
{
    public class TaskMosaic : TaskParent, ITask,ILessonStatsObservable
    {
        private void Awake()
        {
            _checkPieces = GetComponentsInChildren<MosaicPiece>();
            maxBal = 1;
            RegisterLessonStart();
        }
        
        [FormerlySerializedAs("ActiveColor")] public MosaicColor activeColor;
        
        private MosaicPiece[] _checkPieces;

        public bool CheckTaskComplete()
        {
            var counter = 0;
            foreach (var mosaicPiece in _checkPieces)
            {
                if (mosaicPiece.Color() != mosaicPiece.RequredColor)
                    counter++;
            }
            if (_checkPieces.Any(piece => piece.Color() != piece.RequredColor))
                return false;
            RegisterAnswer(true);
            RegisterLessonEnd();
            LoadFinalScene();
            
            return true;
        }


        private int maxBal;
        public int MaxScore
        {
            get => maxBal;
            set => maxBal = value;
        }

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
