using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Anims
{
	public class AnimLevelTrain : MonoBehaviour, ILevelEvent
	{
		[Header("Start")]
		[SerializeField] private bool _onStart = true;
		[SerializeField] private float _startDelay = 0;
		[SerializeField] private bool _randomizeStartDelay = false;
		[SerializeField] private float _startDuration = 1f;
		[SerializeField] private Vector3 _fromMove = Vector3.zero;
		
		[Header("End")]
		[SerializeField] private bool _onEnd = true;
		[SerializeField] private float _endDelay = 0;
		[SerializeField] private bool _randomizeEndDelay = false;
		[SerializeField] private float _endDuration = 1f;
		[SerializeField] private Vector3 _toMove = Vector3.zero;
		[Space]
		[SerializeField] private bool _colorFading = false;

		private Animate _anim;
		private Collider2D _collider;
		private Vector3 _initialPosition;
		
		private void Awake()
		{
			_anim = new Animate(this, false);
			_collider = GetComponent<Collider2D>();
			_initialPosition = transform.position;
			
			if (_randomizeStartDelay)
				_startDelay = Random.Range((_startDelay - 0.25f > 0) ? _startDelay - 0.25f : 0, _startDelay + 0.25f);
			if (_randomizeEndDelay)
				_endDelay = Random.Range((_endDelay - 0.25f > 0) ? _endDelay - 0.25f : 0, _endDelay + 0.25f);
		}

		public void OnLevelStart()
		{
			if (!_onStart) return;

			_anim.SetPosition(_initialPosition + _fromMove);
			if (_colorFading)
				_anim.SetColor(0);
			if (_collider != null)
				_collider.enabled = false;
			Timeout.Set(this, _startDelay, () =>
			{
				_anim.Move(_initialPosition, _startDuration);
				if (_colorFading)
					_anim.Colorize(1, _startDuration);
				if (_collider != null)
					_collider.enabled = true;
			});
		}

		public void OnLevelEnd()
		{
			if (!_onEnd) return;

			if (_collider != null)
				_collider.enabled = false;
			Timeout.Set(this, _endDelay, () =>
			{
				_anim.MoveAdd( _toMove, _endDuration);
				if (_colorFading)
					_anim.Colorize(0, _endDuration);
				if (_collider != null)
					_collider.enabled = true;
			});
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
