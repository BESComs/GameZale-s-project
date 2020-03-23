using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using _Scripts.Utility;

public class OptionGamesManager : MonoBehaviour, ITask
{

	public static OptionGamesManager Instance { get; private set; }
	[SerializeField] private GameObject starsPanel;
	[SerializeField] private GameObject uiElementsParentGameObject;
	[SerializeField] private GameObject gameElementsGameObject;
	[SerializeField] private GameObject starsLayoutElementPrefab;
	[SerializeField] private Sprite starColored;
	[SerializeField] private Sprite starGrey;
	[SerializeField] private Text gameTextObject;
	public GameObject decorationsParentGameObject;
	[SerializeField] private OptionGameController[] games;
	[SerializeField] private AudioClip CorrectAnswerAudioClip;
	[SerializeField] private AudioSource SoundEffects;
	private GameObject[] starsBar;
	private int _currentGamePosition;
	[SerializeField] private GameObject particleSystemPlaceHolder;

	private void Awake()
	{
		if (Instance == null) {
			Instance = this;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_currentGamePosition = -1;
		uiElementsParentGameObject.SetActive(true);
		NextGame();
	}

	private void FixedUpdate()
	{
		if(Input.GetMouseButtonDown(0))
		{
			System.Diagnostics.Debug.Assert(Camera.main != null, "Camera.main != null");
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D hit = Physics2D.OverlapPoint(mousePosition);

			if (hit != null)
			{
				if (hit.transform.GetSiblingIndex() == GetCorrectAnswerOfCurrentQuestion())
				{
					GetCurrentGame().NextQuestion();
				}
				else
				{
					GetCurrentGame().WrongAnswer(hit.transform.gameObject);
				}
			}
		}
	}

	public void SetupStars(int numOfStars)
	{
		ClearStars();
		
		starsBar = new GameObject[numOfStars];
		for (int i = 0; i < numOfStars; i++)
		{
			starsBar[i] = Instantiate(starsLayoutElementPrefab);
			starsBar[i].GetComponent<Image>().sprite = starGrey;
			starsBar[i].transform.SetParent(starsPanel.transform, false);
		}
	}

	private void ClearStars()
	{
		if(starsPanel.transform.childCount == 0)
			return;
		foreach (Transform star in starsPanel.transform)
		{
			Destroy(star.gameObject);
		}

		starsBar = null;
	}

	public void SetupDecorations(Decoration[] decorations)
	{
		for (int i = 0; i < decorations.Length; i++)
		{
			GameObject ob = new GameObject("Decoration " + i);
			Decoration currentDecoration = decorations[i];
			Position2D currentDecorationPosition = currentDecoration.GetDecorationPositions();
			
			ob.transform.position = new Vector3(currentDecorationPosition.X, currentDecorationPosition.Y, 1);
			ob.AddComponent<SpriteRenderer>().sprite = currentDecoration.GetDecorationSprites();
			ob.GetComponent<SpriteRenderer>().sortingLayerName = "Interactive Shapes";

			if (GetCurrentGame().CustomScale > 0)
			{
				ob.transform.localScale = new Vector3(GetCurrentGame().CustomScale, GetCurrentGame().CustomScale, GetCurrentGame().CustomScale);
			}
			
			ob.transform.SetParent(decorationsParentGameObject.transform, false);
		}
	}

	private void ClearDecorations()
	{
		foreach (Transform childDecoration in decorationsParentGameObject.transform)
		{
			Destroy(childDecoration.gameObject);
		}
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
		ClearGameElements();
		ClearDecorations();
		_currentGamePosition++;
		if (CheckTaskComplete())
		{
			TaskCompleted();
			return;
		}
		SetupCurrentGame();
	}

	private void SetupCurrentGame()
	{
		if (_currentGamePosition > games.Length)
		{
			Debug.Log("Trying to setup game - " + _currentGamePosition + ". But there are only " + games.Length + " games.");
			return;
		}
		SetupTextPosition();
		games[_currentGamePosition].SetupGame(gameElementsGameObject);
		SetupDecorations(games[_currentGamePosition].Decorations);
	}

	private void ClearGameElements()
	{
		if (gameElementsGameObject.transform.childCount > 0)
		{
			foreach (Transform gameElement in gameElementsGameObject.transform)
			{
				Destroy(gameElement.gameObject);
			}
		}
	}

	public void SetQuestionText(string questionText)
	{
		gameTextObject.text = questionText;
	}

	public void PaintGreyStar(int index)
	{
		starsBar[index].GetComponent<Image>().sprite = starColored;
	}

	public void TaskCompleted()
	{
		uiElementsParentGameObject.SetActive(false);
		GameManager.GetInstance().CurrentGameFinished(this);
	}

	public bool CheckTaskComplete()
	{
		return _currentGamePosition >= games.Length;
	}

	private OptionGameController GetCurrentGame()
	{
		return games[_currentGamePosition];
	}

	private int GetCorrectAnswerOfCurrentQuestion()
	{
		return GetCurrentGame().NumberOfQuestions[GetCurrentGame().Score].CorrectAnswer;
	}

	public Vector3 GetCurrentCorrectAnswerPosition()
	{
		return gameElementsGameObject.transform.GetChild(GetCurrentGame().Score)
			.GetChild(GetCorrectAnswerOfCurrentQuestion()).localPosition;
	}

	private void SetupTextPosition()
	{
		(gameTextObject.transform as RectTransform).anchoredPosition = new Vector3(GetCurrentGame().GetQuestionTextPosition().X, GetCurrentGame().GetQuestionTextPosition().Y, 0);
	}

	public GameObject ParticleSystemPlaceholder
	{
		get {return particleSystemPlaceHolder;}
		set { particleSystemPlaceHolder = value; }
	}

	public void PlayCorrectAnswerAudio()
	{
		SoundEffects.PlayOneShot(CorrectAnswerAudioClip);
	}
	
	public void DestroyGameObjectOrComponent(Object ob)
	{
		Destroy(ob);
	}

	public GameObject InstantiateGameObject(GameObject ob)
	{
		return Instantiate(ob);
	}
}