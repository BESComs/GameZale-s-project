using System;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace _Scripts.Sudoku
{
	public class SudokuGameController : MonoBehaviour, ITask
	{
		public static SudokuGameController Instance { get; private set; }
		[SerializeField] private Sprite[] typeSprites;
		[SerializeField] private GameObject cellPrefab;
		[SerializeField] private GameObject boardPrefab;
		[SerializeField] private Level[] levels;
		[SerializeField] private ParticleSystem _particleSystem;

		private int currentLevel;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		// Use this for initialization
		void Start ()
		{
			currentLevel = 0;
			Initialize();
		}

		private void Initialize()
		{
			for (int i = 0; i < levels.Length; i++)
			{
				levels[i].Init(this, i);
				if(i > 0)
					levels[i].DisableLevel();
			}
		}

		public async Task PlayParticle()
		{
			_particleSystem.Play();
			_particleSystem.GetComponent<ParticleMoveSpiral>()?.RunParticle();
			await Task.Delay(TimeSpan.FromSeconds(_particleSystem.main.duration));
			_particleSystem.Stop();
			_particleSystem.Clear();
			_particleSystem.transform.position = Vector3.zero;
		}

		public void CurrentLevelCompleted()
		{
			currentLevel++;
			if (CheckTaskComplete())
			{
				TaskCompleted();
			}
			else
			{
				NextLevel();
			}
		}
		
		private void NextLevel()
		{
			levels[currentLevel - 1].DisableLevel();
			levels[currentLevel].EnableLevel();
		}
		
		public GameObject Create(GameObject ob) => Instantiate(ob);

		public Sprite[] TypeSprites => typeSprites;
		public GameObject CellPrefab => cellPrefab;
		public GameObject BoardPrefab => boardPrefab;
		public Level CurrentLevel => levels[currentLevel];
		public void TaskCompleted()
		{
			throw new NotImplementedException();
		}

		public bool CheckTaskComplete()
		{
			return currentLevel >= levels.Length;
		}
	}
}
