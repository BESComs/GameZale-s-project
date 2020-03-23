using UnityEngine;
using Work_Directory.Denis;
using Work_Directory.Denis.Scripts;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Collect
{
	public class CollectCheck : MonoBehaviour
	{
		public TaskCollect CollectTask;
		
		private Animate _anim;
		private Vector3 _initialScale;
		private bool _mouseEnter = false;

		private async void Awake()
		{
			_anim = new Animate(this, false);
			gameObject.SetActive(false);
			await new WaitForSeconds(2);
			gameObject.SetActive(true);
			await new Fade(transform, Mode.In).RunTask();			
		}
	
		private void OnMouseEnter()
		{
			_mouseEnter = true;
			_anim.Colorize(new Color(0.9f,0.9f,0.9f,1), 0.1f);
		}
	
		private void OnMouseExit()
		{
			_mouseEnter = false;
			_anim.Colorize(Color.white, 0.1f);
		}
	
		private void OnMouseDown()
		{
			if (CollectTask != null)
				CollectTask.CheckObjects();
			_anim.Colorize(Color.white, 0.1f);
		}
	
		private void OnMouseUp()
		{
			if (_mouseEnter)
				_anim.Colorize(new Color(0.9f,0.9f,0.9f,1), 0.1f);
		}
	}
}
