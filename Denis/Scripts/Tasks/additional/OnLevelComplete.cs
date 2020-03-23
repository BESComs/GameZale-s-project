using UnityEngine;

namespace Tasks.additional
{
	public class OnLevelComplete : MonoBehaviour, ILevelEvent
	{
		public float delay = 0.5f;
		public GameObject[] enable;
		
		public void OnLevelStart() {}

		public void OnLevelEnd()
		{
			Timeout.Set(this, delay, () =>
			{
				foreach (var obj in enable)
					obj.SetActive(true);
			});
		}
	}
}
