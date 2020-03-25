using UnityEngine;

namespace _Scripts.Sudoku
{
	public class Board : MonoBehaviour
	{
		[SerializeField] private GameObject cellsParent;
		public GameObject CellsParent => cellsParent;
	}
}
