#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    //Автозаполнение игровых иконок в сцене Games Launcher
    public class AddIcons : MonoBehaviour
    {
        //Префикс пути откуда будут браться спрайты
        private const string PathPrefix = "Assets/Work Directory/Denis/Resources/GameIcons/";
        //Объект на сцене содержащий все игры
        public Transform gameHolder;
        
        [Button]
        public void AddGameIcons()
        {
            //загрузка всех спрайтов
            var icons = Resources.LoadAll<Sprite>("/");
            //индекс дочерних объектов gameHolder-а
            var childCounter = 0;
            //индекс дочерних объектов дочернего объекта gameHolder-а
            var counterInChild = 0;
            foreach (var sprite in icons)
            {
                //путь к файлу
                var spritePath = AssetDatabase.GetAssetPath(sprite);
                //проверка корректен ли путь
                if(PathPrefix.Where((t, i) => spritePath[i] != t).Any())
                    continue;
                //объект на котором нужно заменить спрайт
                gameHolder.GetChild(childCounter).GetChild(counterInChild).GetChild(1).GetChild(1)
                    .GetComponent<SpriteRenderer>().sprite = sprite;
                //итерация индексов
                counterInChild++;
                if (counterInChild != gameHolder.GetChild(childCounter).childCount) continue;
                counterInChild = 0;
                childCounter++;
            }
        }
    }
}
#endif