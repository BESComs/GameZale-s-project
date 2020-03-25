using UnityEngine;
using System.Collections.Generic;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.PaintPro
{
	public class PaintProPalette : MonoBehaviour
	{
		public bool autoSelect = false;
		public float autoSelectDelay = 1f;
		[Space]
		private List<PaintProColor> _colors;
		public int mainColor;
		private TaskPaintPro _taskPaintPro;
		private Animate _anim;
		private Vector3 _initialPosition;

		private void Awake()
		{
			_colors = new List<PaintProColor>();
			for (var i = 0; i < transform.childCount; i++)
			{
				var childComponent = transform.GetChild(i).GetComponent<PaintProColor>();
				if(childComponent != null)
					_colors.Add(childComponent);
			}
			_taskPaintPro = transform.GetComponentInParent<TaskPaintPro>();
			_anim = new Animate(this, true);
			_initialPosition = transform.localPosition;
		}
		
		private void Start()
		{
			if (autoSelect)
				Timeout.Set(this, autoSelectDelay, OnMouseDown);
			else
				Selected(false);
		}
		
		public void OnMouseDown()
		{
			if (_taskPaintPro.activePalette)
				_taskPaintPro.activePalette.Selected(false);
			_taskPaintPro.activePalette = this;
			_taskPaintPro.activePalette.Selected(true);
		}

		private void Selected(bool value)
		{
			foreach (var color in _colors)
				color.gameObject.SetActive(value);
			_anim.Move((value) ? _initialPosition + new Vector3(-0.25f, 0, 0) : _initialPosition, 0.2f);
			if (value)
				_colors[mainColor].OnMouseDown();
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
