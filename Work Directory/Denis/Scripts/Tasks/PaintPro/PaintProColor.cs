using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.PaintPro
{
	public class PaintProColor : MonoBehaviour
	{
		private Animate _anim;
		public Color color;
		private TaskPaintPro _taskPaintPro;
		private void Awake()
		{
			_taskPaintPro = transform.GetComponentInParent<TaskPaintPro>();
			_anim = new Animate(this, true);
			color = GetComponent<SpriteRenderer>().color;
		}
		
		public void OnMouseDown()
		{
			_taskPaintPro.Set(this);
			_anim.Scale(1, 1.2f, 0.5f, _anim.BounceCurve);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
