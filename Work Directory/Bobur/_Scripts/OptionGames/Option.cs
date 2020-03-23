using System;
using UnityEngine;

[Serializable]
public class Option
{
	[SerializeField] private Sprite[] options;
	[SerializeField] private int correctAnswer;
	[SerializeField] private String questionText;

	public string GetQuestionText()
	{
		return questionText;
	}
	
	public Sprite[] Options
	{
		get { return options; }
	}

	public int CorrectAnswer
	{
		get { return correctAnswer; }
	}

	public string QuestionText
	{
		get { return questionText; }
	}
	
}