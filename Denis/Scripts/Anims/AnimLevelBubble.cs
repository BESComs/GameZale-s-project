using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Anims
{
	public class AnimLevelBubble : MonoBehaviour, ILevelEvent
	{
		[Header("Start")]
		[SerializeField] private bool _onStart = true;
		[SerializeField] private float _startDelay;
		[SerializeField] private bool _randomizeStartDelay;
		[SerializeField] private float _startDuration = 0.5f;
		
		[Header("End")]
		[SerializeField] private bool _onEnd = true;
		[SerializeField] private float _endDelay;
		[SerializeField] private bool _randomizeEndDelay;
		[SerializeField] private float _endDuration = 0.5f;
		[Space]
		[SerializeField] private bool _colorFading = true;

		private Animate _anim;
		private Collider2D _collider;
		
		private void Awake()
		{
			_anim = new Animate(this, true);
			_collider = GetComponent<Collider2D>();
			if (_randomizeStartDelay)
				_startDelay = Random.Range((_startDelay - 0.25f > 0) ? _startDelay - 0.25f : 0, _startDelay + 0.25f);
			if (_randomizeEndDelay)
				_endDelay = Random.Range((_endDelay - 0.25f > 0) ? _endDelay - 0.25f : 0, _endDelay + 0.25f);
		}

		public void OnLevelStart()
		{
			if (!_onStart) return;
			if(_anim == null)
				_anim = new Animate(this, true);
			_anim.SetScale(0);
			if (_collider != null)
				_collider.enabled = false;
			Timeout.Set(this, _startDelay, () =>
			{
				_anim.Scale(0, 1, _startDuration);
				if (_colorFading)
					_anim.Colorize(0, 1, _startDuration);
				if (_collider != null)
					Timeout.Set(this, _startDuration, () =>
					{
						_collider.enabled = true;
					});
			});
		}
		
		public void OnLevelEnd()
		{
			if (!_onEnd) return;

			if (_collider != null)
				_collider.enabled = false;
			Timeout.Set(this, _endDelay, () =>
			{
				_anim.Scale(0, _endDuration);
				if (_colorFading)
					_anim.Colorize(0, _endDuration);
				if (_collider != null)
					Timeout.Set(this, _endDuration, () =>
					{
						gameObject.SetActive(false);
					});
			});
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}