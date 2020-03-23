using System.Collections.Generic;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.PaintPro
{
	public class TaskPaintPro : TaskParent, ITask,ILessonStatsObservable
	{
		private readonly Stack<PaintStep> _prev = new Stack<PaintStep>();
		private readonly Stack<PaintStep> _next = new Stack<PaintStep>();
		[HideInInspector] public PaintProPalette activePalette;
		private PaintProColor _color;
		public PaintProStep prevStep;
		public PaintProStep nextStep;
		[SerializeField] private SpriteRenderer indicator;
		private int _currentIndex;
		private void Awake()
		{
			if (indicator != null)
				indicator.color = _color.color;
			maxBal = 1;
		}

		private void Start()
		{
			nextStep.SetActive(false);
			prevStep.SetActive(false);
			RegisterLessonStart();
		}
		public void Set(PaintProColor color)
		{
			_color = color;
			if (indicator != null)
				indicator.color = _color.color;
		}
		public Color Get()
		{
			return (_color == null) ? Color.white : _color.color;
		}
		
		private struct PaintStep
		{
			public PaintProPiece Piece;
			public Color From;
			public Color To;

			public PaintStep(PaintProPiece piece, Color from, Color to)
			{
				Piece = piece;
				From = from;
				To = to;
			}
		}
		public void Step(PaintProPiece piece, Color from)
		{
			_next.Clear();
           
			if (Get() == from) return;

			_currentIndex++;
			_prev.Push(new PaintStep(piece, from, Get()));

			nextStep.SetActive(false);
			prevStep.SetActive(true);
		}
		public void Prev()
		{
			var step  = _prev.Pop();
			step.Piece.Colorize(step.From);
			_next.Push(step);
			
			if (_prev.Count == 0)
				prevStep.SetActive(false);
			nextStep.SetActive(true);
		}
		public void Next()
		{	
			var step  = _next.Pop();
			step.Piece.Colorize(step.To);
			_prev.Push(step);
			
			if (_next.Count == 0)
				nextStep.SetActive(false);
			prevStep.SetActive(true);
		}

		public bool CheckTaskComplete()
		{
			return true;
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

	public enum StepType
	{
		Undo, Redo
	}
}