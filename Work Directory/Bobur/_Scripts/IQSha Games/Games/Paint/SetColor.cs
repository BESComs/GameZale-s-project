using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    //устанавливает цвет элементу раскраски
    public class SetColor : MonoBehaviour
    {
        //id текущего элемента
        [HideInInspector]public int currentColorId;
        //правильный id
        public int correctColorId;
        [HideInInspector]public SpriteRenderer sr;

        private void Awake()
        {
            currentColorId = -1;
            sr = GetComponent<SpriteRenderer>();
        }
    }
}
