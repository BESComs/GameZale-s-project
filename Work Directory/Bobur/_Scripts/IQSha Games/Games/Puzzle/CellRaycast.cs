using System.Collections.Generic;
using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Puzzle
{
    public class CellRaycast : MonoBehaviour
    {
        //поиск самого верхнего элемента в указанной точке
        //основной скрипт
        public Puzzle puzzle;
        //все элементы пазла
        public List<Cell> Cells = new List<Cell>();
        private Camera mainCamera;
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!puzzle.canMove || !Input.GetKeyDown(KeyCode.Mouse0)) return;
            //var hit = new RaycastHit2D[49];
            var tmp = Input.mousePosition;
            tmp.z = 5f;
            //пускает луч в указанную позицию
            
            var colliders = new Collider2D[30];
            
            if (Physics2D.OverlapCircleNonAlloc(mainCamera.ScreenToWorldPoint(tmp), .1f, colliders) <= 0) return;
            
            Cell cell1 = null;
            CellMoveController cellMove = null;
            var maxSir = -1;
            //поиск самого верхнего элемента из всех элементов в который попал луч
            foreach (var collider in colliders)
            {
                if(collider == null || !collider.transform) continue;
                var asd = collider.transform.GetComponent<CellMoveController>();
                if (!asd || maxSir >= asd.spriteRenderer.sortingOrder) continue;
                asd.previousMousePosition = Input.mousePosition;
                maxSir = asd.spriteRenderer.sortingOrder;
                cell1 = asd.CurrentCell;
                cellMove = asd;
            }
            
            if (cell1 == null) return;
            //устанавливает нужные элементы на верх

            cellMove.initialSelfPosition = cellMove.transform.position;
            cellMove.initialMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            cell1.CellInMove = 2;
            puzzle.CellsOnTop(cell1);
        }
    }
}
