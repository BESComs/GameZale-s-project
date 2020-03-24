using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;
using Work_Directory.Denis.Scripts.Tasks;

#pragma warning disable 4014
namespace Work_Directory.Denis.Scripts
{
    /*
     * This script using for games are inherited from task parent Class to start
     * game and to weather everything you need to start the game again if player passed the game
     */
    public class StartPlay : MonoBehaviour
    {
        //игровые звуки
        public Transform audioPrefab;
        public AudioClip finalAudio;
        public AudioClip gameAudio;
        public AudioClip introAudio;
        //обьекты на сцене после окончания игры
        public Transform finalScene;
        //кнопка повтора игры
        public Transform replayButton;
        //обьекты на сцене до начала игры
        public Transform intro;
        //выполняются ли асинхронные операции
        private bool _buttonInTask;
        private Button _startButton;
        //префаб игры
        public Transform gamePrefab;
        //
        private TaskParent _cacheGamePrefab;
        public Transform checkButton;
        //скрипт отвечающий за аудио сопровождение в игре
        private AudioController _audioController;
        
        /*
         * prepare games to restart if user completely finish the game
         * used in games that are inherited from taskParent
         */

        private void Awake()
        {
            //загрузка кнопки отвечающей за возврат в лаунчер
            Instantiate((GameObject) UnityEngine.Resources.Load("ToLauncher Button", typeof(GameObject)),
                GetComponentInParent<Canvas>().transform, false);
            _startButton = GetComponent<Button>();
            //загрузка префаба отвечающего за аудио в игре
            audioPrefab = Instantiate(audioPrefab, transform.GetComponentInParent<Canvas>().transform, false);
            //Назначение определенных переменных AudioController-у для проигрывания различных звуков в игре
            _audioController = audioPrefab.GetComponent<AudioController>();
            _audioController.finalAudio = finalAudio;
            _audioController.introAudio = introAudio;
            _audioController.gameSounds = gameAudio;
            _audioController.OnInstantiate();
            _audioController.StartIntroAudioClip();
            //замена спрайта данной кнопки
            var tmp = (Resources.Load("igrat-button_0", typeof(GameObject)));
            GetComponent<Image>().sprite = (tmp as GameObject)?.transform.GetComponent<SpriteRenderer>().sprite;
            _startButton.transition = Selectable.Transition.None;
            _startButton.onClick.AddListener(async delegate
            {
                if (_buttonInTask) return;
                _buttonInTask = true;
                //старт аудио в игре
                _audioController.StartGameSounds();
                _audioController.inGame = true;
                //исчезновение начальных элементов
                new Fade(intro).RunTask();
                for (var i = 0; i < intro.childCount; i++)
                {
                    if (intro.GetChild(i).GetComponent<TextMeshProUGUI>())
                    {
                        new ScaleOut(intro.GetChild(i), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    }
                }

                //исчезновение кнопки
                //todo await new Fade(transform).RunTask();
                new Fade(transform).RunTask();
                if (this == null) return;
                intro.gameObject.SetActive(false);
                gameObject.SetActive(false);
                //создание игры
                _cacheGamePrefab = Instantiate(gamePrefab).GetComponent<TaskParent>();
                replayButton.GetComponent<ReplayGame>().audioPrefab = audioPrefab;
                _cacheGamePrefab.replayButton = replayButton;
                _cacheGamePrefab.finalScene = finalScene;
                _cacheGamePrefab.audioPrefab = audioPrefab;
                _cacheGamePrefab.SetButton();
                _cacheGamePrefab.gameObject.SetActive(true);
                //анимированное появление игры
                if (checkButton != null)
                {
                    checkButton.gameObject.SetActive(true);
                    new Fade(checkButton, Mode.In).RunTask();
                }

                await new Fade(_cacheGamePrefab, Mode.In).RunTask();
                if (this == null) return;
                _buttonInTask = false;
            });
        }
    }
}
