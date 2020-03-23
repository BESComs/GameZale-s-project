using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Points
{
	public class PointsPoint : MonoBehaviour
	{
		public int Number;
		private TaskPoints _taskPoints;
		private TextMesh _text;
		private Animate _anim;

		private void Awake()
		{
			_text = GetComponent<TextMesh>();
			_text.text = "" + Number;
			_anim = new Animate(this, true);
			_anim.SetScale(0);
		}

		private void Start()
		{
			_taskPoints = GetComponentInParent<TaskPoints>();
			Timeout.Set(this, Random.Range(0.5f, 1), ()=>
			{
				_anim.Scale(1, 0.5f, _anim.SpringCurve);
			});
		}
		
		private void OnMouseDown()
		{
			if (_taskPoints.ActiveNumber != Number - 1) return;
			_taskPoints.DrawLine(this);
			_text.color = _taskPoints.PointSecondColor;
			Timeout.Set(this, 0.25f, () =>
			{
				_anim.Scale(0, 0.25f);
			});
			
		}
	}
}
