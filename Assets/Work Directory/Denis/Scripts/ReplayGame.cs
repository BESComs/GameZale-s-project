using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Denis.Scripts.Tasks;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts
{
    /*
     * script using in replayButton for replay game
     * the script is used only in those games that are inherited from class task parent
     */
    public class ReplayGame : MonoBehaviour
    {    
        //Префаб игры
        public Transform gamePrefab;
        [HideInInspector]public TaskParent cacheGamePrefab;
        //обьект который появляется после окончания игры
        public Transform finalScene;
        //выполняется ли асинхронные операции. Используется для того чтобы игрок не нажимал на кнопку во время анимации
        private bool _inTask;
        private Button _replayButton;
        //префаб отвечающий за аудио сопровождение в игре
        [HideInInspector]public Transform audioPrefab;
        //кнопка в игре для проверки ответа на определенном уровне (присутствует в некоторых играх)
        public Transform checkButton;
        
        private void Awake()
        {
            _replayButton = GetComponent<Button>();
            transform.localScale = new Vector3(.75f,.75f,1);
            //изменяет спрайт кнопки повтора
            var tmp = (Resources.Load("fon_zapchasti_cveta_4096(2) (2)_11", typeof(GameObject)));
            var tmpImage = GetComponent<Image>();
            tmpImage.sprite = (tmp as GameObject)?.transform.GetComponent<SpriteRenderer>().sprite;
            tmpImage.SetNativeSize();
            _replayButton.transition = Selectable.Transition.None;
            
            _replayButton.onClick.AddListener(async delegate
            {
                if (_inTask) return;

                if(endPS != null)
                {
                    //уничтожение системы частиц 
                    endPS.GetComponent<ParticleSystem>().Stop();
                    Destroy(endPS);
                }
                _inTask = true;
                //проигрывание аудио в игре
                var ac = audioPrefab.GetComponent<AudioController>();
                ac.StartGameSounds();
                ac.inGame = true;
                //исчезновение финальной сцены и кнопки повтора
                new Fade(finalScene).RunTask();
                await new Fade(transform).RunTask();
                finalScene.gameObject.SetActive(false);
                gameObject.SetActive(false);
                //уничтожение префаба пройденной игры
                Destroy(cacheGamePrefab.gameObject);
                //Создание нового префаба игры чтобы пройти игру снова и его поевление на сцене
                cacheGamePrefab = Instantiate(gamePrefab).GetComponent<TaskParent>();
                if(checkButton != null)
                    checkButton.gameObject.SetActive(true);
                cacheGamePrefab.gameObject.SetActive(true);
                cacheGamePrefab.audioPrefab = audioPrefab;
                cacheGamePrefab.replayButton = transform;
                cacheGamePrefab.finalScene = finalScene;
                cacheGamePrefab.SetButton();
                if(checkButton != null)
                    new Fade(checkButton, Mode.In).RunTask();
                await new Fade(cacheGamePrefab, Mode.In).RunTask();
                _inTask = false;
            });
        }

        //система частиц проигрывается после прохождения игры
        private GameObject endPS;
        private void OnEnable()
        {
            if(endPS == null)
                Destroy(endPS);
            endPS = Instantiate((GameObject)Resources.Load("EndPS", typeof(GameObject)),new Vector3(0,-3,0),new Quaternion());
            endPS.GetComponent<ParticleSystem>().Play();
        }
        
    }
}
