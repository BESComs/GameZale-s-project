using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts.IqushaGame1
{
    public class MyGameManager : MonoBehaviour,ILessonStatsObservable
    {
        //спрайты для анимации после ответа
        public Transform correctAnswerText;
        public Transform wrongAnswerText;
        //обьект TextMeshPro
        public Transform gameTextTransform;
        //кнопка рестарта
        public Transform replayButton;
        //текст
        private TextMeshProUGUI gameText;
        //список уровней 
        public List<Level> levels = new List<Level>();
        public static MyGameManager Instance;
        public bool inTask;
        //текущий уровень
        private Level currentLevel;
        //selected pieces in current level
        private readonly HashSet<int> currentSelectPieces = new HashSet<int>();
        //родительский обьект для всех обьектов текущего уровня
        public Transform parent;
        //количество пройденных уровней
        private int levelCounter;
        //тригер конца игры
        public bool gameEnd;
        private async void Awake()
        {
            Instance = this;
            gameText = gameTextTransform.GetComponent<TextMeshProUGUI>();
            for (var i = 0; i < levels.Count; i++)
            {
                levels[i].levelIndex = i;
            }
            
            inTask = true;
            await LevelGeneration();
            inTask = false;
        }
        
        //рестарт игры
        public async Task RestartGame()
        {
            inTask = true;
            await new FadeOut(replayButton, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            replayButton.gameObject.SetActive(false);
            await LevelGeneration();
            gameEnd = false;
            inTask = false;
        }
        
        //конец игры
        private async Task EndGame()
        {
            gameEnd = true;
            currentLevel = levels[0];
            levelCounter = 0;
            replayButton.gameObject.SetActive(true);
            await new FadeIn(replayButton, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
        }

        //Генерация уровня
        private async Task LevelGeneration()
        {
            //очистка выбранных элементов
            currentSelectPieces.Clear();
            //если последний уровень
            if(levelCounter == 0)
                RegisterLessonStart();
            if (levelCounter == levels.Count)
            {
                RegisterLessonEnd();
                await UnLoadLevel();
                await EndGame();
                inTask = false;
                return;
            }
            currentLevel = levels[levelCounter++];
            //Отгрузка текущего уровня если это не 0 уровень
            if(currentLevel.levelIndex != 0)
                await UnLoadLevel();
            //Загрузка уровня
            LoadLevel();
            
        }

        //анимированная отгрузка уровня
        private async Task UnLoadLevel()
        {
            inTask = true;
            for (var i = 0; i < parent.childCount; i++)
            {
                new FadeOut(parent.GetChild(i), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                new FadeOut(parent.GetChild(i).GetChild(0), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            await new WaitForSeconds(1);
            await new WaitForUpdate();
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
            inTask = false;
        }

        //выбор обьекта
        public void Select(MyGameElementController gameElementController)
        {
            //если обьект уже выбран то пометить как не выбранный иначе выбрать
            if(!currentSelectPieces.Add(gameElementController.index))
            {
                currentSelectPieces.Remove(gameElementController.index);
                gameElementController.backFon.color = Color.white;
                return;
            }
            gameElementController.backFon.color = Color.green;
        }
        
        //анимация эффекта на правильный ответ
        private async Task CorrectAnswer()
        {   
            inTask = true;
            RegisterAnswer(true);
            await new FadeIn(correctAnswerText,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            await new WaitForSeconds(1);
            await new FadeOut(correctAnswerText,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            await LevelGeneration();
            inTask = false;
        }

        //проверка ответа после нажатия кнопки "Ответить"
        public async void CheckAnswer()
        {
            inTask = true;
            //если количество выбранных обьектов неравно количеству правильных обьектов то ответ не правильный
            if (currentSelectPieces.Count != currentLevel.correctAnswerIndex.Count)
            {
                await WrongAnswer();
                inTask = false;
                return;
            }
            //если список индексов выбранных элементов совпадает с правильными то ответ правильный
            foreach (var i in currentLevel.correctAnswerIndex)
            {
                if (currentSelectPieces.Contains(i)) continue;
                await WrongAnswer();
                inTask = false;
                return;
            }
            await CorrectAnswer();
            inTask = false;
        }

        //загрузка уровня
        private void LoadLevel()
        {
            gameText.text = currentLevel.levelText;
            //расстановка всех обьектов на загружаемый уровень
            for (var i = 0; i < currentLevel.figuresOnLevel.Count; i++)
            {
                //создание всех необходимых обьектов и добавление к ним нужных компонентов
                var tmpGo = new GameObject();
                var tmpGoChild = new GameObject();
                var gec = tmpGo.AddComponent<MyGameElementController>();
                var srChild = tmpGoChild.AddComponent<SpriteRenderer>();
                var sr = tmpGo.AddComponent<SpriteRenderer>();
                sr.sprite = currentLevel.spriteBackFon.GetComponent<SpriteRenderer>().sprite;
                tmpGo.AddComponent<BoxCollider2D>();
                //установка на нужные позиции
                tmpGo.transform.localScale = Vector3.one * 2.25f;
                tmpGo.transform.SetParent(parent);
                tmpGo.transform.localPosition = new Vector3(-2.5f * Mathf.Pow(-1, i % 2),1.5f - i  / 2 * 2);
                tmpGoChild.transform.SetParent(tmpGo.transform);
                tmpGoChild.transform.localScale = Vector3.one / 2.25f;
                tmpGoChild.transform.localPosition = Vector3.zero;
                srChild.sprite = currentLevel.figuresOnLevel[i];
                srChild.sortingOrder = 2;
                gec.index = i;
                gec.backFon = sr;
            }
        }

        //анимация эффекта на неправильный ответ
        private async Task WrongAnswer()
        {
            inTask = true;
            RegisterAnswer(false);
            await new FadeIn(wrongAnswerText,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            await new WaitForSeconds(1);
            await new FadeOut(wrongAnswerText,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            await LevelGeneration();
            inTask = false;
        }

        public int MaxScore { get=>levels.Count; set{} }
        public void RegisterAnswer(bool isAnswerRight){
            if (!isAnswerRight) return;
            LessonStatistic.SetScore(1);
            LessonStatistic.SetRight(isAnswerRight);
		}

		public void RegisterLessonStart(){
			LessonStatistic.SetStartLessonTime();
		}
		

		public void RegisterLessonEnd(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
			ServerRequests.PostStatistics();
		}
		public void OnApplicationPause(){
			LessonStatistic.SetLessonDurationWithEndLessonTime();
		}
    }

    //необходимые обьекты для одного уровня
    [Serializable]
    public class Level
    {
        public int levelIndex;
        public Transform spriteBackFon;
        public List<Sprite> figuresOnLevel;
        public List<int> correctAnswerIndex;
        public string levelText;
    }
}
