using System.Linq;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Paint
{
	public class TaskPaint : TaskParent, ITask,ILessonStatsObservable
	{
		[HideInInspector]public PaintColor activeColor;
		
		private PaintPiece[] _checkPieces;
		
		private void Awake()
		{
			_checkPieces = GetComponentsInChildren<PaintPiece>();
			maxBal = 1;
			RegisterLessonStart();
		}
		
		public void CheckPieces()
		{
			if(_checkPieces.Any(piece => !piece.IsPainted))
				return;
			RegisterAnswer(true);
			RegisterLessonEnd();
			LoadFinalScene();
		}

		public bool CheckTaskComplete()
		{
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
