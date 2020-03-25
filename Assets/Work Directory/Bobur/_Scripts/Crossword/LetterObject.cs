using System;
using TMPro;
using UnityEngine;

namespace _Scripts.Crossword
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class LetterObject : MonoBehaviour
	{

		[Serializable]
		enum LetterType
		{
			Letter,
			LetterBox
		}

		[SerializeField] private LetterType _type;
		[SerializeField] private string _letter;
		[SerializeField] private TextMeshPro _textObject;
		
		private CrosswordGameController _gameController => CrosswordGameController.Instance;
		
		private LetterObject _collidedLetterObject;
		private bool _isMoving;
		
		private void Start()
		{
			Init();
		}

		public void Init()
		{
			if (_type == LetterType.LetterBox)
			{
				Letter = (_letter != "") ? _letter.ToUpper() : "";
				SetTextEnabled(false);
				_gameController.AddLetterToFind(this);
			}
		}

		private void OnValidate()
		{
			gameObject.name = _letter.ToUpper();
		}

		private void FixedUpdate()
		{
			if (_type == LetterType.Letter && _isMoving)
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1f, LayerMask.GetMask("Top"));
				if (hit.collider != null)
				{
					_collidedLetterObject = hit.collider.gameObject.GetComponent<LetterObject>();
				}
				else
				{
					_collidedLetterObject = null;
				}
			}
		}

		public string Letter
		{
			get => _letter;
			set
			{
				_letter = value;
				_textObject.text = value;
				gameObject.name = value;
			}
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.GetComponent<LetterObject>() != null && other.gameObject.GetComponent<LetterObject>()._type == LetterType.LetterBox)
			{
				_collidedLetterObject = other.gameObject.GetComponent<LetterObject>();
			}
		}

		private void OnCollisionExit2D(Collision2D other)
		{
			_collidedLetterObject = null;
		}

		private void OnMouseUp()
		{
			if (_collidedLetterObject != null && _collidedLetterObject.Letter == _letter)
			{
				_collidedLetterObject.SetTextEnabled(true);
				_gameController.LetterFound();
			}

			_isMoving = false;
		}

		private void OnMouseDown() => _isMoving = true;

		public void SetTextEnabled(bool value) => _textObject.gameObject.SetActive(value);
	}
}