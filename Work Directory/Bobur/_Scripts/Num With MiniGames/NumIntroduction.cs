using System;
using System.Threading.Tasks;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
	class NumIntroduction
	{
		[SerializeField] private Sprite _numberSprite;
		[SerializeField] private Position2D _moveAnimationTargetPosition;
		[SerializeField] private Sprite _numberDescSprite;
		[SerializeField] private Position2D _numberDescStartPosition;

		private GameObject _introductionParent;
		private GameObject _numberObject;
		private GameObject _numberDescObject;

		public async void Init()
		{
			gameController.TextObject.gameObject.SetActive(false);
			SetIntroductionParent();
			SetObject(out _numberObject, "Number", _numberSprite, _introductionParent);
			SetObject(out _numberDescObject, "Number Description", _numberDescSprite, _introductionParent, _numberDescStartPosition);
			
			_numberObject.transform.localScale = Vector3.zero;
			_numberDescObject.transform.localScale = Vector3.zero;

			await AnimationUtility.ScaleIn(_numberObject, gameController.AnimSpeed, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
			await Awaiters.Seconds(gameController.DelayBetweenAnims);
			await AnimationUtility.MoveTo(_numberObject,
											new Vector3(_moveAnimationTargetPosition.X, _moveAnimationTargetPosition.Y),
											gameController.AnimSpeed,
											AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
			
			await Awaiters.Seconds(gameController.DelayBetweenAnims);
			await AnimationUtility.ScaleIn(_numberDescObject, gameController.AnimSpeed, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

			AnimationUtility.RotateLoop(_numberDescObject, new Vector3(0f, 0f, -15f), gameController.AnimSpeed / 2);

			await Awaiters.Seconds(1f);
			gameController.SetNextButtonEnabled(true);
		}

		public async Task Finish()
		{
			AnimationUtility.FadeOut(_numberObject.GetComponent<SpriteRenderer>(), gameController.AnimSpeed);
			await AnimationUtility.FadeOut(_numberDescObject.GetComponent<SpriteRenderer>(), gameController.AnimSpeed);
			await Awaiters.Seconds(0.5f);
			
			GameObject.Destroy(_introductionParent);
		}

		private void SetObject(out GameObject assignedObject, string name, Sprite sprite, GameObject parent, Position2D position = default(Position2D))
		{
			assignedObject = new GameObject(name);
			assignedObject.AddComponent<SpriteRenderer>().sprite = sprite;
			assignedObject.GetComponent<SpriteRenderer>().sortingLayerName = "Empty Shapes";
			
			assignedObject.transform.SetParent(parent.transform);

			Vector3 pos;
			if (position != default(Position2D))
				pos = new Vector3(position.X, position.Y);
			else
				pos = Vector3.zero;	
			
			assignedObject.transform.localPosition = pos;
		}

		private void SetIntroductionParent()
		{
			_introductionParent = new GameObject("Introduction");
			_introductionParent.transform.SetParent(gameController.transform);
		}
		
		private NumWMGGameController gameController => NumWMGGameController.Instance;
	}
}