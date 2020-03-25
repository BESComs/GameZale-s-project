using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityAsync;
using UnityEngine;
using _Scripts.Color_Mix;
using _Scripts.Utility;

namespace Work_Directory.Bobur._Scripts.Color_Mix
{
	public class ColorMixGameController : MonoBehaviour, ITask, ILessonStatsObservable
	{

		public static ColorMixGameController Instance { get; private set; }
		[SerializeField] private GameObject pipettePrefab;
		[SerializeField] private ColorMixer _colorMixer;
		[SerializeField] private ColorsEnum[] colorsToMix;
		[SerializeField] private SpriteRenderer wrongAnswerSprite;
		[SerializeField] private GameObject UIElements;
		[SerializeField] private TextMeshPro QuestionText;
		[SerializeField] private GameObject EndGameObjects;

		private ColorMixPointer pipette;
		public bool isAnimating { get; set; }
		private int currentColorIndex;
		private bool _isPlaying;
		
		// Use this for initialization
		void Start ()
		{
			isAnimating = false;
			wrongAnswerSprite.color = new Color(1f, 1f, 1f, 0);
			
			UIElements.SetActive(true);
			
			if (Instance != null && Instance != this)
			{
				Destroy(Instance);
				Instance = this;
			}
			else if (Instance == null)
			{
				Instance = this;
			}
		}

		private void Update()
		{
			SetPipetteToMousePosition();
		}

		public void StartGame(GameObject introObject)
		{
			transform.GetChild(0).gameObject.SetActive(true);
			Init();
			introObject.SetActive(false);
		}

		public void Replay() => Init();

		private void Init()
		{
			RegisterLessonStart();
			EndGameObjects.SetActive(false);
			_isPlaying = true;
			Cursor.visible = false;
			pipette = Instantiate(pipettePrefab).GetComponent<ColorMixPointer>();
			pipette.transform.SetParent(transform);
			SetPipetteToMousePosition();
			currentColorIndex = 0;
			SetQuestionText();
		}

		public ColorsEnum GetCurrentColorToMix() => colorsToMix[currentColorIndex];

		public void NextColor()
		{
			currentColorIndex++;
			if (CheckTaskComplete())
			{
				TaskCompleted();
				return;
			}
			//RegisterLessonStart();
			SetQuestionText();
		}
		
		public ColorMixer GetColorMixer() => _colorMixer;
		
		public ColorMixPointer GetPipette() => pipette;
		
		public void SetPipetteToMousePosition()
		{
			if (!_isPlaying) return;
			
			Vector3 position = Input.mousePosition;
			position.z = 1;
			pipette.SetPosition(Camera.main.ScreenToWorldPoint(position));
		}
		
		public async Task WrongAnswerEffect()
		{
			for (float i = 0; i < 1; i += Time.deltaTime)
			{	
				i = (i > 1f) ? 1f : i;
				Color color = new Color(1f, 1f, 1f, i);
				wrongAnswerSprite.color = color;
				await Await.NextUpdate();
			}

			await Task.Delay(TimeSpan.FromSeconds(0.5f));
			
			for (float i = 1; i > 0; i -= Time.deltaTime)
			{	
				i = (i < 0) ? 0 : i;
				Color color = new Color(1f, 1f, 1f, i);
				wrongAnswerSprite.color = color;
				await Await.NextUpdate();
			}
		}

		private void SetQuestionText()
		{
			QuestionText.text = $"{Colors.GetColorName(colorsToMix[currentColorIndex])} цвет";
			QuestionText.color = Colors.EnumToColor(colorsToMix[currentColorIndex]);
		}
		
		public async void TaskCompleted()
		{
			RegisterLessonEnd();
			_isPlaying = false;
			Cursor.visible = true;
			Destroy(pipette.gameObject);
			EndGameObjects.SetActive(true);
			AnimationUtility.ScaleIn(EndGameObjects);
		}

		private void OnDestroy()
		{
			Cursor.visible = true;
		}

		public bool CheckTaskComplete() => currentColorIndex >= colorsToMix.Length;


		public int MaxScore
		{
			get => colorsToMix.Length;
			set {} 
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
}[Serializable]
public class Weather
{
	public int id;
	public string main;
}
[Serializable]
public class WeatherInfo
{
	public int id;
	public string name;
	public List<Weather> weather;
}