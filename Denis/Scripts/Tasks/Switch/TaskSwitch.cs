using System.Linq;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Switch
{
	public class TaskSwitch : TaskParent, ITask,ILessonStatsObservable
	{
		private void Awake()
		{
			_levels = GetComponentsInChildren<LevelInTask>(true);
			foreach (var level in _levels)
			{
				level.gameObject.SetActive(false);
			}
            
			if (Counter != null)
				Counter.Setup(_levels.Length);
			maxBal = _levels.Length;
		}
		private void Start()
		{
			StartNextLevel();
			inLoadNextLevel = false;
		}

		[Space]
		public Counter Counter;
		[Space]
		public float LevelEndDelay = 1f;
		public bool LastLevelKeeping;
		public bool IsComplete { get; private set; }
		public bool inLoadNextLevel; 
		private LevelInTask[] _levels;
		
	    private int _activeLevelId = -1;
		private ILevelEvent[] _activeLevelEvents;//
		private async void StartNextLevel()
		{
			if(_activeLevelId == -1)
				RegisterLessonStart();
			if (_activeLevelEvents != null)
				foreach (var levent in _activeLevelEvents)
					levent.OnLevelEnd();
			
			inLoadNextLevel = true;
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
					_forCheck = _levels[_activeLevelId].GetComponentsInChildren<SwitchItem>(true);
				}
				else
				{
					RegisterLessonEnd();
					IsComplete = true;
					Timeout.Set(this, 1, LoadFinalScene);
					return;
				}
				
			});

			await new WaitForSeconds(LevelEndDelay);
			inLoadNextLevel = false;
		}

		private SwitchItem[] _forCheck;
	    public void CheckObjects()
	    {
		    if (_forCheck.Any(obj => !obj.Success))
		        return;
		    
		    RegisterAnswer(true);
			foreach (var obj in _forCheck)
			    obj.MoveUp();
			
		    if (Counter != null)
			    Counter.Plus();
		    StartNextLevel();
	    }

        public bool CheckTaskComplete()
        {
	        return IsComplete;
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
