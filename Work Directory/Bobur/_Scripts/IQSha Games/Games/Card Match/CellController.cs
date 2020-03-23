using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match
{
    public class CellController : MonoBehaviour
    {
        //текущая карта
        public Cell currentCell;
        //основной скрипт
        public CardMatch mainScript;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private async void Update()
        {
            //основной скрипт ожидает выполнение операции или карта перевернута
            if (!Input.GetKeyDown(KeyCode.Mouse0) || currentCell.cellInTask || mainScript.LastTwoCellsIndex.Count == 2 ||
                mainScript.Checking || currentCell.inverted)
                return;
            
            
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
            //если курсор находится над картой
            if (Mathf.Abs(mousePos.x - currentCell.cellPos.x) < currentCell.cellSize.x / 2f &&
                Mathf.Abs(mousePos.y - currentCell.cellPos.y) < currentCell.cellSize.y / 2f) 
                //повернуть карту
                await mainScript.RotateCell(currentCell);
            
        }
    }
}