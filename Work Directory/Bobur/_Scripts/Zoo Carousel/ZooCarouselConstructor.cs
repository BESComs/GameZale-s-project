using System;
using DefaultNamespace;
using UnityAsync;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Zoo_Carousel
{
	[Serializable]
	public class ZooCarouselConstructor: ITask
	{
		[Header("General Elements")]
		[SerializeField] private Carousel _carousel;
		[SerializeField] private Animal[] Animals;

		[Header("Levels")]
		[SerializeField] private Level[] Levels;

		private ZooCarouselGameController _gameController => ZooCarouselGameController.Instance;
		private int currentLevel;
		
		public ZooCarouselConstructor()
		{
			currentLevel = 0;
		}

		public void InitLevel()
		{
			for (int i = 0; i < _carousel.GetPlaceholders().Length; i++)
			{
				_carousel.GetPlaceholders()[i].SetSprite(Levels[currentLevel].PlaceholderSprites[i]);
				Animals[i].SetSprite(Levels[currentLevel].AnimalSprites[i]);
				Animals[i].FadeIn();
			}
		}

		public Level CurrentLevel => Levels[currentLevel];

		public async void AnimalMatched(int id)
		{
			Animals[id].SetSprite(null);
			_carousel.GetPlaceholders()[id].SetSprite(CurrentLevel.CorrectAnswerSprites[id]);
			await _gameController.PlayParticle(_carousel.GetPlaceholders()[id].transform.position);
			CurrentLevel.IncrementScore();
		}

		public async void NextLevel()
		{
			currentLevel++;
			await _carousel.Spin();

			if (CheckTaskComplete())
			{
				TaskCompleted();
				return;
			}
			
			InitLevel();
		}
		
		
		
		public void TaskCompleted()
		{
			_gameController.TaskCompleted();
		}

		public bool CheckTaskComplete()
		{
			return currentLevel >= Levels.Length;
		}
	}

	[Serializable]
	public class Level : ITask
	{
		[SerializeField] private Sprite[] _animalSprites;
		[SerializeField] private Sprite[] _placeholderSprites;
		[SerializeField] private Sprite[] _correctAnswerSprites;

		private int score;

		public Level()
		{
			score = 0;
		}

		public Sprite[] AnimalSprites => _animalSprites;
		public Sprite[] PlaceholderSprites => _placeholderSprites;
		public Sprite[] CorrectAnswerSprites => _correctAnswerSprites;

		public void IncrementScore()
		{
			score++;
			
			if(CheckTaskComplete())
				TaskCompleted();
		}

		public void TaskCompleted()
		{
			GameController().Constructor().NextLevel();
		}

		public bool CheckTaskComplete()
		{
			return score >= _animalSprites.Length;
		}
		
		private ZooCarouselGameController GameController() => ZooCarouselGameController.Instance;
	}
}
