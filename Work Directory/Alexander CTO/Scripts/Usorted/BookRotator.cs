using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookRotator : MonoBehaviour
{


	public float limitDistance;
	private float leftLimitDistance;
	private float rightLimitDistance;

	public float limitAngle;

	public float zoomDistance;

	private float mousePressedTime;
	
	void Update ()
	{
		leftLimitDistance = -limitDistance;
		rightLimitDistance = limitDistance;
		UpdateRotationAngles();
	}


	public void UpdateRotationAngles()
	{
		float x = transform.position.x;
		x = Mathf.Clamp(x, leftLimitDistance, rightLimitDistance);
		x = x / limitDistance;
		var yRotation  = x * limitAngle + -90;
		transform.rotation = Quaternion.Euler(0, yRotation, 0);
		var zPosition = -zoomDistance * (1 - Mathf.Abs(x));
		var pos = transform.position;
		pos.z = zPosition;
		transform.position = pos;
	}


	private void OnMouseDown()
	{
		mousePressedTime = Time.time;
		//BeginDrag();
	}

	private void OnMouseUp()
	{
		
		if (Time.time - mousePressedTime < 0.2f)
			transform.parent.GetComponentInParent<BookShelfController>().SelectBook(this);
		
		EndDrag();
	}

	public void BeginDrag()
	{
		transform.parent.gameObject.GetComponent<BookShelfController>().BeginDrag();
	}


	public void EndDrag()
	{
		transform.parent.gameObject.GetComponentInParent<BookShelfController>().EndDrag();
	}
	
	
}