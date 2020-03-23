using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BookShelfController : MonoBehaviour
{

	public Transform bookPrefab;

	public int booksCount;
	public float shelfWidth;
	
	public float limitDistance;
	public float limitAngle;
	public float zoomDistance;

	public float transitionTime = 0.5f;

	private Vector3 moveTarget;
	public float moveSpeed = 2f ;

	public bool isDragging;
	private Vector3 dragStartPoint;
	private Plane dragPlane;
	
	private List<Transform> books = new List<Transform>();

	[SerializeField] private Material[] materials;

	private int selected;
	private void Start()
	{
		moveTarget = transform.localPosition;
		selected = 1;
	}

	void Update ()
	{
		UpdateTransform();
		AdjustBooksCount();
		UpdateBooksPosition();
		UpdateBooksParams();

		if (isDragging)
			UpdateDrag();

		kapalak.position = moveTarget;
	}

	public void AdjustBooksCount()
	{
		while (books.Count > booksCount)
		{
			var lastBookIndex = books.Count - 1; 
			var book = books[lastBookIndex];
			GameObject.Destroy(book.gameObject);
			books.RemoveAt(lastBookIndex);
		}

		while (books.Count < booksCount)
		{
			var newBook = GameObject.Instantiate(bookPrefab);
			newBook.parent = transform;
			newBook.GetComponentInChildren<Renderer>().material = materials[books.Count];
			books.Add(newBook);
		}
	}

	public void UpdateBooksPosition()
	{
		var booksListCount = books.Count - 1;

		for (int i = 0; i <= booksListCount; i++)
		{
			var book = books[i];
			float bookPosition = -shelfWidth + shelfWidth * i / booksListCount * 2;
			book.localPosition = new Vector3(bookPosition, transform.localPosition.y, transform.localPosition.z);
		}
	}


	public void UpdateBooksParams()
	{
		foreach (var book in books)
		{
			var bk = book.GetComponent<BookRotator>();
			bk.limitAngle = limitAngle;
			bk.limitDistance = limitDistance;
			bk.zoomDistance = zoomDistance;
		}
	}

	public void UpdateTransform()
	{
		transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
	}


	public void UpdateDrag()
	{
		float rd;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		dragPlane.Raycast(ray, out rd);
		var currentDragPoint = ray.GetPoint(rd);
		moveTarget = Vector3.left * (dragStartPoint.x - currentDragPoint.x);
		moveTarget.y = transform.localPosition.y;
		moveTarget.z = transform.localPosition.z;
	}


	public void SelectBook(BookRotator book)
	{
		var bookIndex = books.IndexOf(book.transform);
		selected = bookIndex;
		var booksShift = shelfWidth * 2 / (booksCount - 1);

		var newShelfPosition = -shelfWidth + bookIndex * booksShift;

		Debug.Log("work", this);

		moveTarget = Vector3.right * newShelfPosition;
		moveTarget.y = transform.localPosition.y;
		moveTarget.z = transform.localPosition.z;
		
	}

	public Transform kapalak;

	public void BeginDrag()
	{
		isDragging = true;
		RaycastHit raycastHit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out raycastHit, 10))
		{
			dragStartPoint = raycastHit.point;
			dragPlane = new Plane(Vector3.forward, dragStartPoint);
		}
	}

	public void EndDrag()
	{
		isDragging = false;
	}


	public void OpenText(GameObject o)
	{
		var can = GameObject.Find("Canvas");
		can.transform.GetChild(selected).gameObject.SetActive(true);
		o.SetActive(false);
	}
	
	
	
	

}