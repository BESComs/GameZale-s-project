using UnityEngine;
using Work_Directory.Denis.Scenes.Pazzle;

namespace Work_Directory.Bobur._Scripts.JigsawPuzzle
{
	public class JigsawPuzzleGameController : MonoBehaviour, ITask ,ILessonStatsObservable
	{
	
		public static JigsawPuzzleGameController Instance { get; private set; }
		private int score;
		private int CurrentPuzzle { get; set; }
		[SerializeField] private GameObject jigsawCanvas;
		[SerializeField] private AnimatorOverrideController shapeAnimatorOverrideController;
		[SerializeField] private ParticleSystem _particleSystem;
	
		private GameObject[] puzzles;
	
		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		
			puzzles = new GameObject[transform.childCount - 2];
			CurrentPuzzle = 0;
		}

		private void Start()
		{
			//Store all child puzzles into array
			for (int i = 0; i < puzzles.Length; i++)
			{
				puzzles[i] = transform.GetChild(i).gameObject;
			}
			score = 0;
		}

		public GameObject GetCurrentPuzzle()
		{
			return puzzles[CurrentPuzzle];
		}
		
		/*public void NextPuzzle()
		{
			puzzles[CurrentPuzzle++].SetActive(false);
			if(CheckTaskComplete())
			{
				Debug.Log("All puzzles finished.");
				TaskCompleted();
				return;
			}
			puzzles[CurrentPuzzle].SetActive(true);
		}*/

		public void Repeat() => puzzles[CurrentPuzzle].GetComponent<Puzzle>().Init();

		public void IncrementScore()
		{
			score++;
		}

		public GameObject GetContinueButton()
		{
			return jigsawCanvas.transform.Find("Repeat Button").gameObject;
		}

		public void TaskCompleted()
		{
			GameManager.GetInstance().CurrentGameFinished(this);
			jigsawCanvas.SetActive(false);
			Instance.gameObject.SetActive(false);
		}

		public bool CheckTaskComplete()
		{
			return CurrentPuzzle >= puzzles.Length;
		}

		public AnimatorOverrideController getShapeAniamtor()
		{
			return shapeAnimatorOverrideController;
		}

		public void StartGame(GameObject puzzleIntroParent)
		{
			puzzleIntroParent.SetActive(false);
			if(puzzles[CurrentPuzzle] != null)
				puzzles[CurrentPuzzle].SetActive(true);
			
		}

		public ParticleSystem ParticleSystem => _particleSystem;

		public void PlayParticleSystem(bool i)
		{
			if (i){
				_particleSystem.Play();
				return;
			}
			_particleSystem.Stop();
			_particleSystem.Clear();
		
		}

		public int MaxScore { get => 1; set { } }
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
