using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MazeGameController : MonoBehaviour, ITask
{

	[SerializeField] private GameObject CollectablesParentObject;
	private int score;
	private int maxScore;
	[SerializeField] private GameObject ExitZone;
	[SerializeField] private Text DescriptionTextComponent;
	[SerializeField] private Text ScoreText;
	[SerializeField] private string DescriptionText;
	[SerializeField] private GameObject UIElements;
	
	// Use this for initialization
	void Start () {
		if (CollectablesParentObject != null)
		{
			maxScore = CollectablesParentObject.transform.childCount;
		}

		score = 0;
		ExitZone.SetActive(false);
		UIElements.SetActive(true);
		
		
		DescriptionTextComponent.text = DescriptionText;
		UpdateScoreText();
	}

	public void IncrementScore()
	{
		score++;
		UpdateScoreText();
		if (CheckTaskComplete())
		{
			ExitZone.SetActive(true);
		}
	}

	private void UpdateScoreText()
	{
		ScoreText.text = score + " / " + maxScore;
	}

	public GameObject GetExitZone()
	{
		return ExitZone;
	}
	
	public void TaskCompleted()
	{
		GameManager.GetInstance().CurrentGameFinished(this);
		UIElements.SetActive(false);
	}

	public bool CheckTaskComplete()
	{
		return score == maxScore;
	}
}
