using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Work_Directory.Denis.Scenes.NewLauncher;
using Work_Directory.Denis.Scenes.NewLauncher.Scripts;


namespace Work_Directory.Denis.Scripts
{
    public class ExitButton : MonoBehaviour
    {
        /*
         * script using for Back to gamesLauncher
         * if button is pressed unload current game scene and activate GamesLauncher scene
         */
        private bool _inTask;
        public Sprite spriteToCategories;
        private Button _exitButton;
        public ToHomeToCategories homeOrCategories;
        private void Awake()
        {
            const int delayMilliseconds = 100;
            
            //GetComponent<RectTransform>().localPosition = new Vector3(905,485,0);
            _exitButton = GetComponent<Button>();
            _exitButton.onClick.AddListener( async delegate { 
                if(_inTask) return;
                _inTask = true;
                var allObj = FindObjectsOfType<Transform>();
                
                foreach (var transform1 in allObj)
                {
                    var iLesson = transform1.GetComponent<ILessonStatsObservable>();
                    if (iLesson == null) continue;

                    if (LessonStatistic.HasStartAndDuration())
                        ServerRequests.PostStatistics();
                }
                var sceneLoading = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                await new WaitWhile(() => !sceneLoading.isDone);
                await Task.Delay(delayMilliseconds);
                LauncherController.Instance?.gameObject.SetActive(true);
                LauncherV2.Instance?.Enable();
                AdvertisementController.Instance.ShowInterstitialAd();
                _inTask = false;
            });
        }
    }
    
    public enum ToHomeToCategories
    {
        ToHome, ToCategories
    }
}