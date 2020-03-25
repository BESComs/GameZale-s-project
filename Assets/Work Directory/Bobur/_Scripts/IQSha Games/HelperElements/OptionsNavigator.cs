using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements
{
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
    public class OptionsNavigator : MonoBehaviour
    {
        [SerializeField, ColorPalette("Options Navigator Colors")]
        private Color onHoverColor;
    
        [SerializeField, ColorPalette("Options Navigator Colors")]
        private Color onChooseColor;
    
        [SerializeField, ColorPalette("Options Navigator Colors")]
        private Color onIdleColor;
        
        [SerializeField, ColorPalette("Options Navigator Colors")]
        private Color onIncorrectColor;

        public bool isPainted { get; private set; }
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        private Action<OptionsNavigator> onMouseDown;
        public bool isDisabled;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();

            spriteRenderer.color = onIdleColor;
        }

        private void OnMouseEnter()
        {
            if (!isPainted)
            {
                Hover();
            }
        }

        private void OnMouseExit()
        {
            if (!isPainted)
            {
                Clear();
            }
        }

        private void OnMouseDown()
        {
            if(isDisabled)
                return;
            
            if (onMouseDown != null)
            {
                onMouseDown.Invoke(this);
            }

            if (isPainted)
            {
                Clear();
            }
            else
            {
                Paint();
            }
        }

        public void Paint()
        {
            spriteRenderer.color = onChooseColor;
            isPainted = true;
        }
        public void Hover() => spriteRenderer.color = onHoverColor;
        public void Clear()
        {
            spriteRenderer.color = onIdleColor;
            isPainted = false;
        }

        public void Incorrect() => spriteRenderer.color = onIncorrectColor; 
        
        public void SetSize(float x, float y)
        {
            spriteRenderer.size = new Vector2(x, y);
            boxCollider.size = new Vector2(x,y);
        }

        public void SetOnMuseDown(Action<OptionsNavigator> action) => onMouseDown = action;
        public Color GetOnIdleColor() => onIdleColor;
        public void SetOnHidleColor(Color idleColor) => onIdleColor = idleColor;
    }
}
