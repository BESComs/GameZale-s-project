using UnityEngine;

namespace Tasks.additional
{
	public class InfinityRotate : MonoBehaviour
	{
		[SerializeField] private float degreesPerSec = 1f;

		private Vector3 _degree = Vector3.zero;
	
		private void Update ()
		{
			_degree.z += degreesPerSec * Time.deltaTime;
			transform.localEulerAngles = _degree;
		}
	}
}
