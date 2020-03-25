using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.CardMatchGame
{
	[Serializable]
	public class Card
	{
		[SerializeField] private int _id;
		[SerializeField] private Sprite _sprite;
		[SerializeField] private int _numOfCopies;

		public int GetCopiesCount()
		{
			return _numOfCopies;
		}

		public GameObject InitializeCardGameObject(GameObject gameObject, CardMatchGameController gameController)
		{
			gameObject.AddComponent<CardGameObject>().Init(_id, _sprite, gameController);
			return gameObject;
		}
	}

	public class CardGameObject : MonoBehaviour, IPointerClickHandler
	{
		public int Id { get; private set; }
		public Sprite Sprite { get; private set; }
		private SpriteRenderer _spriteRenderer;
		private BoxCollider2D _boxCollider;
		private CardMatchGameController _gameController;
		private bool isFlipped;
		private bool isFlipping;
		private float targetAngle;
		private bool isClickEnabled;

		public GameObject Init(int id, Sprite sprite, CardMatchGameController gameController)
		{
			Id = id;
			Sprite = sprite;
			gameObject.AddComponent<BoxCollider2D>();
			gameObject.AddComponent<Image>();
			_gameController = gameController;
			isFlipped = false;
			isFlipping = false;
			isClickEnabled = true;
			
			SetPlaceholderSprite();
			
			return gameObject;
		}

		private void FixedUpdate()
		{
			if (isFlipping)
			{
				//GetComponent<RectTransform>().Rotate(Vector3.up * 90 * Time.deltaTime * _gameController.FlipSpeed);
				GetComponent<RectTransform>().eulerAngles = new Vector3(0f, Mathf.LerpAngle(GetComponent<RectTransform>().eulerAngles.y, targetAngle, Time.deltaTime * _gameController.FlipSpeed));
				
				if (Mathf.FloorToInt(GetComponent<RectTransform>().transform.eulerAngles.y) == 0)
				{
					isFlipping = false;
				}
				else if (Mathf.CeilToInt(GetComponent<RectTransform>().transform.eulerAngles.y) >= 90)
				{
					Flip();
					targetAngle = 0;
				}
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (isClickEnabled && !_gameController.Wait)
			{
				TriggerFlip();
				_gameController.AddFlippedCard(this);
			}
		}

		public void SetClickEnabled(bool value)
		{
			isClickEnabled = value;
		}

		public void TriggerFlip()
		{
			if (!isFlipping) targetAngle = 90f;
			isFlipping = true;
		}
		
		private void Flip()
		{
			if(isFlipped)
			{
				SetPlaceholderSprite();
				isFlipped = !isFlipped;
				return;
			}
			SetMainSprite();
			isFlipped = !isFlipped;
		}
		
		private void SetPlaceholderSprite()
		{
			gameObject.GetComponent<Image>().sprite = _gameController.GetPlaceholderImage();
		}

		private void SetMainSprite()
		{
			gameObject.GetComponent<Image>().sprite = Sprite;
		}
	}
}
