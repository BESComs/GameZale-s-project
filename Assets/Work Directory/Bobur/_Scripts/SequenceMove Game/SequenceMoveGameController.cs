using UnityEngine;
using _Scripts.Utility;
using Work_Directory;

namespace _Scripts.SequenceMove_Game
{
	public class SequenceMoveGameController : MonoBehaviour, ITask,ILessonStatsObservable
	{
		public static SequenceMoveGameController Instance { get; private set; }
		[SerializeField] private GameObject RepeatParent;
		[SerializeField] private SequenceMoveConstructorScript[] SequenceMoveGames;
		private int currentLevel;
		
		void Start ()
		{
			if (Instance != null ){
				if (Instance != this)
				{
					Destroy(Instance);
					Instance = this;
				}
			}
			else
			{
				Instance = this;
			}
		}

		public void StartGame(GameObject introParent)
		{
			introParent.SetActive(false);
			Init();
		}

		public async void RepeatGame()
		{
			await AnimationUtility.ScaleOut(RepeatParent);
			RepeatParent.SetActive(false);
			Init();
		}

		private void Init()
		{
			currentLevel = -1;
			InitializeGame();
			RegisterLessonStart();
			NextLevel();
		}
		
		public GameObject CreateInstance(GameObject ob)
		{
			return Instantiate(ob);
		}

		public SequenceMoveConstructorScript GetCurrentLevel()
		{
			return SequenceMoveGames[currentLevel];
		}

		private void InitializeGame()
		{
			for (int i = 0; i < SequenceMoveGames.Length; i++)
			{
				SequenceMoveGames[i].Initialize();
				SequenceMoveGames[i].ParentGameObject.name = "Level " + (i + 1);
				SequenceMoveGames[i].ParentGameObject.transform.SetParent(transform);
				SequenceMoveGames[i].ParentGameObject.SetActive(false);
			}
		}

		public void NextLevel()
		{
			currentLevel++;
			if (currentLevel == SequenceMoveGames.Length)
				RegisterLessonEnd();
			if (CheckTaskComplete())
			{
				TaskCompleted();
				return;
			}

			

			SequenceMoveGames[currentLevel].ParentGameObject.SetActive(true);
			foreach (Transform obj in SequenceMoveGames[currentLevel].ParentGameObject.transform)
			{
				AnimationUtility.ScaleIn(obj.gameObject, maxScale: obj.localScale.x, initialScale:0.01f);
			}
		}

		public void TaskCompleted()
		{
			//GameManager.GetInstance().CurrentGameFinished(this);
			RepeatParent.SetActive(true);
			AnimationUtility.ScaleIn(RepeatParent);
		}

		public bool CheckTaskComplete()
		{
			return SequenceMoveGames.Length == currentLevel;
		}

		public int MaxScore
		{
			get => 1; set{} 
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
