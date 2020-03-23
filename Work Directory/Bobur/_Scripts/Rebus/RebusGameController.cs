using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Scripts.Utility;
using Work_Directory;

namespace _Scripts.Rebus
{
	public class RebusGameController : MonoBehaviour, ITask ,ILessonStatsObservable
	{
		public static RebusGameController Instance { get; private set; }
		[SerializeField] private string TitleText;
		[SerializeField] private TextMeshProUGUI TitleTextObject;
		[SerializeField] private TextMeshProUGUI HintTextObject;
		[SerializeField] private Button HintButton;
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private Image RebusImagePlaceholder;
		[SerializeField] private Image incorrectImage;
		[SerializeField] private GameObject UIElements;
		[SerializeField] private SpriteAndText[] Questions;
		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private GameObject RepeatButton;
		[SerializeField] private GameObject EndGameImage;
		

		private int currentLevel;
		private ParticleSystem _particleSystem2;
		private bool animationPlaying;
		
		// Use this for initialization
		void Start ()
		{
			if (Instance != null && Instance != this){
				Destroy(Instance);
				Instance = this;
			}
			else if (Instance == null)
				Instance = this;
			
			UIElements.SetActive(true);
			
			Init();
		}

		private void Init()
		{
			RepeatButton.SetActive(false);
			
			animationPlaying = false;
			TitleTextObject.text = TitleText;
			HintTextObject.color = new Color(1f, 1f, 1f, 0f);
			incorrectImage.color = new Color(1f, 1f, 1f, 0f);

			HintButton.onClick.AddListener(() => { StartCoroutine(ShowHint()); });
			_particleSystem2 = Instantiate(_particleSystem);
			_particleSystem2.transform.localPosition = new Vector3(_particleSystem.transform.localPosition.x * -1,
				_particleSystem.transform.localPosition.y);
			_particleSystem2.transform.SetParent(transform);
			
			UIElements.SetActive(false);
		}

		public void StartGame(GameObject introParent)
		{
			UIElements.SetActive(true);
			UIElements.transform.localScale = Vector3.one;
			introParent.SetActive(false);
			EndGameImage.SetActive(false);
			currentLevel = -1;
			RegisterLessonStart();
			NextLevel();
		}

		private void NextLevel()
		{
			currentLevel++;
			inputField.text = "";
			HintTextObject.color = new Color(1f, 1f, 1f, 0f);
			incorrectImage.color = new Color(1f, 1f, 1f, 0f);
			
			StopAllCoroutines();
			
			if (CheckTaskComplete())
			{
				TaskCompleted();
				return;
			}
			
			RebusImagePlaceholder.sprite = Questions[currentLevel].sprite;
			HintTextObject.text = Questions[currentLevel].text.ToUpper();
		}
		
		public async void CheckAnswer()
		{
			if(animationPlaying) return;
			
			if (string.Equals(inputField.text, Questions[currentLevel].text.Trim(), StringComparison.CurrentCultureIgnoreCase))
			{
				animationPlaying = true;
				_particleSystem.Play();
				_particleSystem2.Play();
				RegisterAnswer(true);
				
				await Task.Delay(TimeSpan.FromSeconds(_particleSystem2.main.duration + _particleSystem2.main.startLifetime.constant));
				animationPlaying = false;
				if(this != null)
					NextLevel();
				
				return;
			}
			RegisterAnswer(false);
			StartCoroutine(ShowIncorrect());
		}

		private IEnumerator ShowHint()
		{
			if (!animationPlaying)
			{
				while (HintTextObject.color.a < 1f)
				{
					HintTextObject.color = new Color(1f, 1f, 1f, HintTextObject.color.a + Time.deltaTime);
					yield return null;
				}
	
				yield return new WaitForSeconds(1);
				
				while (HintTextObject.color.a > 0)
				{
					HintTextObject.color = new Color(1f, 1f, 1f, HintTextObject.color.a - Time.deltaTime);
					yield return null;
				}
			}
		}
		
		private IEnumerator ShowIncorrect()
		{
			while (incorrectImage.color.a < 1f)
			{
				incorrectImage.color = new Color(1f, 1f, 1f, incorrectImage.color.a + Time.deltaTime);
				yield return null;
			}

			yield return new WaitForSeconds(1);
			
			while (incorrectImage.color.a > 0)
			{
				incorrectImage.color = new Color(1f, 1f, 1f, incorrectImage.color.a - Time.deltaTime);
				yield return null;
			}
		}

		public async void TaskCompleted()
		{			
			RegisterLessonEnd();
			await AnimationUtility.ScaleOut(UIElements);
			EndGameImage.SetActive(true);
			AnimationUtility.ScaleIn(EndGameImage);
			
			await Task.Delay(TimeSpan.FromSeconds(0.4f));
			
			RepeatButton.SetActive(true);
			AnimationUtility.ScaleIn(RepeatButton);
		}

		public bool CheckTaskComplete()
		{
			return Questions.Length == currentLevel;
		}

		public int MaxScore
		{
			get => Questions.Length;  
			set{}
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
	
	[Serializable]
	public struct SpriteAndText
	{
		public Sprite sprite;
		public string text;
	}
}
