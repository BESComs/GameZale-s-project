using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Riddle_Game
{
	[Serializable]
	public class Option
	{
		[SerializeField] private Sprite sprite;
		[SerializeField] private int ID;

		public Sprite GetSprite() => sprite;
		public int GetID() => ID;
	}

	public class OptionObject : MonoBehaviour
	{
		private SpriteRenderer _spriteRenderer;
		public int ID { get; set; }
		private RiddleGameController _parent;
		
		void Start()
		{
			gameObject.AddComponent<BoxCollider2D>();
			_spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			_spriteRenderer.sortingLayerName = "Interactive Shapes";
		}

		private void OnMouseDown()
		{
			_parent.ObjectClicked(ID, transform.localPosition);
		}

		public void RecalculateBoxCollider()
		{	
			Vector2 newSize = _spriteRenderer.sprite.bounds.size;
			GetComponent<BoxCollider2D>().size = newSize;
			GetComponent<BoxCollider2D>().offset = new Vector2(0,0);
			GetComponent<BoxCollider2D>().isTrigger = true;
			GetComponent<BoxCollider2D>().isTrigger = false;
		}
		
		public void SetParent(RiddleGameController parent) => _parent = parent;
		public SpriteRenderer GetSpriteRenderer() => _spriteRenderer;
		public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;
	}
}
