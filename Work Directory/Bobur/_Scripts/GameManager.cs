using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	private GameManager() {}
	private int currentGamePos;

	private void Awake()
	{
		Assert.IsNull(instance, "GameManager can have only one instance.");
		if(instance == null)
			instance = this;
	}
	
	private void Start()
	{
		SetupCurrentGamePosition();
		
		GameObject toLauncher = Resources.LoadAsync("ToLauncher Button", typeof(GameObject)).asset as GameObject;
		GameObject canvas = GameObject.Find("Canvas");
		var tmp = FindObjectsOfType<Canvas>();
		if (tmp.Length != 0)
		{
			canvas = tmp[tmp.Length - 1].gameObject;
		}
		if (toLauncher)
		{
			Instantiate(toLauncher, canvas.transform);
		}
		
		
		NextGame();
	}

	public static GameManager GetInstance()
	{
		Assert.IsNotNull(instance, "GameManager does not have an instance.");
		return instance;
	}

	public void CurrentGameFinished(ITask game)
	{
		if (game.CheckTaskComplete())
			NextGame();
		else
			Debug.Log("Finish this game first!");
	}

	private void NextGame()
	{
		if (currentGamePos == 0)
		{
			transform.GetChild(currentGamePos++).gameObject.SetActive(true);
			return;
		}
		else if (currentGamePos >= transform.childCount)
		{
			transform.GetChild(currentGamePos - 1).gameObject.SetActive(false);
			Debug.Log("All games finished");
			return;
		}

		transform.GetChild(currentGamePos - 1).gameObject.SetActive(false);
		transform.GetChild(currentGamePos++).gameObject.SetActive(true);
	}

	private void SetupCurrentGamePosition()
	{
		currentGamePos = 0;
		foreach (Transform child in transform)
		{
			if (child.gameObject.activeSelf)
			{
				currentGamePos = child.GetSiblingIndex();
				return;
			}
		}
	}
}

