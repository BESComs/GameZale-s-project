using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Sudoku
{
	[Serializable]
	public class Level : ITask
	{
		private GameObject levelParentObject;
		
		[Header("Board")] [SerializeField] private Position2D boardPosition;

		[Header("Cells on the board")] [SerializeField]
		private Row[] rows;

		[Header("Input objects")] [SerializeField]
		private CellConstructor[] inputObjects;
		private int moves;
		
		public Board Board { get; private set; }
		private SudokuGameController _gameController;
		
		public void Init(SudokuGameController gameController, int index)
		{
			_gameController = gameController;
			levelParentObject = new GameObject("Level " + index);
			levelParentObject.transform.SetParent(gameController.transform);
			
			Board = gameController.Create(gameController.BoardPrefab).GetComponent<Board>();
			Board.transform.SetParent(levelParentObject.transform);
			Board.transform.localPosition = new Vector3(boardPosition.X, boardPosition.Y);

			int id = 0;
			for (int i = 0; i < rows.Length; i++)
			{
				GameObject row = new GameObject("Row " + (i + 1));
				row.transform.SetParent(Board.CellsParent.transform, false);
				for (int j = 0; j < rows[i].Cells.Length; j++)
				{
					Cell cell = gameController.Create(gameController.CellPrefab).GetComponent<Cell>();
					cell.Type = rows[i].Cells[j].Type;
					
					cell.transform.SetParent(row.transform);
					cell.transform.localPosition =
						new Vector3(rows[i].Cells[j].Position.X, rows[i].Cells[j].Position.Y);
					cell.SetupSprite();
					cell.SpriteOrderInLayer(0);
					
					cell.ID = rows[i].Cells[j].ID = id; // Assigning ID

					if (cell.Type != CellTypeEnum.Placeholder)
						cell.DisableColliders();
					else
						moves++;
					
					id++;
				}
			}
			
			GameObject inputsParent = new GameObject("Inputs");
			inputsParent.transform.SetParent(Board.transform, false);
			for (int i = 0; i < inputObjects.Length; i++)
			{
				Cell inputCell = gameController.Create(gameController.CellPrefab).GetComponent<Cell>();
				inputCell.Type = inputObjects[i].Type;
				inputCell.transform.SetParent(inputsParent.transform);
				inputCell.transform.localPosition = new Vector3(inputObjects[i].Position.X, inputObjects[i].Position.Y);
				inputCell.SetupSprite();
				inputCell.AddPolygonCollider();
				inputCell.SpriteOrderInLayer(1);
				inputCell.ID = -1;
			}
		}

		public void AddObjectToBoard(CellTypeEnum cellType, Vector3 position)
		{
			Cell obj = _gameController.Create(_gameController.CellPrefab).GetComponent<Cell>();
			obj.Type = cellType;
			obj.transform.SetParent(Board.transform, false);
			obj.transform.localPosition = position;
			obj.SetupSprite();
			obj.SpriteOrderInLayer(1);
		}

		public void MoveIncrement()
		{ 
			moves++;
		}

		public async void MoveDecrement()
		{
			moves--;
			
			if (moves <= 0)
			{
				if (CheckTaskComplete())
				{
					await _gameController.PlayParticle();
					_gameController.CurrentLevelCompleted();
				}
			}
		}
		
		public CellConstructor GetCell(int id)
		{
			for (int i = 0; i < rows.Length; i++)
			{
				if (rows[i].FindCell(id) != null)
					return rows[i].FindCell(id);
			}

			return null;
		}
	
		public void DisableLevel() => levelParentObject.SetActive(false);
		public void EnableLevel() => levelParentObject.SetActive(true);
		
		public void TaskCompleted()
		{
			throw new NotImplementedException();
		}

		public bool CheckTaskComplete()
		{
			return CheckHorizontal() && CheckVertical() && CheckGroup();
		}
		
		private bool CheckHorizontal()
		{
			HashSet<CellTypeEnum> set = new HashSet<CellTypeEnum>();
			for (int i = 0; i < rows.Length; i++)
			{
				for (int j = 0; j < rows[i].Cells.Length; j++)
				{
					CellTypeEnum cell = rows[i].Cells[j].Type;
					
					if (set.Contains(cell) || cell == CellTypeEnum.Placeholder)
					{
						return false;
					}
					
					set.Add(cell);
				}
				
				set.Clear();
			}
			return true;
		}

		private bool CheckVertical()
		{
			HashSet<CellTypeEnum> set = new HashSet<CellTypeEnum>();
			for (int i = 0; i < rows.Length; i++)
			{
				for (int j = 0; j < rows[i].Cells.Length; j++)
				{
					CellTypeEnum cell = rows[j].Cells[i].Type;

					if (set.Contains(cell))
					{
						return false;
					}

					set.Add(cell);
				}

				set.Clear();
			}
			return true;
		}

		private bool CheckGroup()
		{
			HashSet<CellTypeEnum> set = new HashSet<CellTypeEnum>();
			int max = Convert.ToInt32(Mathf.Sqrt(rows.Length));

			for (int i = 0, group = 0, row = 0; i <= rows.Length; i++)
			{
				if (i > 0 && i % max == 0)
				{
					group++;
					
					if (group % max == 0)
					{
						group = 0;
						row++;
					}
					
					i = row * max;
					
					if(i >= rows.Length)
						break;

					set.Clear();
				}
				
				int startPos = group * max;
				for (int j = startPos; j < startPos + max; j++)
				{
					CellTypeEnum cell = rows[i].Cells[j].Type;
					if (set.Contains(cell) || cell == CellTypeEnum.Placeholder)
					{
						return false;
					}

					set.Add(cell);
				}
			}
			return true;
		}
	}

	[Serializable]
	class Row
	{
		[SerializeField] private CellConstructor[] cells;
		public CellConstructor[] Cells => cells;

		public CellConstructor FindCell(int id)
		{
			for (int i = 0; i < cells.Length; i++)
			{
				if (cells[i].ID == id)
					return cells[i];
			}
			
			return null;
		}
	}
}
