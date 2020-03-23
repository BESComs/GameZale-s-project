using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Spinning_Wheel
{
	public class SpinningWheelGameController : MonoBehaviour, ITask
	{
		public static SpinningWheelGameController Instance { get; private set; }
		[SerializeField] private GameObject _objectsParent;
		private List<int> numbers = new List<int>();

		[SerializeField] private SpinningWheelController _spinningWheel;
		[SerializeField] private Button SpinButton;
		[SerializeField] private Button[] OptionButtons;

		[SerializeField] private Sprite[] OptionImages;
		[SerializeField] private GameObject UIELements;
		
		private int _currentPosition;
		private ObjectIdentifier currentObject;

		void Awake()
		{
			Instance = this;
			InstallNumbers();
		}
		
		// Use this for initialization
		void Start () {
			_currentPosition = 0;
			SetOptionButtonsInteractable(false);
			UIELements.SetActive(true);
		}

		private void InstallNumbers()
		{
			foreach (Transform ob in _objectsParent.transform)
			{
				numbers.Add(ob.GetComponent<ObjectIdentifier>().GetPosition());
			}
		}

		public int GetObjectsCount()
		{
			return _objectsParent.transform.childCount;
		}
		
		public int GetObjectsListCount()
		{
			return numbers.Count;
		}

		public void SetSpinButtonInteractable(bool value)
		{
			SpinButton.interactable = value;
		}

		public void SetOptionButtonsInteractable(bool value)
		{
			foreach (Button button in OptionButtons)
			{
				button.interactable = value;
			}
		}

		public void ChooseOptionButtonClick(int option)
		{
			if (option == currentObject.GetCorrectMatch())
			{
				currentObject.SetCurrentObjectOption(OptionImages[option - 1]);
				SetOptionButtonsInteractable(false);
				RemoveCurrentPositionFromList();

				if (CheckTaskComplete())
				{
					TaskCompleted();
					return;
				}
				
				SetSpinButtonInteractable(true);
			}
		}

		public void SetCurrentObject(ObjectIdentifier ob)
		{
			currentObject = ob;
		}
		
		public void SetCurrentPosition(int newPos)
		{
			_currentPosition = newPos;
		}
		
		public int GetCurrentPosition()
		{
			return _currentPosition;
		}

		public int GetPositionFromListByIndex(int index)
		{
			return numbers[index];
		}

		public void RemoveCurrentPositionFromList()
		{
			numbers.Remove(_currentPosition);
		}

		public void TaskCompleted()
		{
			GameManager.GetInstance().CurrentGameFinished(this);
			UIELements.SetActive(false);
		}

		public bool CheckTaskComplete()
		{
			return numbers.Count == 0;
		}
	}
}
