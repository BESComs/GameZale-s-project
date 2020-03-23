using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Scripts.Spinning_Wheel;

public class SpinningWheelController : MonoBehaviour
{

	[SerializeField] private AnimationCurve _spinAnimationCurve;
	
	private bool _isSpinning;
	private float _anglePerItem;
	
	private int _randomTime;
	private float _timer;
	
	private int _itemNumber;
	private float _maxAngle;
	private float _startAngle;
	
	// Use this for initialization
	void Start ()
	{
		_isSpinning = false;
		_anglePerItem = 360f / SpinningWheelGameController.Instance.GetObjectsCount();
		_timer = 0f;
	}

	private void FixedUpdate()
	{
		if (_isSpinning)
		{
			if (_timer < _randomTime)
			{
				float angle = _maxAngle * _spinAnimationCurve.Evaluate(_timer / _randomTime);
				transform.eulerAngles = new Vector3(0f, 0f, angle + _startAngle);
				_timer += Time.deltaTime;
			}
			else
			{
				SpinningWheelGameController.Instance.SetOptionButtonsInteractable(true);
				_isSpinning = false;
				_timer = 0;
			}
		}
	}

	public void Spin()
	{
		SpinningWheelGameController.Instance.SetSpinButtonInteractable(false);
		SpinningWheelGameController.Instance.SetOptionButtonsInteractable(false);
		_randomTime = Random.Range(5, 6);
		_itemNumber = Random.Range(0, SpinningWheelGameController.Instance.GetObjectsListCount());
		
		float distance = SpinningWheelGameController.Instance.GetObjectsCount() - (SpinningWheelGameController.Instance.GetPositionFromListByIndex(_itemNumber) - SpinningWheelGameController.Instance.GetCurrentPosition());
		print($"target: {SpinningWheelGameController.Instance.GetPositionFromListByIndex(_itemNumber)} cur: {SpinningWheelGameController.Instance.GetCurrentPosition()} distance: {distance}");
		
		_maxAngle = 360 + (distance * _anglePerItem);
		_isSpinning = true;
		_startAngle = transform.eulerAngles.z;
	}
}
