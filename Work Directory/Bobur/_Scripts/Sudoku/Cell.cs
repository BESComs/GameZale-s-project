using System;
using UnityAsync;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Sudoku
{
	public class Cell : MonoBehaviour
	{
		private CircleCollider2D circleCollider => GetComponent<CircleCollider2D>();
		private PolygonCollider2D polygonCollider;
		public CellTypeEnum Type { get; set; }
		private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
		
		private SudokuGameController gameController	=> SudokuGameController.Instance;
		public int ID { get; set; }
		
		// Variables for using when drag
		private Vector3 startPosition;
		private Cell collidedObject;
		private Cell placeholderCell;
		private bool toDestroy;
		
		private void OnMouseDown()
		{
			if (Type != CellTypeEnum.Placeholder)
			{
				startPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
				SpriteOrderInLayer(2);
			}
		}

		private void OnMouseDrag()
		{
			if(Type != CellTypeEnum.Placeholder)
			{
				Vector3 pos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
				transform.position = pos;
			}
		}

		private void OnMouseUp()
		{
			if(Type != CellTypeEnum.Placeholder)
			{
				if (toDestroy)
				{
					Destroy(gameObject);
					placeholderCell.EnableColliders();
					placeholderCell.Type = CellTypeEnum.Placeholder;
					
					gameController.CurrentLevel.MoveIncrement();
					return;
				}
				
				if (collidedObject != null)
				{
					MoveTo(collidedObject.transform.localPosition);
					placeholderCell = collidedObject;
					placeholderCell.DisableColliders();

					gameController.CurrentLevel.GetCell(placeholderCell.ID).Type = Type;
					
					gameController.CurrentLevel.AddObjectToBoard(Type, startPosition);
					gameController.CurrentLevel.MoveDecrement();
					
					toDestroy = true;
				}
				else
				{
					MoveTo(startPosition);
				}
				
				SpriteOrderInLayer(1);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if(other is CircleCollider2D)
			{
				if (other.gameObject.GetComponent<Cell>() && other.gameObject.GetComponent<Cell>().Type == CellTypeEnum.Placeholder)
				{
					collidedObject = other.gameObject.GetComponent<Cell>();
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if(other.gameObject.GetComponent<Cell>().Type == CellTypeEnum.Placeholder)
				collidedObject = null;
		}

		private async void MoveTo(Vector3 target)
		{
			Vector3 initialPos = transform.localPosition;
			for (float i = 0f; i < 1f; i += Time.deltaTime * 5)
			{
				i = (i > 1f) ? 1f : i;
				transform.localPosition = Vector3.Lerp(initialPos, target, i);
				await Await.NextUpdate();
			}
		}
		
		public void DisableColliders()
		{
			foreach (Collider2D collider in GetComponents<Collider2D>())
				collider.enabled = false;
		}
		
		public void EnableColliders()
		{
			foreach (Collider2D collider in GetComponents<Collider2D>())
				collider.enabled = true;
		}

		public void SetupSprite()
		{
			Sprite sprite;
			switch (Type)
			{
				case CellTypeEnum.Type_1: sprite = gameController.TypeSprites[1]; break;
				case CellTypeEnum.Type_2: sprite = gameController.TypeSprites[2]; break;
				case CellTypeEnum.Type_3: sprite = gameController.TypeSprites[3]; break;
				case CellTypeEnum.Type_4: sprite = gameController.TypeSprites[4]; break;
				default: sprite = gameController.TypeSprites[0]; break;
			}
			
			SetSprite(sprite);
			spriteRenderer.sortingLayerName = "Interactive Shapes";
		}

		public void AddPolygonCollider() => polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
		public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
		public void SpriteOrderInLayer(int index) => spriteRenderer.sortingOrder = index;
	}

	[Serializable]
	public class CellConstructor
	{
		[SerializeField] private CellTypeEnum type;
		[SerializeField] private Position2D position;
		
		
		public int ID { get; set; }
		
		public CellTypeEnum Type
		{
			get => type;
			set => type = value;
		} 
		
		public Position2D Position => position;
	}
	
	[Serializable]
	public enum CellTypeEnum
	{
		Placeholder,
		Type_1,
		Type_2,
		Type_3,
		Type_4
	}
}