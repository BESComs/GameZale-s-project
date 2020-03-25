using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.PaintPro
{
	public class PaintProStep : MonoBehaviour
	{
		public StepType type;
		private TaskPaintPro _taskPaintPro;
		private Animate _anim;
		private bool _active;

		private void Awake()
		{
			_taskPaintPro = GetComponentInParent<TaskPaintPro>();
			_anim = new Animate(this, true);
		}
		
		public void SetActive(bool value)
		{
			_active = value;
			_anim.SetColor((value) ? 1f : 0.5f);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		public void OnMouseDown()
		{
			if (!_active) return;
			switch (type)
			{
				case StepType.Undo:
					_taskPaintPro.Prev();
					break;
				case StepType.Redo:
					_taskPaintPro.Next();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_anim.Scale(1f, 1.2f, 0.5f, _anim.BounceCurve);
		}
	}
}
