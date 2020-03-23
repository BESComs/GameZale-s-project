using System.Linq;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Select
{
	public class TaskSelect : TaskParent, ITask,ILessonStatsObservable
	{
		[Space]
		public Counter Counter;
		public int CountingChecks = 10;
		[Space]
		public float LevelEndDelay = 1f;
		public bool LastLevelKeeping;
        
		private bool _complete;
		private LevelInTask[] _levels;
		private int _activeLevelId = -1;
		private ILevelEvent[] _activeLevelEvents;
		
		private void Awake()
		{
			_levels = GetComponentsInChildren<LevelInTask>(true);
			foreach (var level in _levels)
			{
				level.gameObject.SetActive(false);
			}
            
			if (Counter != null)
				Counter.Setup((CountingChecks == 0) ? _levels.Length : CountingChecks);
			maxBal = _levels.Length;
		}

		private void Start()
		{
			StartNextLevel();
		}

		private void StartNextLevel()
		{
			if (_activeLevelEvents != null)
				foreach (var levent in _activeLevelEvents)
					levent.OnLevelEnd();
			if(_activeLevelId == -1)
				RegisterLessonStart();
			Timeout.Set(this, (_activeLevelId == -1) ? 1f : LevelEndDelay, () =>
			{
				if (_activeLevelId != -1 && (_activeLevelId < _levels.Length - 1 || !LastLevelKeeping))
				{
					_levels[_activeLevelId].Completed = true;
					_levels[_activeLevelId].gameObject.SetActive(false);
				}

				if (_activeLevelId < _levels.Length - 1)
				{
					_activeLevelId++;
					_levels[_activeLevelId].gameObject.SetActive(true);
					_activeLevelEvents = _levels[_activeLevelId].GetComponentsInChildren<ILevelEvent>(true);
					foreach (var levent in _activeLevelEvents)
						levent.OnLevelStart();
					_forCheck = _levels[_activeLevelId].GetComponentsInChildren<SelectItem>(true);
				}
				else
				{
					RegisterLessonEnd();
					_complete = true;
					Timeout.Set(this, 1, LoadFinalScene);
					return;
				}
				
			});	
		}

		private SelectItem[] _forCheck;
	    public void CheckObjects()
	    {
		    if (CountingChecks != 0 && Counter != null)
			    Counter.Plus();
			    
		    if (_forCheck.Any(obj => !obj.Success))
		        return;
		    
		    RegisterAnswer(true);
		    
		    if (CountingChecks == 0 && Counter != null)
			    Counter.Plus();
		    StartNextLevel();
	    }

        public bool CheckTaskComplete()
        {
	        return _complete;
        }

		private void OnDisable()
		{
			StopAllCoroutines();
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
