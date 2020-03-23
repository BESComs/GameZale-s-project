using System.Collections;
using UnityEngine;

namespace Work_Directory.Denis.Scripts.New_Folder
{
	//Анимации (Шох Джахона)
	public class Animate
	{
		private readonly MonoBehaviour _behaviour;
		private readonly Transform _transform;
		private readonly AnimateColor _color;
		
		public Animate(MonoBehaviour behavior, bool local)
		{
			_behaviour = behavior;
			
			_transform = _behaviour.transform;
			_isLocal = local;
			_scaleScale = _transform.localScale;

			_color = new AnimateColor(_transform);
			_colorColor = _color.Get();

			Defaults();
		}
		public Animate(MonoBehaviour behavior, Transform transform)
		{
			_behaviour = behavior;
			
			_transform = transform;
			_isLocal = true;
			_scaleScale = _transform.localScale;

			_color = new AnimateColor(_transform);
			_colorColor = _color.Get();

			Defaults();
		}
	
		private readonly bool _isLocal;
		private Vector3 TransformPosition
		{
			get
			{
				return (_isLocal) ? _transform.localPosition : _transform.position;
			}
			set
			{
				if (_isLocal)
					_transform.localPosition = value;
				else
					_transform.position = value;
			}
		}
		
		private Vector3 TransformRotation
		{
			get { return (_isLocal) ? _transform.localEulerAngles : _transform.eulerAngles; }
			set
			{
				if (_isLocal)
					_transform.localEulerAngles = value;
				else
					_transform.eulerAngles = value;
			}
		}
		
		private float _defaultDuration;
		private AnimationCurve _defaultCurve;
		public AnimationCurve SpringCurve;
		public AnimationCurve BounceCurve;
		private void Defaults()
		{
			_defaultDuration = 0.5f;
			_defaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
			_defaultCurve.MoveKey(0, new Keyframe(0, 0, 0, 3));
			_defaultCurve.MoveKey(1, new Keyframe(1, 1, 0, 0));
		
			SpringCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
			SpringCurve.MoveKey(0, new Keyframe(0, 0, 0, 0.5f));
			SpringCurve.MoveKey(1, new Keyframe(0.75f, 1, -0.5f, -0.5f));
			SpringCurve.AddKey(new Keyframe(1, 1, 0f, 0));
		
			BounceCurve = AnimationCurve.EaseInOut(0,0,1,1);
			BounceCurve.MoveKey(0, new Keyframe(0, 0, 0, 0));
			BounceCurve.MoveKey(1, new Keyframe(0.5f, 1, 0, 0));
			BounceCurve.AddKey(new Keyframe(1, 0, 0, 0));
		}

		private Vector3 _moveFrom;
		private Vector3 _moveArc;
		private Vector3 _moveTo;
		private float _moveDuration;
		private AnimationCurve _moveCurve;

		private IEnumerator MoveCoroutine()
		{
			var moveElapsed = 0f;
			if (_moveDuration <= 0) _moveDuration = Time.deltaTime;
			while (moveElapsed < _moveDuration)
			{
				moveElapsed = Mathf.Clamp(moveElapsed + Time.deltaTime, 0, _moveDuration);
				TransformPosition = Vector3.Lerp(_moveFrom, _moveTo, _moveCurve.Evaluate(moveElapsed / _moveDuration));
				yield return null;
			}
			_moveStarted = false;
		}
		private IEnumerator MoveArcCoroutine()
		{
			float moveElapsed = 0f;
			if (_moveDuration <= 0) _moveDuration = Time.deltaTime;
			while (moveElapsed < _moveDuration)
			{
				moveElapsed = Mathf.Clamp(moveElapsed + Time.deltaTime, 0, _moveDuration);
				var curved = _moveCurve.Evaluate(moveElapsed / _moveDuration);
				TransformPosition = Vector3.Lerp(Vector3.Lerp(_moveFrom, _moveArc, curved), Vector3.Lerp(_moveArc, _moveTo, curved), curved);
				yield return null;
			}
			_moveStarted = false;
		}
		private Coroutine _moveCoroutine;
		private bool _moveStarted;
		private void Move()
		{
			if (!_transform.gameObject.activeInHierarchy) return;
		
			if (_moveStarted)
				_behaviour.StopCoroutine(_moveCoroutine);
			else
				_moveStarted = true;
			_moveCoroutine = _behaviour.StartCoroutine(MoveCoroutine());	
		}
		private void MoveArc()
		{
			if (!_transform.gameObject.activeInHierarchy) return;
		
			if (_moveStarted)
				_behaviour.StopCoroutine(_moveCoroutine);
			else
				_moveStarted = true;
			_moveCoroutine = _behaviour.StartCoroutine(MoveArcCoroutine());	
		}

