using UnityEngine;

namespace Work_Directory.Denis.Mozaika
{
    //скрипт отвечающий за выбора цвета для покраски
    public class MazaikaColorCellController : MonoBehaviour
    {
        public Color cellColor;

        private void Awake()
        {
            GetComponent<SpriteRenderer>().color = cellColor;
        }

        private void OnMouseDown()
        {
            MazaikaManager.Instance.GetColor(this);
        }
    }
}
