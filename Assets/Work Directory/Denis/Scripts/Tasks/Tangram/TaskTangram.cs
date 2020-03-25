using System.Linq;
using Tasks;
using Tasks.Tangram;
using UnityEngine;

namespace Work_Directory.Denis.Scripts.Tasks.Tangram
{
	public class TaskTangram : TaskParent, ITask,ILessonStatsObservable
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
		public bool LastLevelKeeping = false;//

		private bool _complete = false;
	    private LevelInTask[] _levels;
		
	    private int _activeLevelId = -1;
		private ILevelEvent[] _activeLevelEvents;//
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
					_forCheck = _levels[_activeLevelId].GetComponentsInChildren<TangramPiece>(true);
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

		private TangramPiece[] _forCheck;
	    public void CheckTangramPieces()
	    {
		    if (_forCheck.Any(piece => piece.requiredField && !piece.IsPaired))
		        return;
		    
			RegisterAnswer(true);
			
		    if (Counter != null)
			    Counter.Plus();
		    StartNextLevel();
	    }
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


		public void RegisterLessonEnd(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
			ServerRequests.PostStatistics();
		}
		public void OnApplicationPause(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
		}
	    
	}
}