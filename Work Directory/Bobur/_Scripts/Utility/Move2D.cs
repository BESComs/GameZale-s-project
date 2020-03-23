using UnityEngine;

namespace _Scripts.Utility
{
	public class Move2D : MonoBehaviour
	{
	
		private enum MoveType
		{
			MovePosition,
			AddForce
		}
	
		private Rigidbody2D _rigidbody2D;
		public float MaxSpeed;
		public float SpeedDivider = 1;
		[SerializeField] private MoveType _moveType;
		public bool DoFlip;
	
		// Use this for initialization
		void Start ()
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if(DoFlip){
				Flip();
			}

			if (_moveType == MoveType.AddForce)
			{
				_rigidbody2D.AddForce(move * MaxSpeed);
			}
			else if(_moveType == MoveType.MovePosition)
			{
				_rigidbody2D.MovePosition(_rigidbody2D.position + (move / SpeedDivider));
			}

		}

		private void Flip()
		{
			if(Input.GetAxis("Horizontal") > 0)
			{
				GetComponent<SpriteRenderer>().flipX = true;
			}
			else if(Input.GetAxis("Horizontal") < 0)
			{
				GetComponent<SpriteRenderer>().flipX = false;
			}
		}
	}
}