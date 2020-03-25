using UnityEngine;

namespace Work_Directory.Denis.Scenes.maze.Scripts
{
    public class PathPointController : MonoBehaviour
    {
        //является ли текущая ячейка финальной
        public bool finalCell;
        //текущий индекс
        public (int, int) currentCellIndex;
        //может ли игрок пойти на эту ячейку когда либо 
        public bool activeCell;
        //может ли игрок пойти на эту ячейку в текущем ходе
        public bool canMove;
        //trigger для проигрывания анимации
        public bool playAnimation;
        private SpriteRenderer spriteRenderer;
        //является ли текущая ячейка стартовой
        public bool startCell;
        private Vector2 startScale;
        
        private void Awake()
        {
            startScale = transform.localScale;
            spriteRenderer = GetComponent<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        //проигрывание анимации
        public async void PlayAnimation()
        {
            var timer = .5f;
            var sign = -1f;
            if(this == null) return;
            var spriteRendererColor = spriteRenderer.color;
            while (playAnimation)
            {
                if(this == null) return;
                timer += sign * Time.deltaTime;
                if (timer < 0)
                    sign = 1f;
                else if (timer > .4)
                    sign = -1f;
                transform.localScale = startScale * (1f - timer);
                spriteRendererColor.a = 2f * timer;
                spriteRenderer.color = spriteRendererColor;
                await new WaitForUpdate();
            }
            spriteRendererColor.a = 1f;
            spriteRenderer.color = spriteRendererColor;
            transform.localScale = startScale;
        }

        private void OnMouseDown()
        {
            /*
             если на указанную ячейку можно пойти и Maze Game Manager не выполняет ассинхронных операци
             и указанная ячейка не является текущей то выключаются все ячейки игрок перемещается
             в указанную ячейку и активируются все необходимые относительно указанной
            */
            if (!canMove || MazeGameManager.Instance.inTask || MazeGameManager.Instance.currentCell == this) return;
            MazeGameManager.Instance.DisableAllCells(this);
            //выключает startArrow при первом ходе
            if (MazeGameManager.Instance.gameStart) return;
            MazeGameManager.Instance.startArrow.gameObject.SetActive(false);
            MazeGameManager.Instance.gameStart = true;
        }
    }
}
