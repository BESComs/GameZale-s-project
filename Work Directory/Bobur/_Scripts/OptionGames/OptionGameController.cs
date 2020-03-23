using System;
using System.Collections.Generic;
using DefaultNamespace;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using _Scripts.Utility;

[Serializable]
public class OptionGameController : ITask
{
	[SerializeField] private Option[] numberOfQuestions;
	[SerializeField] private AnimatorOverrideController animatorOverrideController;
	[SerializeField] private Decoration[] _decorations;
	[SerializeField] private Position2D[] _targetPositionsOfQuestionSprites;
	[SerializeField] private Position2D _questionTextPosition;
	[SerializeField] public float CustomScale;
	[SerializeField] private AnimationType _animationType;
	[SerializeField] private AnimationClip _idleAnimClip;
	[SerializeField] private GameObject _correctAnswerParticleEffect;
	
	//TODO Create class for questions entity to eliminate expensive methods invocation
	//Question class contains: Animator, Collider2D, SpriteRenderer, ShakeObject
	private List<GameObject> _questions = new List<GameObject>();

	private int score;
	
	// Use this for initialization
	public OptionGameController()
	{
		score = 0;
	}

	async void EnterSceneEffect()
	{
		string inAnimName = _animationType + "In";
		foreach (Transform question in _questions[score].transform)
		{
			question.GetComponent<Animator>().SetBool(_animationType + "In", true);
		}
		await Task.Delay(TimeSpan.FromSeconds(animatorOverrideController[inAnimName].length));
		
		foreach (Transform question in _questions[score].transform)
		{
			question.gameObject.AddComponent<BoxCollider2D>();
		}
	}
	
	
	public async Task ExitSceneEffect()
	{
		foreach (Transform question in _questions[score].transform)
		{
			question.GetComponent<Collider2D>().enabled = false;
		}
		
		OptionGamesManager.Instance.PlayCorrectAnswerAudio();
		
		await CorrectAnswerEffect(OptionGamesManager.Instance.GetCurrentCorrectAnswerPosition());
		
		string outAnimName = _animationType + "Out";
		foreach (Transform question in _questions[score].transform)
		{
			question.GetComponent<Animator>().SetBool(outAnimName, true);
		}

		await Task.Delay(TimeSpan.FromSeconds(animatorOverrideController[outAnimName].length));
	}
	
	public async Task CorrectAnswerEffect(Vector3 correctAnswerPosition)
	{
		Assert.IsNotNull(OptionGamesManager.Instance.ParticleSystemPlaceholder, "Particle System not installed");
		OptionGamesManager.Instance.ParticleSystemPlaceholder.transform.GetChild(0).position = correctAnswerPosition;
		OptionGamesManager.Instance.ParticleSystemPlaceholder.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
		
		await Task.Delay(TimeSpan.FromSeconds(_correctAnswerParticleEffect.GetComponent<ParticleSystem>().main.duration));
	}

	public void WrongAnswer(GameObject ob)
	{
		foreach (Transform question in _questions[score].transform)
		{
			question.GetComponent<Collider2D>().enabled = false;
		}
		
		ob.AddComponent<ShakeObject>().Shake(this);
	}

	public void EnableCurrentQuestionColliders()
	{
		foreach (Transform question in _questions[score].transform)
		{
			question.GetComponent<Collider2D>().enabled = true;
		}
	}
	
	public Option[] NumberOfQuestions
	{
		get { return numberOfQuestions; }
	}

	public Position2D[] TargetPositionsOfQuestionSprites
	{
		get { return _targetPositionsOfQuestionSprites; }
	}

	public AnimatorOverrideController AnimatorController
	{
		get { return animatorOverrideController; }
	}

	public Decoration[] Decorations
	{
		get { return _decorations; }
	}

	public int Score
	{
		get { return score; }
	}

	public void SetupGame(GameObject parentGameObject)
	{
		if (OptionGamesManager.Instance.ParticleSystemPlaceholder.transform.childCount > 0)
		{
			foreach (Transform child in OptionGamesManager.Instance.ParticleSystemPlaceholder.transform)
			{
				OptionGamesManager.Instance.DestroyGameObjectOrComponent(child.gameObject);
			}
		}
		
		OptionGamesManager.Instance.InstantiateGameObject(_correctAnswerParticleEffect).transform.SetParent(OptionGamesManager.Instance.ParticleSystemPlaceholder.transform);
		
		animatorOverrideController["Idle"] = _idleAnimClip;
		
		for(int j = 0; j < numberOfQuestions.Length; j++)
		{
			GameObject questionParentObject = new GameObject("Question " + j);
			questionParentObject.transform.SetParent(parentGameObject.transform);
			_questions.Add(questionParentObject);
			
			for (int i = 0; i < numberOfQuestions[j].Options.Length; i++)
			{
				GameObject questionOption = new GameObject();
				
				SpriteRenderer spriteRenderer = questionOption.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = numberOfQuestions[j].Options[i];
				spriteRenderer.sortingLayerName = "Interactive Shapes";

				questionOption.AddComponent<Animator>().runtimeAnimatorController = animatorOverrideController;
				
				questionOption.transform.SetParent(questionParentObject.transform);
				Vector3 questionPosition = new Vector3(_targetPositionsOfQuestionSprites[i].X, _targetPositionsOfQuestionSprites[i].Y, 1);
				questionOption.transform.position = questionPosition;
			}

			if (j > 0)
			{
				questionParentObject.SetActive(false);
			}
			
			if (CustomScale > 0)
			{
				questionParentObject.transform.localScale = new Vector3(CustomScale, CustomScale, CustomScale);
			}
		}
		EnterSceneEffect();
		
		OptionGamesManager.Instance.SetQuestionText(CurrentQuestion().QuestionText);
		OptionGamesManager.Instance.SetupStars(numberOfQuestions.Length);
	}

	public async void NextQuestion()
	{
		Assert.IsNotNull(_questions, "No question was set up");
		OptionGamesManager.Instance.PaintGreyStar(score);

		await ExitSceneEffect();
		
		_questions[score].SetActive(false);

		score++;
		
		if(CheckTaskComplete()){
			TaskCompleted();
			return;
		}
		
		OptionGamesManager.Instance.SetQuestionText(CurrentQuestion().QuestionText);
		_questions[score].SetActive(true);
		EnterSceneEffect();
	}

	public void TaskCompleted()
	{
		OptionGamesManager.Instance.CurrentGameFinished(this);
	}

	public bool CheckTaskComplete()
	{
		return score >= numberOfQuestions.Length;
	}

	public Position2D GetQuestionTextPosition()
	{
		return _questionTextPosition;
	}

	private Option CurrentQuestion()
	{
		return NumberOfQuestions[score];
	}
}

[Serializable]
public enum AnimationType
{
	Fade,
	Bounce,
	Zoom,
	Roll
}
