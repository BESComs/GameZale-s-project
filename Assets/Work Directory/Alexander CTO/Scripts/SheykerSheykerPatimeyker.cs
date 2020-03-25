
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    /*
     * При запуске сцены ScreenShooterScene автоматически по очереди загружаются сцены в том порядке в котором
     * они былои собраны методом GetGamesScene класса GetAllGamesScene
     * и делается скриншот
     * скрины сохраняются в папке C:\Users\User\Desktop\ScreenShoots
     */
    public class SheykerSheykerPatimeyker : MonoBehaviour
    {
        [FormerlySerializedAs("lessonBatch")] [FormerlySerializedAs("lessonCollection")] public LessonPack lessonPack;
        
        public Transform findILesson;
        //размер изображения
        
        public List<(string, int)> levelsCount;
        
        //перед запуском нужно заполнить лист методом GetGamesScene класса GetAllGamesScene
        public List<string> scenePath;

        [Button]
        public void Shit()
        {
            scenePath = new List<string>(GetComponent<ScreenShoot>().scenePath);
        }

        private async void Awake()
        {
            levelsCount = new List<(string, int)>();
            //ожидание ввода пробела
            await new WaitWhile(() => !Input.GetKeyDown(KeyCode.Space));
            
            for (var i = 0; i < scenePath.Count; i++)
            {
                var lessonPath = scenePath[i];
                var lastSlashIndex = lessonPath.LastIndexOf('/') + 1;
                var lessonName = lessonPath.Substring(lastSlashIndex, lessonPath.Length - lastSlashIndex - 6);
                
                await SceneManager.LoadSceneAsync(scenePath[i], LoadSceneMode.Additive);
                
                await new WaitForSeconds(.1f);
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                var startPlayButton = FindObjectOfType<StartPlay>();
                
                if (startPlayButton)
                    startPlayButton.GetComponent<Button>().onClick.Invoke();
                else
                {
                    if(FindObjectsOfType<GameConstructor>().Length == 0)
                    {
                        var buttons = FindObjectsOfType<Button>();
                        foreach (var b in buttons)
                        {
                            if (b.transform.GetComponent<ExitButton>() != null) continue;
                            b.onClick.Invoke();
                            break;
                        }
                    }
                }
                await new WaitForUpdate();
                var tmp = Instantiate(findILesson);
                var maxScore =  tmp.GetComponent<FindILessonTimeObservable>().MaxScore();
                print(maxScore);
                lessonPack.SetLessonMaxScore(lessonName, maxScore);
                
                await SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                await new WaitForSeconds(.1f);
              
            }
            
            lessonPack.Save();
            
        }
     
    }
}
#endif
