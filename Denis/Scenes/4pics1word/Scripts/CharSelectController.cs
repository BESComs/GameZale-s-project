using UnityEngine;

namespace Work_Directory.Denis.Scenes._4pics1word.Scripts
{
    //скрипт отвечает за выбор пользователем букв
    public class CharSelectController : MonoBehaviour
    {
        [HideInInspector] public int childId;
        [HideInInspector] public char curChar;
        [HideInInspector] public FourWordOnePicture fourWordOnePicture;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        }

        private void OnMouseEnter()
        {
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = 1f;
            _spriteRenderer.color = spriteRendererColor;
        }

        private void OnMouseExit()
        {
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = .5f;
            _spriteRenderer.color = spriteRendererColor;
        }

        private void OnMouseDown()
        {
            if(!fourWordOnePicture.inTask)
                fourWordOnePicture.AddChar(curChar, childId);
        }
    }
}
