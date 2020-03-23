using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Puzzle
{
    //движение элемента пазла
    public class CellMoveController : MonoBehaviour
    {
        public Cell CurrentCell;
        public Vector3 previousMousePosition;
        private bool moveCell;
        public Puzzle puzzle;
        public SpriteRenderer spriteRenderer { get; private set; }

        public Vector3 initialMousePosition;
        public Vector3 initialSelfPosition;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //движение при зажатии мыши
        //при отпуске проверка на соединение
        private void Update()
        {
            if (!puzzle.canMove || CurrentCell.CellInMove != 2) return;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                CurrentCell.CellInMove = 0;
                puzzle.ConnectCell(CurrentCell);
                puzzle.lastSortingOrder++;
            }
            else if(Input.GetKey(KeyCode.Mouse0))
            {
                puzzle.MoveCells(this, previousMousePosition);
                previousMousePosition = Input.mousePosition;
            }
        }
    }
}
