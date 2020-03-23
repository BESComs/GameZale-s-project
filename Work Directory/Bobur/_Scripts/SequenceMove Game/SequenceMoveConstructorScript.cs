using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.SequenceMove_Game
{
	[Serializable]
	public class SequenceMoveConstructorScript : ITask
	{
		[SerializeField] private GameObject _playerPrefab;
		[SerializeField] private GameObject _moveTargetSpot;
		[SerializeField] private float WaitTimeForAnimations;
		[SerializeField] private Position2D _playerStartPosition;
		[SerializeField] private Position2D _playerEndPosition;
		[SerializeField] private Position2D[] MoveSequence;
		public GameObject ParentGameObject { get; private set; }
		public GameObject PlayerGameObject { get; private set; }

		public int CurrentTarget { get; private set; }

		SequenceMoveConstructorScript()
		{
			CurrentTarget = 0;
		}

		public void Initialize()
		{
			ParentGameObject = new GameObject();
			PlayerGameObject = Instantiate(_playerPrefab);
			PlayerGameObject.transform.SetParent(ParentGameObject.transform);
			PlayerGameObject.transform.localPosition = new Vector3(_playerStartPosition.X, _playerStartPosition.Y, 1);

			for (int i = 0; i < MoveSequence.Length; i++)
			{
				GameObject currentSpot = Instantiate(_moveTargetSpot);
				currentSpot.GetComponent<TargetSpot>().ID = i;
				currentSpot.name = "Spot " + (i + 1);
				currentSpot.transform.SetParent(ParentGameObject.transform);
				currentSpot.transform.localPosition = new Vector3(MoveSequence[i].X, MoveSequence[i].Y, 1);
				currentSpot.GetComponent<TargetSpot>().SetNumber(i + 1);
			}
		}

		private GameObject Instantiate(GameObject ob)
		{
			if (SequenceMoveGameController.Instance == null)
				return null;
			return SequenceMoveGameController.Instance.CreateInstance(ob);
		}

		public async Task NextTarget()
		{
			CurrentTarget++;
			if (CheckTaskComplete())
			{
				Vector3 endPosition = new Vector3(_playerEndPosition.X, _playerEndPosition.Y, 1);
				await Task.Delay(TimeSpan.FromSeconds(WaitTimeForAnimations));
				PlayerGameObject.GetComponent<PlayerScript>().Jump(endPosition);
				await Task.Delay(TimeSpan.FromSeconds(WaitTimeForAnimations * 2));
				TaskCompleted();
			}
		}
		
		public async void TaskCompleted()
		{
			if(ParentGameObject == null || ParentGameObject.transform == null) return;
			SequenceMoveGameController.Instance.RegisterAnswer(true);
			for (int i = 0; i < ParentGameObject.transform.childCount; i++)
			{
				var current = ParentGameObject.transform.GetChild(i);

				if (i == ParentGameObject.transform.childCount - 1)
				{
					await AnimationUtility.ScaleOut(current.gameObject, initialScale: current.localScale.x);
				}
				else
				{
					AnimationUtility.ScaleOut(current.gameObject, initialScale: current.localScale.x);
				}
			}

			await Task.Delay(TimeSpan.FromSeconds(0.3f));
			
			GameObject.Destroy(ParentGameObject);
			CurrentTarget = 0;
			//ParentGameObject.SetActive(false);
			SequenceMoveGameController.Instance.NextLevel();
		}

		public bool CheckTaskComplete()
		{
			return MoveSequence.Length == CurrentTarget;
		}
	}
}
