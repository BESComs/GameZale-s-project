using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
	[Serializable]
	public class NumWMGConstructor
	{
		[SerializeField] private NumIntroduction _introduction;
		[SerializeField] private NumMiniGames _miniGames;
		[SerializeField] private NumFindObjects _findObjects;
		[SerializeField] private NumEndGame _endGameScene;
		
		public void InitCurrent()
		{
			switch (gameController.CurrentScene)
			{
				case 0 : _introduction.Init(); break;
				case 1 : _miniGames.Init(); break;
				case 2 : _findObjects.Init(); break;
				case 3 : _endGameScene.Init(); break;
			}
		}

		public async Task FinishCurrent()
		{
			gameController.StopCorrectAnswerParticle();
			switch (gameController.CurrentScene)
			{
				case 0 : await _introduction.Finish(); break;
				case 1 : _miniGames.Finish(); break;
				case 2 : _findObjects.Finish(); break;
				case 3 : _endGameScene.Finish(); break;
			}
		}
		
		private NumWMGGameController gameController => NumWMGGameController.Instance;
		public IValidatable MiniGame => _miniGames.ChosenGame();
		public IValidatable FindObjects => _findObjects;
	}
}
