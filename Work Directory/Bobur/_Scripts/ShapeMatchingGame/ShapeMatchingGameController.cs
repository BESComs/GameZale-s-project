using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class ShapeMatchingGameController : MonoBehaviour, ITask
{

	public static ShapeMatchingGameController Instance { get; private set; }
	public GameObject deskPanel;
	public GameObject shapesParentObject;
	public List<GameObject> shapePrefabs;
	public Color[] colors;
	private int score;
	public Button continueButton;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	void Start ()
	{
		score = 0;
		SetupShapes();
		SetupEmptyShapes();
	}
	
	private void SetupShapes()
	{
		ShuffleList(shapePrefabs);
		Vector3 pos = new Vector3(-4, 3.2f, 0);
		for (int i = 0, k = 0; i < shapePrefabs.Count; i++, k++)
		{
			if (k > 4)
			{
				k = 0;
				pos.x = 4f;
				pos.y = 3.2f;
			}
			GameObject instantiatedObject = Instantiate(shapePrefabs[i]);
			instantiatedObject.transform.SetParent(shapesParentObject.transform, false);
			instantiatedObject.GetComponent<ShapeController>().role = Roles.ACTIVE;
			instantiatedObject.GetComponent<SpriteRenderer>().color = colors[i];
			instantiatedObject.transform.position = pos;
			pos.y -= 1.5f;
		}
	}

	private void SetupEmptyShapes()
	{
		//Shuffle shapes first
		ShuffleList(shapePrefabs);
		//Setup elements on Desk Panel
		Vector3 pos = new Vector3(-1.5f, 1.5f, 0);
		for (int i = 0, k = 0; i < shapePrefabs.Count; i++, k++)
		{
			if (k > 2)
			{
				k = 0;
				pos.x = -1.5f;
				pos.y -= 1.5f;
			}
			GameObject instantiatedObject = Instantiate(shapePrefabs[i]);
			instantiatedObject.transform.SetParent(deskPanel.transform, false);
			instantiatedObject.GetComponent<ShapeController>().role = Roles.PASSIVE;
			instantiatedObject.transform.position = pos;
			pos.x += 1.5f;
		}
	}

	public void IncrementScore()
	{
		score++;
		if (CheckTaskComplete())
		{
			continueButton.gameObject.SetActive(true);
		}
	}
	
	private void ShuffleList<T>(List<T> list)
	{
		int n = list.Count;
		System.Random rng = new System.Random();
		
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public void TaskCompleted()
	{
		GameManager.GetInstance().CurrentGameFinished(this);
		continueButton.gameObject.SetActive(false);
	}

	public bool CheckTaskComplete()
	{
		return shapePrefabs.Count == score;
	}
}
