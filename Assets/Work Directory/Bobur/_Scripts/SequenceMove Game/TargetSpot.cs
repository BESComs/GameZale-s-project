using TMPro;
using UnityEngine;

namespace _Scripts.SequenceMove_Game
{
	public class TargetSpot : MonoBehaviour
	{

		[SerializeField] private TextMeshPro _number;
		private SpriteRenderer _spriteRenderer;
		public int ID { get; set; }
	
		// Use this for initialization
		void Start ()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			SetIdleColor();
		}

		private void OnMouseEnter()
		{
			Highlight();
		}

		private void OnMouseExit()
		{
			SetIdleColor();
		}

		private void OnMouseDown()
		{
			if (CurrentLevel().CurrentTarget == ID)
			{
				var player = SequenceMoveGameController.Instance.GetCurrentLevel().PlayerGameObject
					.GetComponent<PlayerScript>();
				player.Jump(transform.localPosition + (Vector3) player.Delta);
			}
		}

		public void SetIdleColor()
		{
			_spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 1f);
		}
	
		public void Highlight()
		{
			_spriteRenderer.color = Color.white;
		}

		public void SetNumber(int num)
		{
			_number.text = num.ToString();
		}

		private SequenceMoveConstructorScript CurrentLevel()
		{
			return SequenceMoveGameController.Instance.GetCurrentLevel();
		}
	}
}
