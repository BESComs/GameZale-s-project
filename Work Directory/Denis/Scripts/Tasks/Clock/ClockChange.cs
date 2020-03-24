using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;
using Work_Directory.Denis.Scripts.Tasks.Clock;

namespace Tasks.Clock
{
	public class ClockChange : MonoBehaviour
	{
		public TaskClock.ChangeType Change = TaskClock.ChangeType.Hours;
		public int Add = 1;

		private Animate _anim;

		private void Awake()
		{
			_anim = new Animate(this, true);
		}
	
		private void OnMouseDown()
		{
			_anim.Scale(1.2f, 0.2f);
			TaskClock.Instance.ChangeClock(Change, Add);
			TaskClock.Instance.CheckClock();
		}
		
		private void OnMouseUp()
		{
			_anim.Scale(1f, 0.2f);
		}
	}
}
