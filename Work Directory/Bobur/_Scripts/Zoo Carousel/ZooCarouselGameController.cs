using System;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Zoo_Carousel
{
	public class ZooCarouselGameController : MonoBehaviour, ITask
	{
		public static ZooCarouselGameController Instance;
		[SerializeField] private ParticleSystem _particleSystemPrefab;
		[SerializeField] private GameObject UIElements;
		[SerializeField] private ZooCarouselConstructor _constructor;

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
		void Start ()
		{
			UIElements.SetActive(true);
			_constructor.InitLevel();
		}
	
		// Update is called once per frame
		void Update () {
			
		}

		public void NextLevel()
		{
			_constructor.NextLevel();
		}

		public async Task PlayParticle(Vector3 pos)
		{
			ParticleSystem _particleSystem = Instantiate(_particleSystemPrefab).GetComponent<ParticleSystem>();
			_particleSystem.transform.SetParent(transform);
			_particleSystem.transform.position = pos;
			_particleSystem.Play();
			await Task.Delay(TimeSpan.FromSeconds(_particleSystem.main.duration + _particleSystem.main.startLifetime.constant));
			
			if (_particleSystem.isPlaying)
			{
				_particleSystem.Stop();
			}
			
			Destroy(_particleSystem.gameObject);
		}

		public ZooCarouselConstructor Constructor() => _constructor;
		
		public void TaskCompleted()
		{
			GameManager.GetInstance().CurrentGameFinished(this);
			UIElements.SetActive(false);
		}

		public bool CheckTaskComplete()
		{
			return _constructor.CheckTaskComplete();
		}
	}
}
