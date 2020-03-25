using System.Collections;
using UnityEngine;

namespace Tasks.additional
{
	[RequireComponent(typeof(LineRenderer))]
	public class DrawPath : MonoBehaviour
	{
		public float duration = 5f;
		
		private LineRenderer _line;

		private void Awake()
		{
			_line = GetComponent<LineRenderer>();
		}
	
		private void OnEnable ()
		{
			StartCoroutine(Drawing());
		}

		private IEnumerator Drawing()
		{
			var points = new Vector3[_line.positionCount];
			_line.GetPositions(points);
			var dur = 1f * duration / points.Length;
			for (var i = 0; i < points.Length; i++)
			{
				_line.positionCount = i + 1;
				var li = i == 0 ? 0 : i - 1;
				var elp = 0f;
				while (elp < dur)
				{
					elp = Mathf.Clamp(elp + Time.deltaTime, 0f, dur);
					_line.SetPosition(i, Vector3.Lerp(points[li], points[i], elp / dur));
					yield return null;
				}
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
