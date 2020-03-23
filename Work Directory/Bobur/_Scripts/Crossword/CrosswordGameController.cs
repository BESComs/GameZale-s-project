using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _Scripts.Utility;
using Work_Directory;

namespace _Scripts.Crossword
{
	public class CrosswordGameController : MonoBehaviour, ITask,ILessonStatsObservable
	{

		public static CrosswordGameController Instance { get; private set; }

		[Header("Prefabs")]
		[SerializeField] private LetterObject _letterPrefab;

		[Header("Lines")]
		[SerializeField] private GameObject line1;
		[SerializeField] private GameObject line2;

		[Header("Others")]
		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private GameObject UIElements;
		[SerializeField] private GameObject LettersParent;
		[SerializeField] private GameObject EndGameUI;
		[SerializeField] private GameObject LetterBoxesParent;
		[SerializeField] private float animSpeed;
		
		private HashSet<LetterObject> _lettersToFind = new HashSet<LetterObject>();
		private int lettersFound;
		

		private void Awake()
		{
			if (Instance == null) {
				Instance = this;
			}
			else if (Instance != this) {
				Destroy(Instance);
				Instance = this;
			}
		}

		// Use this for initialization
		void Start () {
			InitLetters();
			
			lettersFound = 0;
			
			UIElements.SetActive(true);
			
			LettersParent.transform.localScale = Vector3.one;
			EndGameUI.transform.localScale = Vector3.zero;
		}

		public void StartGame(GameObject playButtonParent)
		{
			gameObject.SetActive(true);
			RegisterLessonStart();
			playButtonParent.SetActive(false);
		}
		
		public void Restart()
		{
			LettersParent.transform.localScale = Vector3.one;
			EndGameUI.transform.localScale = Vector3.zero;
			lettersFound = 0;
			
			_particleSystem.Stop();
			_particleSystem.Clear();
			
			foreach (LetterObject letterObject in _lettersToFind)
			{
				letterObject.Init();
			}
			
			RegisterLessonStart();
			InitLetters();
		}

		private void InitLetters()
		{
			char[] letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ".ToCharArray();
			
			for (int i = 0; i < letters.Length; i++)
			{
				LetterObject ob = Instantiate(_letterPrefab).GetComponent<LetterObject>();
				ob.Letter = letters[i].ToString();
				
				ob.transform.SetParent((i < 17) ? line1.transform : line2.transform);
			}
		}

		public void AddLetterToFind(LetterObject letter)
		{
			if(_lettersToFind.Contains(letter))
				return;
			_lettersToFind.Add(letter);
		}
		
		public async void LetterFound()
		{
			lettersFound++;
			if (CheckTaskComplete())
			{
				RegisterLessonEnd();
				RegisterAnswer(true);
				_particleSystem.Play();
				await AnimationUtility.ScaleOut(LettersParent, animSpeed);
				await Task.Delay(TimeSpan.FromSeconds(1f));
				await AnimationUtility.ScaleIn(EndGameUI, animSpeed);
				DeleteLetters();
			}
		}

		private void DeleteLetters()
		{
			foreach (Transform child in line1.transform)
			{
				Destroy(child.gameObject);
			}

			foreach (Transform child in line2.transform)
			{
				Destroy(child.gameObject);
			}
		}

		public Vector2 LetterBoxColliderSize() => _letterPrefab.GetComponent<BoxCollider2D>().size;

		public void TaskCompleted()
		{
			UIElements.SetActive(false);
		}

		public bool CheckTaskComplete()
		{
			return _lettersToFind.Count == lettersFound;
		}

		public int MaxScore
		{
			get => 1; set { } }

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
