using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Work_Directory.Denis.Scripts.Tasks
{
	public class Counter : MonoBehaviour
	{
		//shows as many stars as the player has passed levels
		public SpriteRenderer StarPrefab;
		public float Margin = 0.6f;
		[Space]
		public float Delay = 0.5f;
		public Color Color = Color.yellow;

		private int _maxcount;
		private int _counter = 0;
		private SpriteRenderer[] _stars;
		private Animate _starAnim;
		private AnimationCurve _curve;

		
		private void Awake()
		{
			_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
			_curve.MoveKey(0, new Keyframe(0, 0, 0, 10));
			_curve.MoveKey(1, new Keyframe(0.5f, 1, 0, -10));
			_curve.AddKey(new Keyframe(1, 0, 0, 0));
		}
	
		public void Setup(int maxcount)
		{
			if (_stars != null)
				foreach (var star in _stars)
					Destroy(star);
			_maxcount = maxcount;
			_stars = new SpriteRenderer[_maxcount];
			for (var i = 0; i < _stars.Length; i++)
			{
				_stars[i] = Instantiate(StarPrefab.gameObject, transform).GetComponent<SpriteRenderer>();
				_stars[i].transform.position += Vector3.right * Margin * i;
			}	
		}

		public void Plus()
		{
			if (_counter == _maxcount) return;

			Timeout.Set(this, Delay, () =>
			{
				_starAnim = new Animate(this, _stars[_counter].transform);
				_starAnim.Colorize(Color, 0.35f);
				_starAnim.Scale(1.2f, 0.7f, _curve);
				_starAnim.RotateAdd(72f, 0.7f);
				_counter++;
			});
		}
	}
}
