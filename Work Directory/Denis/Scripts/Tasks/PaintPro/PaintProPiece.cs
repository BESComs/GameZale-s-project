using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.PaintPro
{
	public class PaintProPiece : MonoBehaviour
	{
		private Animate _anim;
		private TaskPaintPro _taskPaintPro;
		private void Awake()
		{
			_taskPaintPro = GetComponentInParent<TaskPaintPro>();
			_anim = new Animate(this, true);
		}
		
		private void OnMouseDown()
		{
			_taskPaintPro.Step(this, _anim.GetColor());
			Colorize(_taskPaintPro.Get());
		}

		public void Colorize(Color color)
		{
			_anim.Colorize(color, 0.2f);
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}