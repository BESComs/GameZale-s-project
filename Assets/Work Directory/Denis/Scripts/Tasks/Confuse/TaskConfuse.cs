using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Confuse
{
	public class TaskConfuse : TaskParent, ITask, ILessonStatsObservable
	{
		private void Awake()
		{
			if (Counter != null)
				Counter.Setup(Levels.Length);
			_items = transform.GetComponentsInChildren<ConfuseItem>(true);
			maxBal = Levels.Length;
		}

		private void Start()
		{
			RegisterLessonStart();
			StartNextLevel();
		}

		public Counter Counter;
		[Space]
		public Fuses[] Levels;
		[Serializable]
		public class Fuses
		{
			public float Duration = 2f;
			public Transpose[] Fusing;
		}
		[Serializable]
		public class Transpose
		{
			public ConfuseItem First;
			public ConfuseItem Second;
		}
        
        private bool _complete;
		public bool IsComplete => _complete;
		
	    private int _activeLevelId = -1;
		private void StartNextLevel()
		{//
			_complete = false;
			foreach (var i in _items)
			{
				i.Selected = false;
				i.Enable(false);
			}
	
			Timeout.Set(this, 1f, () =>
			{
				if (_activeLevelId < Levels.Length - 1)
				{
					_activeLevelId++;
					foreach (var item in _items)
						if (item.Object != null)
							Look(item);
					Timeout.Set(this, 2, () =>
					{
						StartCoroutine(FuseCoroutine());
					});
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

		private void Look(ConfuseItem item)
		{
			item.Looked = true;
			item.Parented(false);
			var itemPosition = item.transform.localPosition;
			item.Anim.MoveAdd(item.LookMove, 0.5f);
			Timeout.Set(item, 1, () =>
			{
				item.Anim.Move(itemPosition, 0.5f);
				Timeout.Set(item, 0.5f, () =>
				{
					item.Looked = false;
					item.Parented(true);
				});
			});
		}
		
		IEnumerator FuseCoroutine()
		{
			yield return new WaitForSeconds(0.5f);

			foreach (var f in Levels[_activeLevelId].Fusing)
			{
				Vector3 firstPosition = f.First.transform.localPosition;
				Vector3 secondPosition = f.Second.transform.localPosition;
				Vector3 middlePosition = (firstPosition + secondPosition) / 2f;
				f.First.Anim.MoveArc(firstPosition, middlePosition + new Vector3(0, 0, -0.2f), secondPosition, Levels[_activeLevelId].Duration);
				f.Second.Anim.MoveArc(secondPosition, middlePosition + new Vector3(0, 0, 0.2f), firstPosition, Levels[_activeLevelId].Duration);
				yield return new WaitForSeconds(Levels[_activeLevelId].Duration);
			}

			foreach (var item in _items)
				item.Enable(true);
		}

		public void Selected(ConfuseItem item)
		{
			if (item.Looked) return;
			Look(item);
			if (item.Object == null)
			{
				RegisterAnswer(false);
				return;
			}
			item.Selected = true;
			CheckObjects();
		}

		private ConfuseItem[] _items;
	    private void CheckObjects()
	    {
		    if (_complete) return;
		    _complete = true;
		    if (_items.Any(i => i.Object != null && !i.Selected))
		    {
			    RegisterAnswer(false);
			    return;
		    }
		    RegisterAnswer(true);
		    if (Counter != null)
			    Counter.Plus();
		    Timeout.Set(this, 2f, StartNextLevel);
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
			throw new NotImplementedException();
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
