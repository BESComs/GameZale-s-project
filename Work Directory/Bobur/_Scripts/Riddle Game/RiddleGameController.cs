using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityAsync;
using _Scripts.Utility;

namespace _Scripts.Riddle_Game
{
	public class RiddleGameController : MonoBehaviour, ITask
	{
		[Serializable]
		enum AnimType
		{
			Fade,
			Scale
		}
		
		[SerializeField] private GameObject UIElements;
		[SerializeField] private Text QuestionText;
		[SerializeField] private Text AnswerText;
		[SerializeField] private Image QuestionImage;
		[SerializeField] private bool IsOptionsStatic;
		[SerializeField] private AnimType _animType;
		[SerializeField] private float animDuration;
		[SerializeField] private float delayBetweenAnims;
		[SerializeField] private float optionsScale; // TODO this is temporary solution. Remove in the future
		[SerializeField] private ParticleSystem _particleSystem;

		[SerializeField] private Riddle[] _riddles;
		[SerializeField] private Position2D[] optionPositions;

		private List<OptionObject> OptionObjects;
		private List<object> FirstGroupToAnimate = new List<object>();
		private List<object> SecondGroupToAnimate = new List<object>();

		private bool isAnimating;
		private int currentRiddle;

		void Start () {
			currentRiddle = -1;
			OptionObjects = new List<OptionObject>();

			Initialize();
		}

		private async void Initialize()
		{
			FirstGroupToAnimate.Add(QuestionImage);
			FirstGroupToAnimate.Add(QuestionText);
			
			UIElements.SetActive(true);
			
			GameObject optionsParent = new GameObject("Option Positions");
			optionsParent.transform.SetParent(transform);
			
			for (int i = 0; i < optionPositions.Length; i++)
			{
				GameObject optionObject = new GameObject("Option Position " + (i + 1));
				optionObject.transform.SetParent(optionsParent.transform);
				optionObject.transform.position = new Vector3(optionPositions[i].X, optionPositions[i].Y) / optionsScale;
				
				OptionObject optionObjectComponent = optionObject.AddComponent<OptionObject>();
				optionObjectComponent.SetParent(this);
				
				SecondGroupToAnimate.Add(optionObject);
				OptionObjects.Add(optionObjectComponent);
			}
			optionsParent.transform.localScale = new Vector3(optionsScale, optionsScale);
			
			await Await.NextFixedUpdate();
			NextRiddle();
		}

		private async Task NextRiddle()
		{
			currentRiddle++;
			if (CheckTaskComplete())
			{
				TaskCompleted();
				return;
			}
			
			if (currentRiddle == 0)
			{
				if (IsOptionsStatic)
				{
					SetCurrentRiddle();
					SetCurrentRiddleText();
					await AnimIn(FirstGroupToAnimate);
				}
				else
				{
					SetCurrentRiddleText();
					await AnimIn(FirstGroupToAnimate);
					//await Task.Delay(TimeSpan.FromSeconds(0.3));
					SetCurrentRiddle();
					await AnimIn(SecondGroupToAnimate);
					RecalculateOptionsCollider();
				}
				
				return;
			}

			if (IsOptionsStatic)
			{
				AnimOut(FirstGroupToAnimate);
				await HideAnswer();
				SetCurrentRiddleText();
				SetCurrentRiddle();
				await AnimIn(FirstGroupToAnimate);
				return;
			}
			
			AnimOut(FirstGroupToAnimate);
			await HideAnswer();
			await AnimOut(SecondGroupToAnimate);
			SetCurrentRiddleText();
			SetCurrentRiddle();
			await Task.Delay(TimeSpan.FromSeconds(delayBetweenAnims));
			await AnimIn(FirstGroupToAnimate);
			await AnimIn(SecondGroupToAnimate);
			RecalculateOptionsCollider();
		}

		private async Task AnimIn(List<object> objects)
		{
			if (_animType == AnimType.Fade)
			{
				await FadeInObjects(objects);
			}
			else if(_animType == AnimType.Scale)
			{
				await ScaleInObjects(objects);
			}
		}

