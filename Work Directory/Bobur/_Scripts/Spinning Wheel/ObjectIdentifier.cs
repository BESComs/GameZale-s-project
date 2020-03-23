using UnityEngine;

namespace _Scripts.Spinning_Wheel
{
	public class ObjectIdentifier : MonoBehaviour
	{

		[SerializeField] private int _correctMatch;
		[SerializeField] private int _position;

		private void OnTriggerEnter2D(Collider2D other)
		{
			SpinningWheelGameController.Instance.SetCurrentPosition(_position);
			SpinningWheelGameController.Instance.SetCurrentObject(this);
		}

		public void SetCurrentObjectOption(Sprite optionSprite)
		{
			transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = optionSprite;
		}
		
		public int GetCorrectMatch()
		{
			return _correctMatch;
		}
		
		public int GetPosition()
		{
			return _position;
		}
	}
}
