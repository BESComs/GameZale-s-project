using UnityEngine;

namespace Tasks.additional
{
	public class DisableForTime : MonoBehaviour
	{
		public bool doOnEnable = true;
		[Space]
		public Collider2D[] objects;
		public float disableTime = 1f;

		private SpriteRenderer[] _sprites;

		private void Awake()
		{
			_sprites = new SpriteRenderer[objects.Length];
			for (var i = 0; i < objects.Length; i++)
				_sprites[i] = objects[i].GetComponent<SpriteRenderer>();
		}

		private void OnEnable()
		{
			if (doOnEnable)
				Do();
		}

		private void Do ()
		{
			if (objects.Length <= 0) return;
			for (var i = 0; i < objects.Length; i++)
			{
				objects[i].enabled = false;
				if (_sprites[i] != null)
					_sprites[i].enabled = false;
			}
			if (disableTime > 0)
				Timeout.Set(this, disableTime, () =>
				{
					for (var i = 0; i < objects.Length; i++)
					{
						objects[i].enabled = true;
						if (_sprites[i] != null)
							_sprites[i].enabled = true;
					}
				});
		}
	}
}