		private async Task AnimOut(List<object> objects)
		{
			if (_animType == AnimType.Fade)
			{
				await FadeOutObjects(objects);
			}
			else if(_animType == AnimType.Scale)
			{
				await ScaleOutObjects(objects);
			}
		}
		
		
		private async Task FadeInObjects(List<object> objects)
		{	
			float transparency = 0f;

			while (transparency < 1)
			{
				transparency += Time.deltaTime / animDuration;
				transparency = transparency > 1f ? 1f : transparency;
				foreach (object ob in objects)
				{
					AnimReferenceUtility.ChangeTransparency(ob, transparency);
				}
				await Await.NextUpdate();
			}
		}
		
		private async Task FadeOutObjects(List<object> objects)
		{
			float transparency = 1f;

			while (transparency > 0)
			{
				transparency -= Time.deltaTime / animDuration;
				transparency = transparency < 0f ? 0f : transparency;
				foreach (object ob in objects)
				{
					AnimReferenceUtility.ChangeTransparency(ob, transparency);
				}
				await Await.NextUpdate();
			}
		}
		
		private async Task ScaleInObjects(List<object> objects)
		{
			float scale = 0;

			while (scale < 1f)
			{
				scale += Time.deltaTime / animDuration;
				scale = scale > 1f ? 1f : scale;
				
				foreach (object ob in objects)
				{
					AnimReferenceUtility.ChangeScale(ob, scale);
				}
				await Await.NextUpdate();
			}
		}
		
		private async Task ScaleOutObjects(List<object> objects)
		{
			float scale = 1f;

			while (scale > 0)
			{
				scale -= Time.deltaTime / animDuration;
				scale = scale < 0f ? 0f : scale;
				foreach (object ob in objects)
				{
					AnimReferenceUtility.ChangeScale(ob, scale);
				}
				await Await.NextUpdate();
			}
		}

		private async Task ShowAnswer()
		{
			FirstGroupToAnimate.Remove(QuestionImage);
			AnimOut(new List<object> {QuestionImage});
			await Task.Delay(TimeSpan.FromSeconds(0.2f));
			AnswerText.gameObject.SetActive(true);
			await AnimIn(new List<object> {AnswerText});
		}
		
		private async Task HideAnswer()
		{
			await AnimOut(new List<object> {AnswerText});
			FirstGroupToAnimate.Add(QuestionImage);
		}

		private void SetCurrentRiddle()
		{
			if (IsOptionsStatic && currentRiddle > 0)
				return;
			
			for (int i = 0; i < OptionObjects.Count; i++)
			{
				OptionObjects[i].SetSprite(_riddles[currentRiddle].Options[i].GetSprite());
				OptionObjects[i].ID = _riddles[currentRiddle].Options[i].GetID();
			}
		}

		private void SetCurrentRiddleText()
		{
			QuestionText.text = _riddles[currentRiddle].Question;
			AnswerText.text = _riddles[currentRiddle].Answer;
		}

		public async Task ObjectClicked(int clickedID, Vector3 position)
		{
			if (isAnimating)
				return;
			
			if (clickedID == _riddles[currentRiddle].CorrectAnswer)
			{
				isAnimating = true;
				_particleSystem.transform.localPosition = position * optionsScale;
				_particleSystem.Play();
				await ShowAnswer();
				await Task.Delay(TimeSpan.FromSeconds(delayBetweenAnims));
				
				_particleSystem.Clear();
				await NextRiddle();
				isAnimating = false;
			}
		}

		private void RecalculateOptionsCollider()
		{
			foreach (OptionObject optionObject in OptionObjects)
			{
				optionObject.RecalculateBoxCollider();
			}
		}

		public void TaskCompleted()
		{
			UIElements.SetActive(false);
			GameManager.GetInstance().CurrentGameFinished(this);
		}

		public bool CheckTaskComplete() => _riddles.Length == currentRiddle;
	}
}