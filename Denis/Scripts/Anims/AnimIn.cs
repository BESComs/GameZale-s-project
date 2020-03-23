using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Anims
{
	public class AnimIn : MonoBehaviour
	{
		[SerializeField] private float _startDelay = 0;
		[SerializeField] private Vector3 _fromMove = Vector3.zero;
		[SerializeField] private Vector3 _fromRotate = Vector3.zero;
		[SerializeField] private float _fromScale = 1f;
		[SerializeField] private float _duration = 1f;
		[Space]
		[SerializeField] private bool _colorize = false;
		[SerializeField] private Color _fromColor = Color.white;
		[Space]
		[SerializeField] private bool _pingPong = false;
		[SerializeField] private float _pingPongDelay = 0.5f;

		private Animate _anim;
		private Collider2D _collider;
		private Vector3 _initialPosition;
		private Vector3 _initialRotation;
		private Color _initialColor;

		private void Awake()
		{
			_anim = new Animate(this, true);
			_collider = GetComponent<Collider2D>();
			_initialPosition = transform.localPosition;
			_initialRotation = transform.localEulerAngles;
			_initialColor = _anim.GetColor();
		}

		private void Start()
		{
			if (_fromMove != Vector3.zero)
				_anim.SetPosition(_initialPosition + _fromMove);
			if (_fromRotate != Vector3.zero)
				_anim.SetRotation(_initialRotation + _fromRotate);
			if (_fromScale != 1f)
				_anim.SetScale(_fromScale);
			if (_colorize)
				_anim.SetColor(_fromColor);
			if (_collider != null)
			_collider.enabled = false;
			
			Timeout.Set(this, _startDelay, () =>
			{
				if (_fromMove != Vector3.zero)
					_anim.Move(_initialPosition, _duration);
				if (_fromRotate != Vector3.zero)
					_anim.Rotate(_initialRotation, _duration);
				if (_fromScale != 1f)
					_anim.Scale(1, _duration, _anim.SpringCurve);
				if (_colorize)
					_anim.Colorize(_initialColor, _duration);

				if (_pingPong)
					Timeout.Set(this, _pingPongDelay, () =>
					{
						if (_fromMove != Vector3.zero)
							_anim.MoveAdd(_fromMove, _duration);
						if (_fromRotate != Vector3.zero)
							_anim.RotateAdd(_fromRotate, _duration);
						if (_fromScale != 1f)
							_anim.Scale(_fromScale, _duration, _anim.SpringCurve);
						if (_colorize)
							_anim.Colorize(_fromColor, _duration);
					});
				
				if (_collider != null)
					Timeout.Set(this, (_pingPong) ? (_duration * 2f + _pingPongDelay) : _duration, () =>
					{
						_collider.enabled = true;
					});
			});
		}
	}
}