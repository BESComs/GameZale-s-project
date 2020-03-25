using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Difference
{
    public class ClickEvent : MonoBehaviour
    {
        //проверка находится ли курсор над данным обьектом
        public bool mouseOver;
        public int id;
        private void OnMouseEnter()
        {
            mouseOver = true;
        }

        private void OnMouseExit()
        {
            mouseOver = false;
        }
    }
}
