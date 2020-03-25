using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Num_With_MiniGames
{
	public class NumWMGGameController : MonoBehaviour, ITask
	{
		public static NumWMGGameController Instance;
		
		[Header("General Elements")]
		[SerializeField] private GameObject UIElements;
		[SerializeField] private GameObject clickablePrefab;
		[SerializeField] private float _animationSpeed;
		[SerializeField] private float _delayBetweenAnims;
		[SerializeField] private Button _nextButton;
		[SerializeField] private Button _replayButton;
		[SerializeField] private TextMeshPro textObject;
		
		[Header("Particles")]
		[SerializeField] private ParticleSystem correctAnswerParticlePrefab;
		[SerializeField] private GameObject correctAnswerParticlesParent;
		[SerializeField] private ParticleSystem endGameParticle;
		
		[Header("Game Constructor")]
		[SerializeField] private NumWMGConstructor _constructor;
		
		private int currentScene;
		
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				Destroy(Instance);
				Instance = this;
			}
		}

		// Use this for initialization
		void Start () {
			UIElements.SetActive(true);
			SetNextButtonEnabled(false);
			currentScene = 0;
			_replayButton.gameObject.SetActive(false);
			
			_constructor.InitCurrent();
			_nextButton.onClick.AddListener(() => NextScene());
			_replayButton.onClick.AddListener(() => Replay());
		}

		private void OnValidate()
		{
			_constructor.MiniGame?.OnValidate();
			_constructor.FindObjects?.OnValidate();
		}

		public GameObject Create(GameObject ob) => Instantiate(ob);

		public float AnimSpeed => _animationSpeed;
		public float DelayBetweenAnims => _delayBetweenAnims;
		public int CurrentScene => currentScene;
		public TextMeshPro TextObject => textObject;
		public GameObject ClickablePrefab => clickablePrefab;

		public ParticleSystem CorrectAnswerParticle => correctAnswerParticlePrefab;
		public ParticleSystem EndGameParticle => endGameParticle;

		public void SetNextButtonEnabled(bool value) => _nextButton.interactable = value;

		public async void NextScene()
		{
			SetNextButtonEnabled(false);
			await _constructor.FinishCurrent();
			await Awaiters.Seconds(_delayBetweenAnims * 2);
			
			currentScene++;
			_constructor.InitCurrent();
			
			if (CheckTaskComplete())
			{
				_nextButton.gameObject.SetActive(false);
				_replayButton.gameObject.SetActive(true);
			}
		}

		public void Replay()
		{
			_constructor.FinishCurrent();
			currentScene = 0;
			_constructor.InitCurrent();
			_replayButton.gameObject.SetActive(false);
			_nextButton.gameObject.SetActive(true);
		}
		
		public async Task PlayCorrectAnswerParticle(GameObject ob, Vector3? size = null)
		{
			ParticleSystem particle = Instantiate(correctAnswerParticlePrefab);
			particle.transform.SetParent(correctAnswerParticlesParent.transform);
			
			Vector3 particleSystemSize = ob.transform.localScale;

			if (size != null)
			{
				particleSystemSize = size.Value;
			}
			else if (ob.GetComponent<SpriteRenderer>() != null)
			{
				particleSystemSize = ob.GetComponent<SpriteRenderer>().size * ob.transform.localScale;
			}

			var shape = particle.shape;
			shape.scale = particleSystemSize;
			
			particle.transform.position = ob.transform.position;
			
			particle.Play();
			await Awaiters.Seconds(particle.main.duration +
			                       particle.main.startLifetime.constant);
		}

		public void StopCorrectAnswerParticle()
		{
			foreach (Transform particle in correctAnswerParticlesParent.transform)
			{
				Destroy(particle.gameObject);
			}
		}

		public void PlayEndGameParticle() => EndGameParticle.Play();

		public void StopEndGameParticle()
		{
			EndGameParticle.Stop();
			EndGameParticle.Clear();
		}

		public void TaskCompleted()
		{
			UIElements.SetActive(false);
		}

		public bool CheckTaskComplete() => currentScene == 3;
	}
}
