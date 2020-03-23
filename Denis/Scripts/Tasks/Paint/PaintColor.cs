using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Paint
{
	public class PaintColor : MonoBehaviour
	{
		public bool autoSelect;
		public float autoSelectDelay = 1f;
		private TaskPaint _taskPaint;
		private Animate _anim;
		private Color _color;

		private void Awake()
		{
			_taskPaint = GetComponentInParent<TaskPaint>();
			_anim = new Animate(this, true);
			_color = GetComponent<SpriteRenderer>().color;
		}

		private void Start()
		{
			if (autoSelect)
				Timeout.Set(this, autoSelectDelay, OnMouseDown);
		}
		
		public void OnMouseDown()
		{
			if (_taskPaint.activeColor != null)
				_taskPaint.activeColor.Scaled(false);
			_taskPaint.activeColor = this;
			Scaled(true);
		}

		private void Scaled(bool value)
		{
			_anim.Scale((value) ? 1.3f : 1f, 0.25f);
		}

		public Color GetColor()
		{
			return _color;
		}
	}
}
