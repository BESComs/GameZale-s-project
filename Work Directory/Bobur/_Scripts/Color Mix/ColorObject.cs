using UnityEngine;

namespace _Scripts.Color_Mix
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ColorObject : MonoBehaviour
	{
		[SerializeField] private ColorsEnum color;
		private SpriteRenderer spriteRenderer;
	
		// Use this for initialization
		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		public Color GetColor() => Colors.EnumToColor(color);
		public ColorsEnum ColorEnum => color;
	}
}
