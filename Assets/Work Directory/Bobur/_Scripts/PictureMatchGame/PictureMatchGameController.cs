using System;
using System.Threading;
using DefaultNamespace;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.PictureMatchGame
{
	public class PictureMatchGameController : MonoBehaviour, ITask
	{
		public static PictureMatchGameController Instance { get; private set; }
		
		[SerializeField] private PictureMatchGame[] Games;
		[SerializeField] private Position2D[] _pieces;
		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private GameObject UIElements;
		private int _currentGame;
		
		// Use this for initialization
		void Start () {
			if (Instance == null)
			{
				Instance = this;
			}
			
			UIElements.SetActive(true);
			
			SetupGame();
		}

		private void SetupGame()
		{
			for (int i = 0; i < Games.Length; i++)
			{
				GameObject gameParentObject = new GameObject($"Picture Match {i}");
				Games[i].SetupShapes();
				gameParentObject.transform.SetParent(this.transform);
				Games[i].GetPiecesParentObject().transform.SetParent(gameParentObject.transform);
				gameParentObject.SetActive(i == 0);
			}
		}

		public Position2D[] GetPositions()
		{
			return _pieces;
		}

		public void CheckCurrentGame()
		{
			if (Games[_currentGame].CheckTaskComplete())
			{
				_particleSystem.Play();
			}
				
		}

		public void TaskCompleted()
		{
			throw new NotImplementedException();
		}

		public bool CheckTaskComplete()
		{
			throw new NotImplementedException();
		}
	}
}
