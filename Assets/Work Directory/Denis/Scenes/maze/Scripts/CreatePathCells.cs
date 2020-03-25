using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Work_Directory.Denis.Scenes.maze.Scripts
{
    //скрипт для автоматического создания ячеек в игре "BeeMaze" 
    public class CreatePathCells : MonoBehaviour
    {
        public List<Transform> cells;
        public Transform cellPrefab;
        public int line, column;
        public float radius;
        public float margin;
        public Vector2 startPos;
        [Button]
        public void TmpFunc()
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }
        
        [Button]
        public void CreateCells()
        {
            var currentPos = startPos;
            for (var i = 0; i < line; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    currentPos.x += radius + margin;
                    cells.Add(Instantiate(cellPrefab,currentPos,new Quaternion(),transform));
                }
                currentPos.x = startPos.x;
                currentPos.y += radius + margin;
            }
        }
        
        [Button]
        public void AddComponentOnCell()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.AddComponent<PathPointController>();
            }
        }
        
        [Button]
        public void DestroyCells()
        {
            var n = transform.childCount;
            for (int i = 0; i < n; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        } 
    }
}
