using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts.IqushaGame3
{
    public class PositionInSpaceGameManager : MonoBehaviour,ILessonStatsObservable
    {
        //Кнопка повтора игры Активируется после окончания 
        public Transform replayButton;
        //родительский обьект
        public Transform gameElementHolder;
        //TMPUGUI Элемент отвечающий за появление текста
        public TextMeshProUGUI gameTextBox;
        public bool gameIsEnd;
        //Контроллер игроых элементов
        public PositionInSpaceElementController selectedElement;
        public static PositionInSpaceGameManager Instance;
        public bool inTask;
        //спрайты для анимации после ответа
        public Transform imageCorrectAnswer, imageIncorrectAnswer;
        //Индекс правильных ответов на всех уровнях
        public List<int> correctAnswerIndex;
        //тексты описывающие уровни
        public List<string> gameQuestion;
        private int currentGameLevel;

        private async void Awake()
        {
            Instance = this;
            inTask = true;
            await LoadUnloadLevel();
            RegisterLessonStart();
            inTask = false;
        }

        //проверка ответа используется кнопкой "Ответить"
        public async void CheckAnswer()
        {
            if (selectedElement == null) return;
            inTask = true;
            //если индекс выбранного элемента равен правильному индексу на текущий уровень 
            if (correctAnswerIndex[currentGameLevel - 1] == selectedElement.index)
            {
                RegisterAnswer(true);
                //анимация после правильного ответа
                await new ScaleIn(imageCorrectAnswer, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new WaitForSeconds(1);
                await new ScaleOut(imageCorrectAnswer, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            else
            {
                RegisterAnswer(false);
                //анимация после неправильного ответа
                await new ScaleIn(imageIncorrectAnswer, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new WaitForSeconds(1);
                await new ScaleOut(imageIncorrectAnswer, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            }
            //загрузка уровня
            await LoadUnloadLevel();
            inTask = false;
        }

        //загрузка следующего уровня и отгрузка текущего
        private async Task LoadUnloadLevel()
        {
            //очистка выбранного элемента для следующего уровня
            if (selectedElement != null)
            {
                selectedElement.isSelected = false;
                selectedElement.TurnOffImage();
                selectedElement = null;
            }
            //если текущий уровень не последний
            if (currentGameLevel != correctAnswerIndex.Count)
            {
                //если загружаемый уровень не первый
                if (currentGameLevel != 0)
                {
                    //анимированная загрузка следующего уровня и отгрузка предыдущего
                    new ScaleOut(gameTextBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    await new ScaleOut(gameElementHolder.GetChild(currentGameLevel - 1), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                    gameElementHolder.GetChild(currentGameLevel - 1).gameObject.SetActive(false);
                    gameTextBox.text = gameQuestion[currentGameLevel];
                    gameElementHolder.GetChild(currentGameLevel).gameObject.SetActive(true);
                    new ScaleIn(gameTextBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    await new ScaleIn(gameElementHolder.GetChild(currentGameLevel), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                }
                else
                {
                    //анимированная загрузка первого уровня
                    gameTextBox.text = gameQuestion[currentGameLevel];
                    gameElementHolder.GetChild(currentGameLevel).gameObject.SetActive(true);
                    new ScaleIn(gameTextBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    await new ScaleIn(gameElementHolder.GetChild(currentGameLevel), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                }
            }
            else
            {  
                RegisterLessonEnd();
                //анимированная отгрузка последнего уровня
                new ScaleOut(gameTextBox.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                await new ScaleOut(gameElementHolder.GetChild(currentGameLevel - 1), AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                gameElementHolder.GetChild(currentGameLevel - 1).gameObject.SetActive(false);
                await LoadFinalScene();
            }
            currentGameLevel++;
        }

        //загрузка конца игры после её прохождения
        private async Task LoadFinalScene()
        {
            gameIsEnd = true;
            replayButton.gameObject.SetActive(true);
            await new ScaleIn(replayButton, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
        }

        //рестарт игры используется кнопкой "рестарт" - активна после прохождения игры
        public async void RestartGame()
        {
            inTask = true;
            currentGameLevel = 0;
            await new ScaleOut(replayButton.transform, AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            await LoadUnloadLevel();
            gameIsEnd = false;
            inTask = false;
        }

        public int MaxScore
        {
            get => correctAnswerIndex.Count;
            set {}
        }

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
