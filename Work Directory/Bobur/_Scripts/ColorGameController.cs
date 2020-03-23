using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work_Directory;
using Button = UnityEngine.UI.Button;

public class ColorGameController : MonoBehaviour, ITask, ILessonStatsObservable
{

	private int currentLevelPos;
	[SerializeField] private GameObject UIElements;
	[SerializeField] private TextMeshPro _mainTextObject;
	[SerializeField] private Button _previousButton;
	[SerializeField] private Button _nextButton;
	[SerializeField] private Button _checkButton;
	[SerializeField] private Button _reloadButton;
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private string[] Texts;
	[SerializeField] private int _questionnireStartIndex;
	[SerializeField] private GameObject[] _questionnaireCorrectGameObjects;
	[SerializeField] private int SubtractElements;
	private GameObject _chosenObject;
	private GameObject _hoveredObject;
	public bool IsAnimating { get; set; }

	// Use this for initialization
	void Start()
	{
		UIElements.SetActive(true);
		_checkButton.gameObject.SetActive(false);
		AddBoxCollidersToClickableObjects();
		Initialize();
	}

	private void Update()
	{
		RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
		
		if (hit.collider && hit.collider.CompareTag("Ignore")) return;
		
		hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		
		if (hit.collider != null)
		{
			if (hit.collider.gameObject != _hoveredObject || _hoveredObject == null)
			{
				if (_chosenObject == null || hit.collider.gameObject != _chosenObject)
				{
					_hoveredObject = hit.collider.gameObject;
					_hoveredObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
				}
			}
		}
		else if (_hoveredObject != null)
		{
			_hoveredObject.transform.localScale = Vector3.one;
			_hoveredObject = null;
		}
		
		if(Input.GetMouseButtonDown(0))
		{
			if (hit.collider != null)
			{
				if (_chosenObject != null)
				{
					if (_chosenObject != hit.collider.gameObject)
					{
						_chosenObject.GetComponent<SpriteRenderer>().color -= new Color(0,0,0, 0.5f);
						_chosenObject.transform.localScale = Vector3.one;
						_chosenObject = hit.collider.gameObject;
						_chosenObject.GetComponent<SpriteRenderer>().color = Color.white;
						_hoveredObject = null;
					}
					else
					{
						foreach (Transform colorSprite in CurrentLevelObject.transform)
						{
							colorSprite.GetComponent<SpriteRenderer>().color = Color.white;
						}
						_chosenObject = null;
					}
				}
				else
				{
					_chosenObject = hit.collider.gameObject;
					_hoveredObject = null;
					foreach (Transform colorSprite in CurrentLevelObject.transform)
					{
						if(colorSprite.gameObject != _chosenObject)
							colorSprite.GetComponent<SpriteRenderer>().color -= new Color(0,0,0, 0.5f);
					}
				}
			}
			else
			{
				if (_chosenObject)
				{
					_chosenObject.transform.localScale = Vector3.one;
					_chosenObject = null;
					foreach (Transform colorSprite in CurrentLevelObject.transform)
					{
						colorSprite.GetComponent<SpriteRenderer>().color = Color.white;
					}
				}
			}
		}  
	}

	private void RevertChosenAndHoveredObjects()
	{
		_hoveredObject = null;
		_chosenObject = null;
		foreach (Transform colorSprite in CurrentLevelObject.transform)
		{
			colorSprite.localScale = Vector3.one;
			colorSprite.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}
	
	void Initialize()
	{
		currentLevelPos = -1;
		_previousButton.interactable = false;
		_reloadButton.gameObject.SetActive(false);
		_nextButton.interactable = true; 
		nextImg();
	}
	
	private void AddBoxCollidersToClickableObjects()
	{
		for (int i = _questionnireStartIndex; i < transform.childCount - SubtractElements; i++)
		{
			foreach (Transform clickableObject in transform.GetChild(i))
			{
				clickableObject.gameObject.AddComponent<BoxCollider2D>();
			}
		}
	}

	public void nextImg()
	{
		if(currentLevelPos == 13)
			RegisterLessonEnd();
		currentLevelPos++;
		if (CheckTaskComplete())
		{
			_reloadButton.gameObject.SetActive(true);
			_nextButton.interactable = false;
			_particleSystem.Clear();
			_mainTextObject.text = "";
			transform.GetChild(currentLevelPos - 1).gameObject.SetActive(false);
			_previousButton.interactable = false;
			return;
		}

		if(currentLevelPos == _questionnireStartIndex)
			RegisterLessonStart();
		if (currentLevelPos >= _questionnireStartIndex)
		{
			RevertChosenAndHoveredObjects();
			_particleSystem.Clear();
			_nextButton.gameObject.SetActive(false);
			_checkButton.gameObject.SetActive(true);
		}
		
		if (currentLevelPos > 0)
		{
			transform.GetChild(currentLevelPos - 1).gameObject.SetActive(false);
			_previousButton.interactable = true;
		}

		_mainTextObject.text = Texts[currentLevelPos];
		CurrentLevelObject.SetActive(true);
	}

	private GameObject CurrentLevelObject => transform.GetChild(currentLevelPos).gameObject;


	public void Previous()
	{
		transform.GetChild(currentLevelPos--).gameObject.SetActive(false);
		transform.GetChild(currentLevelPos).gameObject.SetActive(true);
		_mainTextObject.text = Texts[currentLevelPos];
		
		if (currentLevelPos >= _questionnireStartIndex)
		{
			_nextButton.gameObject.SetActive(false);
			_checkButton.gameObject.SetActive(true);
			return;
		}
		
		_nextButton.gameObject.SetActive(true);
		_checkButton.gameObject.SetActive(false);
		
		if (currentLevelPos == 0)
			_previousButton.interactable = false;
	}

	public void CheckIfCorrectSelected()
	{
		if (_chosenObject != null)
		{
			if (_chosenObject == _questionnaireCorrectGameObjects[currentLevelPos - _questionnireStartIndex])
			{
				RegisterAnswer(true);
				_particleSystem.transform.localPosition = _chosenObject.transform.localPosition * 0.75f;
				_particleSystem.Play();
				_nextButton.gameObject.SetActive(true);
				_checkButton.gameObject.SetActive(false);
			}
			else
			{
				RegisterAnswer(false);
				if(_chosenObject.GetComponent<ShakeObject>() != null)
					return;
				
				_chosenObject.AddComponent<ShakeObject>().Shake();
			}
		}
	}

	public void ReloadGame()
	{
		Initialize();
	}
	
	public bool CheckTaskComplete()
	{
		if (currentLevelPos >= transform.childCount - SubtractElements)
		{
			return true;
		}

		return false;
	} 

	public void TaskCompleted()
	{
		GameManager.GetInstance().CurrentGameFinished(this);
		UIElements.SetActive(false);
	}

	public int MaxScore { get => _questionnaireCorrectGameObjects.Length; set {  } }
	public void RegisterAnswer(bool isAnswerRight){
		if (!isAnswerRight) return;
		LessonStatistic.SetScore(1);
		LessonStatistic.SetRight(isAnswerRight);
		
	}

	public void RegisterLessonStart(){
		LessonStatistic.SetStartLessonTime();
	}
	
	public void RegisterLessonEnd(){
		LessonStatistic.SetLessonDurationWithEndLessonTime();
		ServerRequests.PostStatistics();
	}
	public void OnApplicationPause(){
		LessonStatistic.SetLessonDurationWithEndLessonTime();
	}
}
