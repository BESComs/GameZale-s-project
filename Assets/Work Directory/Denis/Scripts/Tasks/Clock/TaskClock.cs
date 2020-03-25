using Tasks;
using Tasks.Clock;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Work_Directory.Denis.Scripts.New_Folder;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts.Tasks.Clock
{
	public class TaskClock : TaskParent, ITask,ILessonStatsObservable
	{
		public static TaskClock Instance;
		private void OnEnable()
		{
			Instance = this;
		}
		
		public enum ChangeType { Hours, Minutes }
		
		public Counter Counter;
		public GameObject Complete;
		public Message Message;
		public ClockBird Bird;
		public List<Transform> Daytime;
		public Vector2Int[] Levels;
		public float LevelEndDelay = 1f;
		[Space]
		public TextMesh ChangeTimeHours;
		public TextMesh ChangeTimeMinutes;
		public Transform ShowTimeHours;
		public Transform ShowTimeMinutes;
		private Animate _showHoursAnim;
		private Animate _showMinsAnim;
		private float _lastshowh;
		private bool _complete;
		private ClockChange[] _clockButtons;
		private int _activeClockId = -1;
		private void Awake()
		{
			maxBal = Levels.Length;
			if (Complete != null)
				Complete.SetActive(false);
			Counter.Setup(Levels.Length);
			_clockButtons = GetComponentsInChildren<ClockChange>(true);
			foreach (var btn in _clockButtons)
				btn.gameObject.SetActive(false);
			_showHoursAnim = new Animate(this, ShowTimeHours);
			_showMinsAnim = new Animate(this, ShowTimeMinutes);
		}

		private void Start()
		{
			RegisterLessonStart();
			StartNextClock();
		}

		private void SetChangeTime(Vector2Int set)
		{
			var hours = set.x.ToString();
			if (hours.Length == 1)
				hours = "0" + hours;
			ChangeTimeHours.text = hours;
			var minutes = set.y.ToString();
			if (minutes.Length == 1)
				minutes = "0" + minutes;
			ChangeTimeMinutes.text = minutes;
		}


		private void SetShowTime(Vector2Int set)
		{
			set.x = set.x % 12;
			
			var m = 6f * set.y;
			Timeout.Set(this, 0.5f, () =>
			{
				_showMinsAnim.Rotate(-m, 1f);
			});
			var h = 30f * set.x + set.y / 2f;
			_showHoursAnim.Rotate(-h, 0.5f);
			
		}

		private async void SetDaytime(float hours)
		{
			Transform tmpTransform = null, tmpTransform2;
			foreach (var transform1 in Daytime)
			{
				if (!transform1.gameObject.activeSelf) continue;
				tmpTransform = transform1;
				break;
			}
			if (hours >= 0 && hours < 6)
			{
				tmpTransform2 = Daytime[0];
			}
			else if( hours < 12)
			{
				tmpTransform2 = Daytime[1];
			}
			else if(hours < 18)
			{
				tmpTransform2 = Daytime[2];
			}
			else
			{
				tmpTransform2 = Daytime[3];
			}

			if (tmpTransform == tmpTransform2) return;
			tmpTransform2.gameObject.SetActive(true);
			new Fade(tmpTransform2, Mode.In, AnimationCurve.EaseInOut(0, 0, 0.5f, 1)).RunTask();
			if (!tmpTransform) return;
			await new Fade(tmpTransform, Mode.Out, AnimationCurve.EaseInOut(0, 0, 0.5f, 1)).RunTask();
			tmpTransform.gameObject.SetActive(false);
		}
        
		private  void StartNextClock()
		{
			Timeout.Set(this, LevelEndDelay, () =>
			{
				if (_activeClockId < Levels.Length - 1)
				{
					_activeClockId++;
					Levels[_activeClockId] = GetCorrectTime(Levels[_activeClockId]);
					SetShowTime(Levels[_activeClockId]);
					SetDaytime(Levels[_activeClockId].x);
					Timeout.Set(this, 1.5f, () =>
					{
						foreach (var btn in _clockButtons)
							btn.gameObject.SetActive(true);
					});
				}
				else
				{
					if (Complete != null)
						Complete.SetActive(true);
					_complete = true;
					LoadFinalScene();
					return;
				}
				
			});
		}
		
		private Vector2Int _changedClock = new Vector2Int(0,0);
		public void ChangeClock (ChangeType type, int add)
		{
			if (type == ChangeType.Hours)
				_changedClock.x += add;
			else
				_changedClock.y += add;
			_changedClock = GetCorrectTime(_changedClock);
			SetChangeTime(_changedClock);
		}

		public void CheckClock()
		{
			if (_changedClock != Levels[_activeClockId])
				return;
				
			RegisterAnswer(true);
			foreach (var btn in _clockButtons)
				btn.gameObject.SetActive(false);
			Bird.Tweet();
			Timeout.Set(this, 1, () =>
			{
				RegisterLessonEnd();
				Counter.Plus();
				_changedClock = new Vector2Int(0, 0);
				SetChangeTime(_changedClock);
				StartNextClock();
			});
		}

		private Vector2Int GetCorrectTime(Vector2Int input)
		{
			if (input.x < 0)
				input.x = Mathf.CeilToInt(-input.x / 24f) * 24 + input.x;
			else
				input.x = input.x % 24;
			
			if (input.y < 0)
				input.y = Mathf.CeilToInt(-input.y / 60f) * 60 + input.y;
			else
				input.y = input.y % 60;
			return input;
		}

		public bool CheckTaskComplete()
		{
			Message.Show("Отлично");
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
		

		public void RegisterLessonEnd(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
			ServerRequests.PostStatistics();
		}
		public void OnApplicationPause(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
		}
	}
}
