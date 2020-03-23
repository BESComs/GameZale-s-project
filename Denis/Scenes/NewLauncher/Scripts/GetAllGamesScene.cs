using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    public class GetAllGamesScene : MonoBehaviour
    {
        
        //Объект на сцене содержащий все игры 
        public Transform gameHolder;
        //Ранее использовалась для назначения имяни играм 
        public List<string> gameSceneName = new List<string>();
        //Используется для назначения путей к сценам для запуска игр
        public List<string> gameScenePath = new List<string>();
        //Ранее использовалась для назначения имяеи категориям
        public List<string> gameFolderName = new List<string>();
        //хранит первый индекс игры в категориях использовалась для проверки корректного добовления игр
        public List<int> gameIndex = new List<int>();
        //префикс путей
        public string pref;

#if UNITY_EDITOR
        
        //Устанавливает листьям иерархии (играм) путь к сцене для запуска игр
        [Button]
        public void SetPath()
        {
            var counter = 0;
            for (var i = 0; i < gameHolder.childCount; ++i)
            {
                for (var j = 0; j < gameHolder.GetChild(i).childCount; ++j)
                {
                    var tmpChild = gameHolder.GetChild(i).GetChild(j);
                    tmpChild.GetComponent<FolderController>().gamePath = gameScenePath[counter];
                    counter++;
                }
            }
        }
        
        //Устанавливает фалг игры
        [Button]
        public void SetGamesFolder()
        {
            for (var i = 0; i < gameHolder.childCount; i++)
            {
                for (var j = 0; j < gameHolder.GetChild(i).childCount; j++)
                {
                    gameHolder.GetChild(i).GetChild(j).GetComponent<FolderController>().isGame = true;
                }
            }
        }
        //Собирает все сцены из папки Resoursec методом DFS (Поиск в глубину)
        [Button]
        private void GetGamesScene()
        {
            //Собирает все SceneAsset из папки Resources
            var scenesAsset = Resources.LoadAll<SceneAsset>("/");
            //Очистка всех списков
            gameIndex.Clear();
            gameFolderName.Clear();
            gameScenePath.Clear();
            //Заполнение всех списков
            foreach (var sceneAsset in scenesAsset)
            {
                //путь к сцене
                var tmpSceneName = AssetDatabase.GetAssetPath(sceneAsset);
                var tmpPostfix = tmpSceneName.Substring(0,pref.Length);
                if(tmpPostfix != pref)
                    continue;
                //добовляет путь к сцене
                gameScenePath.Add(tmpSceneName);
                //Парсит путь
                var tmpPrefix = tmpSceneName.Substring(pref.Length, tmpSceneName.Length - tmpPostfix.Length);
                var tmpFolderName = string.Empty;
                var slashCount = 0;
                for (var i = tmpPrefix.Length - 1; i >= 0; i--)
                {
                    if (tmpPrefix[i] == '/')
                        slashCount++;
                    else if (slashCount == 1)
                        tmpFolderName += tmpPrefix[i];
                    else if(slashCount == 2)
                        break;
                }
                var folderName = string.Empty;
                for (var i = tmpFolderName.Length - 2; i >= 0; i--)
                    folderName += tmpFolderName[i];
                gameFolderName.Add(folderName);
            }
            //добавливает индексы
            var currentFolderName = gameFolderName[0];
            gameIndex.Add(0);
            for (var i = 1; i < gameFolderName.Count; i++)
            {
                if (currentFolderName == gameFolderName[i]) continue;
                currentFolderName = gameFolderName[i];
                gameIndex.Add(i);
            }
        }

        [Button]
        public void AddScenePathToScreenShooter()
        {
            transform.GetComponent<ScreenShoot>().scenePath = gameScenePath.GetRange(0, gameScenePath.Count);
        }

#endif
    }
}