		public void Move(Vector3 to)
		{
			_moveFrom = TransformPosition;
			_moveTo = to;
			_moveDuration = _defaultDuration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void Move(Vector2 to)
		{
			Move(new Vector3(to.x, to.y, TransformPosition.z));
		}
		public void Move(Vector3 to, float duration)
		{
			_moveFrom = TransformPosition;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void Move(Vector2 to, float duration)
		{
			Move(new Vector3(to.x, to.y, TransformPosition.z), duration);
		}
		public void Move(Vector3 to, float duration, AnimationCurve curve)
		{
			_moveFrom = TransformPosition;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = curve;
			Move();
		}
	
		public void Move(Vector2 to, float duration, AnimationCurve curve)
		{
			Move(new Vector3(to.x, to.y, TransformPosition.z), duration, curve);
		}
	
		public void Move(Vector3 from, Vector3 to)
		{
			_moveFrom = from;
			_moveTo = to;
			_moveDuration = _defaultDuration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void Move(Vector2 from, Vector2 to)
		{
			Move(new Vector3(from.x, from.y, TransformPosition.z), new Vector3(to.x, to.y, TransformPosition.z));
		}
		public void Move(Vector3 from, Vector3 to, float duration)
		{
			_moveFrom = from;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void Move(Vector2 from, Vector2 to, float duration)
		{
			Move(new Vector3(from.x, from.y, TransformPosition.z), new Vector3(to.x, to.y, TransformPosition.z), duration);
		}
		public void Move(Vector3 from, Vector3 to, float duration, AnimationCurve curve)
		{
			_moveFrom = from;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = curve;
			Move();
		}
		public void Move(Vector2 from, Vector2 to, float duration, AnimationCurve curve)
		{
			Move(new Vector3(to.x, to.y, TransformPosition.z), duration, curve);
		}
		public void MoveAdd(Vector3 add)
		{
			_moveFrom = TransformPosition;
			_moveTo = _moveFrom + add;
			_moveDuration = _defaultDuration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void MoveAdd(Vector2 add)
		{
			MoveAdd(new Vector3(add.x, add.y, TransformPosition.z));
		}
		public void MoveAdd(Vector3 add, float duration)
		{
			_moveFrom = TransformPosition;
			_moveTo = _moveFrom + add;
			_moveDuration = duration;
			_moveCurve = _defaultCurve;
			Move();
		}
		public void MoveAdd(Vector2 add, float duration)
		{
			MoveAdd(new Vector3(add.x, add.y, TransformPosition.z), duration);
		}
		public void MoveAdd(Vector3 add, float duration, AnimationCurve curve)
		{
			_moveFrom = TransformPosition;
			_moveTo = _moveFrom + add;
			_moveDuration = duration;
			_moveCurve = curve;
			Move();
		}
		public void MoveAdd(Vector2 add, float duration, AnimationCurve curve)
		{
			MoveAdd(new Vector3(add.x, add.y, TransformPosition.z), duration, curve);
		}
		public void MoveArc(Vector3 from, Vector3 arc, Vector3 to)
		{
			_moveFrom = from;
			_moveArc = arc;
			_moveTo = to;
			_moveDuration = _defaultDuration;
			_moveCurve = _defaultCurve;
			MoveArc();
		}
		public void MoveArc(Vector3 from, Vector3 arc, Vector3 to, float duration)
		{
			_moveFrom = from;
			_moveArc = arc;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = _defaultCurve;
			MoveArc();
		}
		public void MoveArc(Vector3 from, Vector3 arc, Vector3 to, float duration, AnimationCurve curve)
		{
			_moveFrom = from;
			_moveArc = arc;
			_moveTo = to;
			_moveDuration = duration;
			_moveCurve = curve;
			MoveArc();
		}
	
		public void SetPositionZ(float value)
		{
			_moveFrom.z = value;
			_moveTo.z = value;
			TransformPosition = new Vector3(TransformPosition.x, TransformPosition.y, value);
		}
		public void SetPosition(Vector3 value)
		{
			if (_moveStarted)
			{
				_moveStarted = false;
				_behaviour.StopCoroutine(_moveCoroutine);
			}

			TransformPosition = value;
		}
		public void SetPosition(Vector2 value)
		{
			SetPosition(new Vector3(value.x, value.y, TransformPosition.z));
		}
	
		private Vector3 _rotateFrom;
		private Vector3 _rotateTo;
		private float _rotateDuration;
		private AnimationCurve _rotateCurve;

		private Vector3? _rotateRotation;
		private Vector3 Rotation {
			get
			{
				if (_rotateRotation == null) _rotateRotation = TransformRotation;
				return _rotateRotation.Value;
			}
			set { _rotateRotation = value; }
		}
		private IEnumerator RotateCoroutine()
		{
			float rotateElapsed = 0f;
			if (_rotateDuration <= 0) _rotateDuration = Time.deltaTime;
			while (rotateElapsed < _rotateDuration)
			{
				rotateElapsed = Mathf.Clamp(rotateElapsed + Time.deltaTime, 0, _rotateDuration);
				Rotation = Vector3.Lerp(_rotateFrom, _rotateTo, _rotateCurve.Evaluate(rotateElapsed / _rotateDuration));
				TransformRotation = Rotation;
				yield return null;
			}
			_rotateStarted = false;
		}
	
		private Coroutine _rotateCoroutine;
		private bool _rotateStarted;
		private void Rotate()
		{
			if (!_transform.gameObject.activeInHierarchy) return;
		
			if (_rotateStarted)
				_behaviour.StopCoroutine(_rotateCoroutine);
			else
				_rotateStarted = true;
			_rotateCoroutine = _behaviour.StartCoroutine(RotateCoroutine());	
		}

		public void Rotate(Vector3 to)
		{
			_rotateFrom = Rotation;
			_rotateTo = to;
			_rotateDuration = _defaultDuration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void Rotate(Vector3 to, float duration)
		{
			_rotateFrom = Rotation;
			_rotateTo = to;
			_rotateDuration = duration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void Rotate(Vector3 to, float duration, AnimationCurve curve)
		{
			_rotateFrom = Rotation;
			_rotateTo = to;
			_rotateDuration = duration;
			_rotateCurve = curve;
			Rotate();
		}
		public void Rotate(Vector3 from, Vector3 to)
		{
			_rotateFrom = from;
			_rotateTo = to;
			_rotateDuration = _defaultDuration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void Rotate(Vector3 from, Vector3 to, float duration)
		{
			_rotateFrom = from;
			_rotateTo = to;
			_rotateDuration = duration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void Rotate(Vector3 from, Vector3 to, float duration, AnimationCurve curve)
		{
			_rotateFrom = from;
			_rotateTo = to;
			_rotateDuration = duration;
			_rotateCurve = curve;
			Rotate();
		}
		public void Rotate(float to)
		{
			Rotate(new Vector3(Rotation.x, Rotation.y, to));
		}
		public void Rotate(float to, float duration)
		{
			Rotate(new Vector3(Rotation.x, Rotation.y, to), duration);
		}
		public void Rotate(float to, float duration, AnimationCurve curve)
		{
			Rotate(new Vector3(Rotation.x, Rotation.y, to), duration, curve);
		}
		public void Rotate(float from, float to, float duration)
		{
			Rotate(new Vector3(Rotation.x, Rotation.y, from), new Vector3(Rotation.x, Rotation.y, to), duration);
		}
		public void Rotate(float from, float to, float duration, AnimationCurve curve)
		{
			Rotate(new Vector3(Rotation.x, Rotation.y, from), new Vector3(Rotation.x, Rotation.y, to), duration, curve);
		}
		public void RotateAdd(Vector3 add)
		{
			_rotateFrom = Rotation;
			_rotateTo = _rotateFrom + add;
			_rotateDuration = _defaultDuration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void RotateAdd(Vector3 add, float duration)
		{
			_rotateFrom = Rotation;
			_rotateTo = _rotateFrom + add;
			_rotateDuration = duration;
			_rotateCurve = _defaultCurve;
			Rotate();
		}
		public void RotateAdd(Vector3 add, float duration, AnimationCurve curve)
		{
			_rotateFrom = Rotation;
			_rotateTo = _rotateFrom + add;
			_rotateDuration = duration;
			_rotateCurve = curve;
			Rotate();
		}
		public void RotateAdd(float add)
		{
			RotateAdd(new Vector3(0, 0, add));
		}
		public void RotateAdd(float add, float duration)
		{
			RotateAdd(new Vector3(0, 0, add), duration);
		}
		public void RotateAdd(float add, float duration, AnimationCurve curve)
		{
			RotateAdd(new Vector3(0, 0, add), duration, curve);
		}
		public void SetRotation(Vector3 value)
		{
			if (_rotateStarted)
			{
				_rotateStarted = false;
				_behaviour.StopCoroutine(_rotateCoroutine);
			}
			
			Rotation = value;
			_transform.localEulerAngles = Rotation;
		}
		public void SetRotation(float value)
		{
			SetRotation(new Vector3(Rotation.x, Rotation.y, value));
		}
	
		private Vector3 _scaleFrom;
		private Vector3 _scaleTo;
		private float _scaleDuration;
		private AnimationCurve _scaleCurve;

		private Vector3 _scaleScale;
		private IEnumerator ScaleCoroutine()
		{
			float scaleElapsed = 0f;
			if (_scaleDuration <= 0) _scaleDuration = Time.deltaTime;
			while (scaleElapsed < _scaleDuration)
			{
				scaleElapsed = Mathf.Clamp(scaleElapsed + Time.deltaTime, 0, _scaleDuration);
				_transform.localScale = Vector3.LerpUnclamped(_scaleFrom, _scaleTo, _scaleCurve.Evaluate(scaleElapsed / _scaleDuration));
				yield return null;
			}
			_scaleStarted = false;
		}
	
		private Coroutine _scaleCoroutine;
		private bool _scaleStarted;
		private void Scale()
		{
			if (!_transform.gameObject.activeInHierarchy) return;
		
			if (_scaleStarted)
				_behaviour.StopCoroutine(_scaleCoroutine);
			else
				_scaleStarted = true;
			_scaleCoroutine = _behaviour.StartCoroutine(ScaleCoroutine());	
		}

		public void Scale(Vector3 to)
		{
			_scaleFrom = _transform.localScale;
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = _defaultDuration;
			_scaleCurve = _defaultCurve;
			Scale();
		}
		public void Scale(Vector3 to, float duration)
		{
			_scaleFrom = _transform.localScale;
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = duration;
			_scaleCurve = _defaultCurve;
			Scale();
		}
		public void Scale(Vector3 to, float duration, AnimationCurve curve)
		{
			_scaleFrom = _transform.localScale;
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = duration;
			_scaleCurve = curve;
			Scale();
		}
		public void Scale(Vector3 from, Vector3 to)
		{
			_scaleFrom = _scaleScale;
			_scaleFrom.Scale(from);
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = _defaultDuration;
			_scaleCurve = _defaultCurve;
			Scale();
		}
		public void Scale(Vector3 from, Vector3 to, float duration)
		{
			_scaleFrom = _scaleScale;
			_scaleFrom.Scale(from);
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = duration;
			_scaleCurve = _defaultCurve;
			Scale();
		}
		public void Scale(Vector3 from, Vector3 to, float duration, AnimationCurve curve)
		{
			_scaleFrom = _scaleScale;
			_scaleFrom.Scale(from);
			_scaleTo = _scaleScale;
			_scaleTo.Scale(to);
			_scaleDuration = duration;
			_scaleCurve = curve;
			Scale();
		}

		public void Scale(float to) => Scale(Vector3.one * to);
		public void Scale(float to, float duration)
		{
			Scale(Vector3.one * to, duration);
		}
		public void Scale(float to, float duration, AnimationCurve curve)
		{
			Scale(Vector3.one * to, duration, curve);
		}
		public void Scale(float from, float to, float duration)
		{
			Scale(Vector3.one * from, Vector3.one * to, duration);
		}
		public void Scale(float from, float to, float duration, AnimationCurve curve)
		{
			Scale(Vector3.one * from, Vector3.one * to, duration, curve);
		}
		public void SetScale(Vector3 value)
		{
			if (_scaleStarted)
			{
				_scaleStarted = false;
				_behaviour.StopCoroutine(_scaleCoroutine);
			}

			_transform.localScale = value;
		}
		public void SetScale(float value)
		{
			if (value.Equals(0f))
				value = .001f;
			var val = _scaleScale;
			val.Scale(new Vector3(value, value, 1f));
			SetScale(val);
		}

		private class AnimateColor
		{
			public SpriteRenderer _sprite;
			public TextMesh _text;

			public AnimateColor(Transform transform)
			{
				_sprite = transform.GetComponent<SpriteRenderer>();
				_text = transform.GetComponent<TextMesh>();
			}

			public Color Get()
			{
				if (_sprite != null)
					return _sprite.color;
				return _text != null ? _text.color : Color.clear;
			}

			public bool Set(Color color)
			{
				if (_sprite != null)
					_sprite.color = color;
				else if (_text != null)
					_text.color = color;
				else
					return false;
				return true;
			}
		}

		public void SetVisible(bool value)
		{
			if (_color._sprite != null)
				_color._sprite.enabled = value;
			else if (_color._text != null)
				_color._text.GetComponent<MeshRenderer>().enabled = value;
		}
		public void SetOrder(int value)
		{
			if (_color._sprite != null)
				_color._sprite.sortingOrder = value;
			else if (_color._text != null)
				_color._text.GetComponent<MeshRenderer>().sortingOrder = value;
		}
	
		private Color _colorFrom;
		private Color _colorTo;
		private float _colorDuration;
		private AnimationCurve _colorCurve;

		private Color _colorColor;
		private IEnumerator ColorCoroutine()
		{
			float colorElapsed = 0f;
			if (_colorDuration <= 0) _colorDuration = Time.deltaTime;
			while (colorElapsed < _colorDuration)
			{
				colorElapsed = Mathf.Clamp(colorElapsed + Time.deltaTime, 0, _colorDuration);
				_colorColor = Color.LerpUnclamped(_colorFrom, _colorTo, _colorCurve.Evaluate(colorElapsed / _colorDuration));
				if (!_color.Set(_colorColor))
					break;
				yield return null;
			}
			_colorStarted = false;
		}
	
		private Coroutine _colorCoroutine;
		private bool _colorStarted;

		private void Colorize()
		{
			if (!_transform.gameObject.activeInHierarchy) return;
		
			if (_colorStarted)
				_behaviour.StopCoroutine(_colorCoroutine);
			else
				_colorStarted = true;
			_colorCoroutine = _behaviour.StartCoroutine(ColorCoroutine());	
		}

		public void Colorize(Color to)
		{
			_colorFrom = _colorColor;
			_colorTo = to;
			_colorDuration = _defaultDuration;
			_colorCurve = _defaultCurve;
			Colorize();
		}
		public void Colorize(Color to, float duration)
		{
			_colorFrom = _colorColor;
			_colorTo = to;
			_colorDuration = duration;
			_colorCurve = _defaultCurve;
			Colorize();
		}
		public void Colorize(Color to, float duration, AnimationCurve curve)
		{
			_colorFrom = _colorColor;
			_colorTo = to;
			_colorDuration = duration;
			_colorCurve = curve;
			Colorize();
		}
		public void Colorize(Color from, Color to)
		{
			_colorFrom = from;
			_colorTo = to;
			_colorDuration = _defaultDuration;
			_colorCurve = _defaultCurve;
			Colorize();
		}
		public void Colorize(Color from, Color to, float duration)
		{
			_colorFrom = from;
			_colorTo = to;
			_colorDuration = duration;
			_colorCurve = _defaultCurve;
			Colorize();
		}
		public void Colorize(Color from, Color to, float duration, AnimationCurve curve)
		{
			_colorFrom = from;
			_colorTo = to;
			_colorDuration = duration;
			_colorCurve = curve;
			Colorize();
		}
		public void Colorize(float to)
		{
			Colorize(new Color(_colorColor.r, _colorColor.g, _colorColor.b, to));
		}
		public void Colorize(float to, float duration)
		{
			Colorize(new Color(_colorColor.r, _colorColor.g, _colorColor.b, to), duration);
		}
		public void Colorize(float to, float duration, AnimationCurve curve)
		{
			Colorize(new Color(_colorColor.r, _colorColor.g, _colorColor.b, to), duration, curve);
		}
		public void Colorize(float from, float to, float duration)
		{
			Colorize(new Color(_colorColor.r, _colorColor.g, _colorColor.b, from), new Color(_colorColor.r, _colorColor.g, _colorColor.b, to), duration);
		}
		public void Colorize(float from, float to, float duration, AnimationCurve curve)
		{
			Colorize(new Color(_colorColor.r, _colorColor.g, _colorColor.b, from), new Color(_colorColor.r, _colorColor.g, _colorColor.b, to), duration, curve);
		}
		public Color GetColor()
		{
			return _color.Get();
		}
		public void SetColor(Color value)
		{
			if (_colorStarted)
			{
				_colorStarted = false;
				_behaviour.StopCoroutine(_colorCoroutine);
			}
		
			_colorColor = value;
			_color.Set(_colorColor);
		}
		public void SetColor(float value)
		{
			SetColor(new Color(_colorColor.r, _colorColor.g, _colorColor.b, value));
		}

		public void StopAll()
		{
			if (_moveStarted)
			{
				_moveStarted = false;
				_behaviour.StopCoroutine(_moveCoroutine);
			}
		
			if (_rotateStarted)
			{
				_rotateStarted = false;
				_behaviour.StopCoroutine(_rotateCoroutine);
			}

			if (_scaleStarted)
			{
				_scaleStarted = false;
				_behaviour.StopCoroutine(_scaleCoroutine);
			}

			if (_colorStarted)
			{
				_colorStarted = false;
				_behaviour.StopCoroutine(_colorCoroutine);
			}
		}
	}
}