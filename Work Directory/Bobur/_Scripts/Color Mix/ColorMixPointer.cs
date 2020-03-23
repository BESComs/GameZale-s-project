using UnityEngine;
using _Scripts.Color_Mix;

namespace Work_Directory.Bobur._Scripts.Color_Mix
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ColorMixPointer : MonoBehaviour
	{
		[SerializeField] private Sprite sprite;
		[SerializeField] private Sprite colorSprite;

		private PipetteColor pipetteColor;
		private ColorObject chosenColorObject;
		private SpriteRenderer spriteRenderer;
		private bool inColorMixer;

		private void Start()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = "Top Layer";
			spriteRenderer.sortingOrder = 3;
			inColorMixer = false;

			SetColorSprite();
		}

		private async void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				ColorMixGameController.Instance.SetPipetteToMousePosition();
				await new WaitForUpdate();
				await new WaitForUpdate();
				if(chosenColorObject != null)
				{
					Color color = chosenColorObject.GetColor();
					pipetteColor.SetColor(color);
				}
				else if (inColorMixer && !ColorMixGameController.Instance.isAnimating)
				{
					ColorMixGameController.Instance.GetColorMixer().AddColor(pipetteColor.GetColor());
				}
			}
		}

		private void SetColorSprite()
		{
			GameObject ob = new GameObject("Pipette Color");
			pipetteColor = ob.AddComponent<PipetteColor>();
			pipetteColor.Initialize(gameObject, colorSprite);
		}

		public void SetColorObject(ColorObject colorObject) => chosenColorObject = colorObject;

		public void SetPosition(Vector3 position) => transform.localPosition = position;

		public void SetInColorMixer(bool value) => inColorMixer = value;
	}

	class PipetteColor : MonoBehaviour
	{
		private SpriteRenderer _spriteRenderer;
		private ColorMixPointer _parent;

		public void Initialize(GameObject parent, Sprite colorSprite)
		{
			_parent = parent.GetComponent<ColorMixPointer>();
			transform.SetParent(parent.transform);
			transform.localPosition = Vector3.zero;
			_spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			_spriteRenderer.sprite = colorSprite;
			_spriteRenderer.sortingLayerName = "Top Layer";
			_spriteRenderer.sortingOrder = 4;
			gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;
			gameObject.AddComponent<Rigidbody2D>().isKinematic = true;
			SetColor(Color.clear);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.GetComponent<ColorObject>() != null)
			{
				_parent.SetColorObject(other.GetComponent<ColorObject>());
			}
			else if(other.GetComponent<ColorMixer>() != null)
			{
				_parent.SetInColorMixer(true);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			_parent.SetColorObject(null);
			_parent.SetInColorMixer(false);
		}

		public void SetColor(Color color) => _spriteRenderer.color = color;
		public Color GetColor() => _spriteRenderer.color;
	}
}
