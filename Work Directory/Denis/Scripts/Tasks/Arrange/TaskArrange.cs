using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Arrange
{
	public class TaskArrange : TaskParent, ITask, ILessonStatsObservable
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
		}

		public float DroppingShift = 0.3f;
		[Space]
		public Counter Counter;
		[Space]
		public float LevelEndDelay = 1f;//
		public bool LastLevelKeeping;//
        
        private bool _complete;
		private LevelInTask[] _levels;

		private int _activeLevelId = -1;
		private ILevelEvent[] _activeLevelEvents;//
		private void StartNextLevel()
		{
			if(_activeLevelId == -1)
				RegisterLessonStart();
				
			if (_activeLevelEvents != null)
				foreach (var levent in _activeLevelEvents)
					levent.OnLevelEnd();
			Timeout.Set(this, (_activeLevelId == -1) ? 1f : LevelEndDelay, () =>
			{
				if (_activeLevelId != -1 && (_activeLevelId < _levels.Length - 1 || !LastLevelKeeping))
				{
					_levels[_activeLevelId].Completed = true;
					_levels[_activeLevelId].gameObject.SetActive(false);
				}

				if (_activeLevelId < _levels.Length - 1)
				{
					_allArranged = false;
					_activeLevelId++;
					_levels[_activeLevelId].gameObject.SetActive(true);
					_activeLevelEvents = _levels[_activeLevelId].GetComponentsInChildren<ILevelEvent>(true);
					foreach (var levent in _activeLevelEvents)
						levent.OnLevelStart();
					_forCheck = _levels[_activeLevelId].GetComponentsInChildren<ArrangeItem>(true);
				}
				else
				{
					RegisterLessonEnd();
					_complete = true;
					Timeout.Set(this, 1, LoadFinalScene);
					return;
				}

			});	
		}//

		private bool _allArranged;
		private ArrangeItem[] _forCheck;
	    public void CheckTangramPieces()
	    {
		    foreach (var item in _forCheck)
		    {
			    var contain = false;
			    foreach (var req in item.RequiredPositions)
				    if (req && item.PairedPosition == req)
					    contain = true;
			    
			    if (!contain)
			    {
				    return;
			    }
		    }		

		    _allArranged = true;
		    if (Counter != null)
			    Counter.Plus();
		   	RegisterAnswer(true);
		    StartNextLevel();
	    }
		public bool AllArranged() => _allArranged;

        public bool CheckTaskComplete()
        {
            return _complete;
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