using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames.MiniGames.Draw_Number_Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PaintCell : MonoBehaviour
    {
        [SerializeField] private Color _paintedColor;
        [SerializeField] private Sprite _tileSprite;
        [SerializeField] private bool _painted;

        private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

        private void Start()
        {
            gameObject.AddComponent<Clickable>();
        }

        private void OnValidate()
        {
            UpdateColor();
            spriteRenderer.sprite = _tileSprite;
        }
        
        public void OnClick()
        {
            _painted = !_painted;
            UpdateColor();
        }

        public Clickable GetClickable()
        {
            return GetComponent<Clickable>() ?? gameObject.AddComponent<Clickable>();
        }

        public bool IsPainted => _painted;

        public void SetColliderEnabled(bool value) => GetComponent<BoxCollider2D>().enabled = value;
        
        private void UpdateColor() => spriteRenderer.color = (_painted) ? _paintedColor : Color.white;
    }
}