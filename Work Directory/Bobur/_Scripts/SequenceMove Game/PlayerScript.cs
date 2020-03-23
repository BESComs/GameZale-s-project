using System.Threading.Tasks;
using UnityEngine;

namespace _Scripts.SequenceMove_Game
{
	public class PlayerScript : MonoBehaviour
	{
		[SerializeField] private float JumpHeight;
		[SerializeField] private Sprite IdleSprite;
		[SerializeField] private Sprite JumpingSprite;
		[SerializeField] private Vector2 JumpDelta;

		private SpriteRenderer _spriteRenderer;
		
		// Elements needed for Jump
		private Vector3 targetPos;
		private Vector3 startPos;
		private bool isMoving;
		private float timer;
		
		void Start ()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			isMoving = false;
			timer = 0.0f;
		}
	
		void Update () {
			if (isMoving)
			{
				if (timer <= 1.0f)
				{
					float height = Mathf.Sin(Mathf.PI * timer) * JumpHeight;
					transform.localPosition = Vector3.Lerp(startPos, targetPos, timer) + Vector3.up * height;
					timer += Time.deltaTime;
				}
				else
				{
					isMoving = false;
					timer = 0.0f;
					_spriteRenderer.sprite = IdleSprite;
					_spriteRenderer.flipX = false;
					SequenceMoveGameController.Instance.GetCurrentLevel().NextTarget();
				}
			}
		}
		
		public void Jump(Vector3 target)
		{
			if (isMoving) return;
			
			targetPos = target;		
			startPos = transform.localPosition;
			isMoving = true;
			_spriteRenderer.sprite = JumpingSprite;
			
			// Sprite Flip Logic Here
			if (!(transform.localPosition.x - target.x < 0))
			{
				_spriteRenderer.flipX = true;
			}
		}

		public Vector2 Delta => JumpDelta;
	}
}
