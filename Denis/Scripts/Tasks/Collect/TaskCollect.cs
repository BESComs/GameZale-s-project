using System.Linq;
using Tasks.Select;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis;
using Work_Directory.Denis.Scripts;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Collect
{
	public class TaskCollect : TaskParent, ITask,ILessonStatsObservable
	{

		private GameObject _message1;
		private GameObject _message2;
		private void Awake()
		{
			_message1 = transform.GetChild(0).gameObject;
			_message2 = transform.GetChild(1).gameObject;
			_message1.SetActive(false);
			_message2.SetActive(false);
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
		
		public Counter Counter;
		[Space]//
		public float LevelEndDelay = 1f;//
		public bool LastLevelKeeping = false;//
        
        private bool _complete = false;
	    private LevelInTask[] _levels;
	    private bool _inTask;
	    private int _activeLevelId = -1;
		private ILevelEvent[] _activeLevelEvents;//
		private async void StartNextLevel()
		{//
			_inTask = true;
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
					_activeLevelId++;
					_levels[_activeLevelId].gameObject.SetActive(true);
					_activeLevelEvents = _levels[_activeLevelId].GetComponentsInChildren<ILevelEvent>(true);
					foreach (var levent in _activeLevelEvents)
						levent.OnLevelStart();
					_forCheck = _levels[_activeLevelId].GetComponentsInChildren<CollectItem>(true);
				}
				else
				{
					_complete = true;
					Timeout.Set(this, 1, LoadFinalScene);
					return;
				}
			});
			await new WaitForSeconds(LevelEndDelay + 1);
			_inTask = false;
		}//

		private CollectItem[] _forCheck;
	    public async void CheckObjects()
	    {
		    if(_message2.activeSelf ||  _message1.activeSelf || _inTask) return;
		    if (!_forCheck.All(obj => obj.Required && obj.Success || !obj.Required && !obj.Success))
		    {
			    if(_message1.activeSelf) return;
			    RegisterAnswer(false);
			    _message1.SetActive(true);
			    await new Fade(_message1.transform, Mode.In).RunTask();
			    await new WaitForSeconds(1);
			    await new Fade(_message1.transform).RunTask();
			    _message1.SetActive(false);
			    return;
		    }
		    RegisterAnswer(true);
		    _message2.SetActive(true);
		    await new Fade(_message2.transform, Mode.In).RunTask();
		    await new WaitForSeconds(1);
		    await new Fade(_message2.transform).RunTask();
		    _message2.SetActive(false);
		    if (Counter != null)
			    Counter.Plus();
		    RegisterLessonEnd();
		    StartNextLevel();
	    }

		public void Reset()
		{
			foreach (var obj in _forCheck)
				if (obj.Success)
					obj.OnMouseDown();
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
