using Sirenix.OdinInspector;
using UnityEngine;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    //Скрипт используется для автоматического разброса по сцене игр и категорий
    public class TreeCreator : MonoBehaviour
    {
        //отступы элементов в списках игр и списках категорий 
        public Vector2 gamesMargin, categoriesMargin;
        //количество столбцев для разброса категорий по сетке
        public int columnNumber;
        //Стартовые позиции списка игр и списка категорий
        public Vector2 gameStartPos, categoriesStartPos;
        //обьект содержащий все игры
        public Transform games;
        //объект содержащий все категории
        public Transform categories;
#if UNITY_EDITOR
        //Элементы разбрасываются слева направо сверху вниз
        
        /*
         * Разбрасывает игры на сцене в два столбика
         * столбики находятся на растоянии 2 * gamesMargin.x - 1
         * соседнии строки находятся на растоянии gamesMargin.y
         * первый элемент находится на позиции gameStartPos
         */
        [Button]
        public void ThrowOnSceneGamesList()
        {  
            for (var i = 0; i < games.childCount; ++i)
            for (var j = 0; j < games.GetChild(i).childCount; ++j)
                games.GetChild(i).GetChild(j).localPosition = new Vector3(
                    gameStartPos.x + (j % 2 == 0 ? -gamesMargin.x : gamesMargin.x - 1),
                    gameStartPos.y - j / 2 * gamesMargin.y, 0);
        }

        /*
         * Разбрасывает категории по сетке в columnNumber столбцов
         * с отступами categories Margin.y по вертикали и categories Margin.x по горизонтали
         */
        [Button]
        public void GridCategories()
        {
            var currentPos = categoriesStartPos;
            for (var i = 0; i < categories.childCount; i++)
            {
                categories.GetChild(i).localPosition = currentPos;
                currentPos.x += categoriesMargin.x;
                if ((i + 1) % columnNumber != 0) continue;
                currentPos.y -= categoriesMargin.y;
                currentPos.x = categoriesStartPos.x;
            }
        }
        
        //Выравнивает список игр по списку категорий для корректной анимации
        [Button]
        public void SetPos()
        {
            if (games.childCount != categories.childCount)
            {
                Debug.Log("Something wrong");
                return;
            }
            for (var i = 0; i < games.childCount; i++)
            {
                games.GetChild(i).localPosition = categories.GetChild(i).localPosition;
            }
        }
#endif
    }
}