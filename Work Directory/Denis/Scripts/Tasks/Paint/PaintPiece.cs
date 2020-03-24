using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Paint
{
	public class PaintPiece : MonoBehaviour
	{
		[HideInInspector] public bool IsPainted = false;
		private TaskPaint _taskPaint;
		private Animate _anim;

		private void Awake()
		{
			_taskPaint = GetComponentInParent<TaskPaint>();
			_anim = new Animate(this, true);
		}
		
		private void OnMouseDown()
		{
			if (_taskPaint.activeColor == null) return;
			
			_anim.Colorize(_taskPaint.activeColor.GetColor(), 0.2f);
			IsPainted = true;
			_taskPaint.CheckPieces();
		}
	}
}
