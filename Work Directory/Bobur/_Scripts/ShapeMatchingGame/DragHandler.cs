using UnityEngine;

public class DragHandler : MonoBehaviour
{
	private Vector3 originalPosition;
	private Vector3 previousMousePosition;
	private bool movingBack;
	private ShapeController _shapeController;
	
	// Use this for initialization
	void Start ()
	{
		_shapeController = GetComponent<ShapeController>();
		movingBack = false;
	}

	void Update()
	{
		if (movingBack)
		{
			float step = 15 * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, originalPosition, step);
			if (transform.position == originalPosition)
			{
				movingBack = false;
				_shapeController.SetSortingLayer(0);
			}
		}
	}
	
	private void OnMouseDown()
	{
		if (_shapeController.role == Roles.ACTIVE)
		{
			DisableTriggerOfUnnecessaryEmptyObjects();
			originalPosition = transform.position;
			previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_shapeController.SetSortingLayer(1);
		}
	}

	public void OnMouseDrag()
	{
		if (_shapeController.role == Roles.ACTIVE)
		{
			Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - previousMousePosition;
			transform.position += distance;
			previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	public void OnMouseUp()
	{
		if (_shapeController.role == Roles.ACTIVE)
		{
			EnableTriggerOfEmptyObjects();
			GameObject collidedGameObject = _shapeController.CollidedGameObject;
			if (collidedGameObject != null && _shapeController.ID == collidedGameObject.GetComponent<ShapeController>().ID)
			{
				transform.position = collidedGameObject.transform.position;
				ShapeMatchingGameController.Instance.IncrementScore();
				_shapeController.SetSortingLayer(0);
				_shapeController.role = Roles.PASSIVE;
				_shapeController.disableTrigger();
				Destroy(collidedGameObject);
				return;
			}
			movingBack = true;
		}
	}

	private void DisableTriggerOfUnnecessaryEmptyObjects()
	{
		GameObject desk = ShapeMatchingGameController.Instance.deskPanel;
		foreach (Transform emptyObject in desk.transform)
		{
			if (emptyObject.GetComponent<ShapeController>().ID != _shapeController.ID)
				emptyObject.gameObject.GetComponent<ShapeController>().disableTrigger();
		}
	}
	
	private void EnableTriggerOfEmptyObjects()
	{
		GameObject desk = ShapeMatchingGameController.Instance.deskPanel;
		foreach (Transform emptyObject in desk.transform)
		{
				emptyObject.GetComponent<ShapeController>().enableTrigger();
		}
	}
}
