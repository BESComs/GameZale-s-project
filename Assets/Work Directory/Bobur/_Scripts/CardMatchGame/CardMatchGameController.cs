using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace _Scripts.CardMatchGame
{
	public class CardMatchGameController : MonoBehaviour, ITask
	{
		[SerializeField] private Sprite _placeholderImage;
		public ParticleSystem ParticleSystem;
		public List<Card> Cards;
		private List<CardGameObject> _flippedCards;
		public GameObject CardsParentObject;
		public GameObject UiElements;
		public float FlipSpeed;
		private int _score;
		private int _maxScore;
		[SerializeField] private float FlipBackWaitTime;
		[SerializeField] private AudioClip SuccessAudioClip;
		[HideInInspector] public bool Wait { get; private set; }
		
		//TODO - in future avoid of using it. This was not best solution :(
		private bool _playSuccess;

		// Use this for initialization
		void Start () {
			UiElements.SetActive(true);
			InitializeCards();
			_score = 0;
			_maxScore = CardsParentObject.transform.childCount / 2;
			_flippedCards = new List<CardGameObject>();
			Wait = false;
			_playSuccess = false;
			UiElements.SetActive(true);
		}

		// Update is called once per frame
		void Update () {
			if (_playSuccess)
			{
				AudioSource.PlayClipAtPoint(SuccessAudioClip, transform.position);
				_playSuccess = false;
				
				if (CheckTaskComplete())
					TaskCompleted();
			}
		}

		private void InitializeCards()
		{
			ShuffleCards();
			foreach (Card card in Cards)
			{
				for (int i = 0; i < card.GetCopiesCount(); i++)
				{
					GameObject newCard = card.InitializeCardGameObject(new GameObject(), this);
					newCard.transform.SetParent(CardsParentObject.transform, false);
				}
			}
		}

		public void AddFlippedCard(CardGameObject cardGameObject)
		{
			_flippedCards.Add(cardGameObject);
			cardGameObject.SetClickEnabled(false);
			new Thread(CheckMatch).Start();
		}

		private void CheckMatch()
		{
			if (_flippedCards.Count == 2)
			{
				if (_flippedCards[0].Id == _flippedCards[1].Id)
				{
					_score++;
					_flippedCards.Clear();
					_playSuccess = true;
					return;
				}

				Wait = true;
				Thread.Sleep(TimeSpan.FromSeconds(FlipBackWaitTime));
				
				foreach (var flippedCard in _flippedCards)
				{
					flippedCard.TriggerFlip();
					flippedCard.SetClickEnabled(true);
				}
				_flippedCards.Clear();
				Wait = false;
			}
		}
		
		public Sprite GetPlaceholderImage()
		{
			return _placeholderImage;
		}

		private void ShuffleCards()
		{
			System.Random random = new System.Random();
			int n = Cards.Count;
			
			while (n > 1)
			{
				n--;
				int k = random.Next(n + 1);
				Card value = Cards[k];
				Cards[k] = Cards[n];
				Cards[n] = value;
			}
		}
		
		public void TaskCompleted()
		{
			GameManager.GetInstance().CurrentGameFinished(this);
			UiElements.SetActive(false);
		}

		public bool CheckTaskComplete()
		{
			return _score == _maxScore;
		}
	}
}
