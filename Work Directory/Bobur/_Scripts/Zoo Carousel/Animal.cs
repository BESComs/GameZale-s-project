using UnityAsync;
using UnityEngine;
using _Scripts.Utility.Drag;

namespace _Scripts.Zoo_Carousel
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Animal : MonoBehaviour
    {
        [SerializeField] private int _id;
        private SpriteRenderer _spriteRenderer;
        private Animal _collidedAnimal;
        private ZooCarouselGameController gameController;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            gameController = ZooCarouselGameController.Instance;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Animal>() != null)
            {
                _collidedAnimal = other.gameObject.GetComponent<Animal>();
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            _collidedAnimal = null;
        }

        private void OnMouseUp()
        {
            if (_collidedAnimal != null && _collidedAnimal.Id == _id)
            { 
                gameController.Constructor().AnimalMatched(_id - 1);
            }
        }

        public async void FadeIn()
        {
            _spriteRenderer.color = new Color(1f,1f,1f, 0f);
            for (float i = 0; i < 1f; i += Time.deltaTime / 1.2f)
            {
                i = (i > 1f) ? 1f : i;
                _spriteRenderer.color = new Color(1f,1f,1f, i);
                await Await.NextUpdate();
            }
        }

        public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;
        
        public int Id => _id;
    }
}