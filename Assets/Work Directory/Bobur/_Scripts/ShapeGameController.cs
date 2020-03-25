using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class ShapeGameController : MonoBehaviour, ITask {

	private int currentImgPos;
	public Button nextButton;

	// Use this for initialization
	void Start()
	{
		currentImgPos = 0;
		nextButton.gameObject.SetActive(true);
		nextImg();
	}

	public void nextImg()
	{
		if (currentImgPos > 0)
		{
			transform.GetChild(currentImgPos - 1).gameObject.SetActive(false);
		}

		if (currentImgPos >= transform.childCount)
		{
			TaskCompleted();
			return;
		}

		transform.GetChild(currentImgPos++).gameObject.SetActive(true);

	}

	public bool CheckTaskComplete()
	{
		if (currentImgPos == transform.childCount)
		{
			currentImgPos = 0;
			nextButton.gameObject.SetActive(false);
			return true;
		}

		return false;
	}

	public void TaskCompleted()
	{
		GameManager.GetInstance().CurrentGameFinished(this);
	}
}
