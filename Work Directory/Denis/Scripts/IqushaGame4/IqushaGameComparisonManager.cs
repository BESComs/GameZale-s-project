using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts.IqushaGame4
{
    public class IqushaGameComparisonManager : MonoBehaviour,ILessonStatsObservable
    {
        //текстовое поле
        public TextMeshProUGUI textBox;
        public static IqushaGameComparisonManager Instance;
        //список вопросов на все уровни
        public List<string> levelQuestions;
        //Обьект содержайщий элементы на уровнях
        public Transform levelHolder;
        //спрайты для анимации после ответа
        public Transform correctAnswerImage, incorrectAnswerImage;
        //кнопка рестарта игры
        public Transform replayButton;
        public bool gameIsEnd;
        public bool inTask;
        //список индексов правильных ответов на всех уровнях
        public List<int> correctAnswerIndex;
        //индекс текущего уровня
        private int currentLevel;
        public IqushaGameComparisonController currentSelected;

        
        //проверка ответа используется кнопкой "ответить"
        public async void CheckAnswer()
        {
            if(currentSelected == null) return;
            currentSelected.TurnOffImage();
            inTask = true;
            //если индекс выбранного элемента равен правильному индексу на текущий уровень 
            if (currentSelected.currentIndex == correctAnswerIndex[currentLevel - 1])
            {
                RegisterAnswer(true);
                //анимация после правильного ответа    
                await new ScaleIn(correctAnswerImage, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new WaitForSeconds(1);
                await new ScaleOut(correctAnswerImage, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            else
            {
                RegisterAnswer(false);
                //анимация после неправильного ответа
                await new ScaleIn(incorrectAnswerImage, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new WaitForSeconds(1);
                await new ScaleOut(incorrectAnswerImage, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            //загрузка уровня
            await LoadUnloadLevel();
            inTask = false;
        }

        //рестарт игры используется кнопкой "рестарт" - активна после прохождения игры
        public async void RestartGame()
        {
            inTask = true;
            currentLevel = 0;
            currentSelected = null;
            await new ScaleOut(replayButton, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            replayButton.gameObject.SetActive(false);
            await LoadUnloadLevel();
            gameIsEnd = false;
            inTask = false;
        }
        
        private async void Awake()
        {
            inTask = true;
            Instance = this;
            await LoadUnloadLevel();
            inTask = false;
        }

        private async Task LoadUnloadLevel()
        {
            //если загружаемый уровень первый 
            if (currentLevel == 0)
            {
                //анимированная загрузка первого уровня
                levelHolder.GetChild(currentLevel).gameObject.SetActive(true);                
                textBox.text = levelQuestions[currentLevel];
                new ScaleIn(levelHolder.GetChild(currentLevel), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new ScaleIn(textBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                RegisterLessonStart();
            }
            //если текущий уровень последний
            else if (currentLevel == correctAnswerIndex.Count)
            {
                RegisterLessonEnd();
                await EndGame();   
            }
            else
            {
                //анимированная загрузка следующего уровня и отгрузка предыдущего
                new ScaleOut(textBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new ScaleOut(levelHolder.GetChild(currentLevel - 1), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                levelHolder.GetChild(currentLevel - 1).gameObject.SetActive(false);                
                levelHolder.GetChild(currentLevel).gameObject.SetActive(true);                
                textBox.text = levelQuestions[currentLevel];
                new ScaleIn(levelHolder.GetChild(currentLevel), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new ScaleIn(textBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            currentLevel++;
        }

        private async Task EndGame()
        {
            //анимированная отгрузка последнего уровня и загрузка конца игры
            gameIsEnd = true;
            new ScaleOut(textBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            await new ScaleOut(levelHolder.GetChild(currentLevel - 1), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            levelHolder.GetChild(currentLevel - 1).gameObject.SetActive(false);
            replayButton.gameObject.SetActive(true);
            await new ScaleIn(replayButton, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
        }

        public int MaxScore { get=>correctAnswerIndex.Count; set {} }
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
}
