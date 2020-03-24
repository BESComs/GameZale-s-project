using UnityEngine;

namespace Work_Directory.Denis.Scenes.Pazzle
{
    public class PuzzleCellController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        //правильная\стартовая позиция
        public Vector3 correctPos, startPos;
        //id элемента пазла
        public int id;
        //находится в правильной позиции
        public bool inCorrectPos;
        //движется к стартовой позиции/ к правильной позиции
        public bool moveToStartPos, moveToCorrectPos;
        [SerializeField] private new Camera camera;
        //в движении
        public bool inMove;
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            startPos = transform.position;
            camera = Camera.main;
        }

        private void OnMouseDown()
        {
            if (inCorrectPos) return;
            inMove = true;
            _spriteRenderer.sortingOrder = 3;
        }
        private void Update()
        {
            var position = transform.position;
            //движение за курсором
            if (inMove)
            {
                var pos = camera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                transform.position = pos;
            }
            //плавное движение к правильной позиции
            if (moveToCorrectPos)
            {
                if ((position - correctPos).magnitude > .1f)
                {
                    position += Time.deltaTime * 10f * (correctPos - position).normalized;
                    transform.position = position;
                }
                else
                {
                    moveToCorrectPos = false;
                    transform.position = correctPos;
                    _spriteRenderer.sortingOrder = 2;
                }
            }
            //плавное движение к стартовой позиции
            else if(moveToStartPos)
            {
                if ((position - startPos).magnitude > .1f)
                {
                    position += Time.deltaTime * 10f * (startPos - position).normalized;
                    transform.position = position;
                }
                else
                {
                    moveToStartPos = false;
                    transform.position = startPos;
                    _spriteRenderer.sortingOrder = 2;
                }
            }
            //проверка позиции
            if(!inMove || !Input.GetKeyUp(KeyCode.Mouse0) || inCorrectPos) return;
            SeaPuzzleManager.Instance.CheckPos(this);
        }
    }
}
