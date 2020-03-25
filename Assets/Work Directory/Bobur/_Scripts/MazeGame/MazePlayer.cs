using UnityEngine;

namespace _Scripts.MazeGame
{
	public class MazePlayer : MonoBehaviour
	{

		[SerializeField] private MazeGameController _gameController; 
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("Collectable"))
			{
				if(_gameController){
					_gameController.IncrementScore();
					Destroy(other.gameObject);
				}
			} else if (other.gameObject.Equals(_gameController.GetExitZone()))
			{
				_gameController.TaskCompleted();
			}
		}
	}
}
