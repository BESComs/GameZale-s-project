using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scripts.IqushaGame2
{
    public class GameLearnFigures : MonoBehaviour, ILessonStatsObservable
    {
        //окончена ли игра
        public bool gameIsEnd;
        //кнопка рестарта игры
        public Transform restartButton;
        //текс описания уровня
        public TextMeshProUGUI textMeshPro;
        public static GameLearnFigures Instance;
        //контроллер игровых обьектов
        public GameLearnFiguresElementController selectElement;
        //спрайты для анимации после ответа
        public Transform pravilnoImage, nepravilnoImage;
        //максимальное количество уровней
        public int maxLevel;
        public bool inTask;
        //индекс правильных ответов на всех уровнях
        public List<int> correctAnswerIndex;
        //Вопросы на всех уровнях
        public List<string> levelQuestion;
        //обьект хранящий игровые обьекты на всех уровнях 
        public Transform gameObjHolder;
        //Индекс текущего уровня
        private int currentLevel;
        private async void Awake()
        {
            inTask = true;
            Instance = this;
            await LoadLevel();
            inTask = false;
        }

        //загрузка уровня
        private async Task LoadLevel()
        {
            //если загружаемый уровень не первый
            if(currentLevel != 0)
                await UnLoadLevel();
            selectElement = null;
            //текс описания уровня
            textMeshPro.text = levelQuestion[currentLevel];
            //активация нужного уровня
            gameObjHolder.GetChild(currentLevel).gameObject.SetActive(true);
            //анимация
            new ScaleIn(textMeshPro.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            await new ScaleIn(gameObjHolder.GetChild(currentLevel),AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            if(currentLevel == 0)
                RegisterLessonStart();
        }
        
        //отгрузка уровня
        private async Task UnLoadLevel()
        {
            //если был выбран какой нибудь элемент    
            
            if(selectElement != null)
            {
                selectElement.TurnOffImage();
                selectElement.selected = false;
            }
            //анимации
            new ScaleOut(textMeshPro.transform, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            await new ScaleOut(gameObjHolder.GetChild(currentLevel - 1), AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
            //деактивация предыдущего уровня
            gameObjHolder.GetChild(currentLevel - 1).gameObject.SetActive(false);
        }

        //проверка ответа при нажатии кнопки "ответить"
        public async void CheckAnswer()
        {
            //если правильный индекс = -1 то перейти без анимация на следующий уровень
            if(selectElement == null && correctAnswerIndex[currentLevel] != -1) return;
            inTask = true;
            if (correctAnswerIndex[currentLevel] != -1)
            {
                //проигрывание анимации в зависимости от того правильный ответ или нет
                if (correctAnswerIndex[currentLevel] == selectElement.currentElementIndex)
                {
                    RegisterAnswer(true);
                    pravilnoImage.gameObject.SetActive(true);
                    await new Fade(pravilnoImage,Mode.In,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                    await new WaitForSeconds(1);
                    await new Fade(pravilnoImage,Mode.Out,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                    pravilnoImage.gameObject.SetActive(false);
                }
                else
                {
                    RegisterAnswer(false);
                    nepravilnoImage.gameObject.SetActive(true);
                    await new Fade(nepravilnoImage,Mode.In,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                    await new WaitForSeconds(1);
                    await new Fade(nepravilnoImage,Mode.Out,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
                    nepravilnoImage.gameObject.SetActive(false);
                }                    
            }
            else
                RegisterAnswer(true);
            
            currentLevel++;
            //если пройденный уровень не последний то загрузить следующий иначе отгрузить и загрузить конец игры
            if (currentLevel != maxLevel)
            {
                await LoadLevel();
            }
            else
            {
                RegisterLessonEnd();
                await UnLoadLevel();
                await LoadFinalScene();
            }
            inTask = false;
        }
        
        //рестарт игры
        public async void RestartGame()
        {
            inTask = true;
            //отгрузка всех элементов финальной сцены
            await new ScaleOut(restartButton,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
            restartButton.gameObject.SetActive(false);
            //загрузка первого уровня
            await LoadLevel();
            gameIsEnd = false;
            inTask = false;
        }

        //Загрузка конца игры
        private async Task LoadFinalScene()
        {
            gameIsEnd = true;
            currentLevel = 0;
            selectElement = null;
            await new WaitForSeconds(1);
            restartButton.gameObject.SetActive(true);
            await new ScaleIn(restartButton,AnimationCurve.EaseInOut(0,0,1,1)).RunTask();
        }

        public int MaxScore
        {
            get => correctAnswerIndex.Count;
            set{}
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
