using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AnimType = _Scripts.Utility.AnimationUtility.AnimationType;
using Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Random = UnityEngine.Random;

namespace Work_Directory.Bobur._Scripts.IQSha_Games
{
    [RequireComponent(typeof(AudioSource))]
    public class GameConstructor : SerializedMonoBehaviour, ITask, ILessonStatsObservable
    {
        #region Serizilized fields
        
        [FoldoutGroup("Main Elements")]
        [OdinSerialize, HideReferenceObjectPicker, HideLabel]
        private MainElements main;

        [OdinSerialize, HideReferenceObjectPicker]
        private List<Question> questions;

        [OdinSerialize, HideReferenceObjectPicker,HideLabel, FoldoutGroup("Final Scene Elements")]
        private FinalScene finalScene;

        #endregion
        
        private int score;
        private int currentQuestion;
        private ISceneInOut activeQuestion;
        private Action onUpdate;

        private GameObject finalSceneParent;
        public bool isAnimating { get; private set; }
        
        private void Reset()
        {
            main = new MainElements();
            questions = new List<Question>();
        }

        private void Awake()
        {
            maxBal = questions.Count;
            Init();
        }
        
        
        private void Init()
        {
            // TODO - can be inefficient code 
            var toLauncher = Resources.Load<GameObject>("ToLauncher Button");
            var canvas = GameObject.Find("Canvas");
            var tmp = FindObjectsOfType<Canvas>();
            if (tmp.Length != 0)
            {
                canvas = tmp[tmp.Length - 1].gameObject;
            }
            if (toLauncher)
            {
                Instantiate(toLauncher, canvas.transform);
            }
            
            score = 0;
            currentQuestion = -1;
            onUpdate = null;
            GetHelpButtonParent.SetActive(false);
            GetNextButton.interactable = true;
            RegisterLessonStart();
            Next();
        }

        private void Update()
        {
            onUpdate?.Invoke();
        }

        private async void Next()
        {
            if (currentQuestion == -1)
            {
                currentQuestion++;
                activeQuestion = main.Init(this);
                isAnimating = false;

                if (!main.HasIntroduction)
                {
                    Next();
                }
                return;
            }
            if (activeQuestion != null)
            {
                await activeQuestion?.SceneOut();
                await Task.Delay(TimeSpan.FromSeconds(1));
                onUpdate = null;
            }
            if (CheckTaskComplete())
            {
                TaskCompleted();
                return;
            }
           
            activeQuestion = await questions[currentQuestion].Init(this);
            currentQuestion++;
            
            isAnimating = false;
        }
        
        public Question GetCurrentQuestion() => activeQuestion as Question;

        public async void NextButtonCLicked()
        {
            if (isAnimating)
                return;

            isAnimating = true;
            GetSoundsAudioSource.Stop();
            if(currentQuestion > 0 && (activeQuestion as Question).HasAnswer)
            {
                GetCurrentQuestion().ChangeStateAfterAnswer();
                if (((IGame) activeQuestion).CheckAnswer())
                {
                    score += 1;
                    RegisterAnswer(true);
                    await CorrectAnswerEffect();
                }
                else
                {
                    score -= 1;
                    RegisterAnswer(false);
                    await WrongAnswerEffect();
                }
                await Task.Delay(TimeSpan.FromSeconds(0.5f));
            }
            if(this == null) return;
            Next();
        }

        private async Task WrongAnswerEffect()
        {
            var wrongAnswerData = main.GetRandomWrongAnswerEffect();
            main.WrongAnswerTextFrame.SetText(wrongAnswerData.text);
            SetCurrentAudioClip(wrongAnswerData.audio);
            PlayCurrentAudioClip();
            main.WrongAnswerTextFrame.gameObject.SetActive(true);
            
            AnimationUtility.FadeIn(main.WrongAnswerTextFrame.spriteRenderer);
            await AnimationUtility.FadeIn(main.WrongAnswerTextFrame.TextObject.renderer);

            await Task.Delay(TimeSpan.FromSeconds(0.8f));
            
            AnimationUtility.FadeOut(main.WrongAnswerTextFrame.spriteRenderer);
            if(this == null) return;
            await AnimationUtility.FadeOut(main.WrongAnswerTextFrame.TextObject.renderer);
        }

        private async Task CorrectAnswerEffect()
        {
            var wrongAnswerData = main.GetRandomCorrectAnswerEffect();
            main.CorrectAnswerTextFrame.SetText(wrongAnswerData.text);
            SetCurrentAudioClip(wrongAnswerData.audio);
            PlayCurrentAudioClip();
            main.CorrectAnswerTextFrame.gameObject.SetActive(true);
            AnimationUtility.FadeIn(main.CorrectAnswerTextFrame.spriteRenderer);
            await AnimationUtility.FadeIn(main.CorrectAnswerTextFrame.TextObject.renderer);
            
            await Task.Delay(TimeSpan.FromSeconds(0.8f));
            
            AnimationUtility.FadeOut(main.CorrectAnswerTextFrame.spriteRenderer);
            await AnimationUtility.FadeOut(main.CorrectAnswerTextFrame.TextObject.renderer);
        }

        #region Getters
        
        public TextMeshPro GetMainTextObject => main.MainTextObject;
        public TextMeshPro GetHeaderTextObject => main.HeaderTextObject;
        public Button GetNextButton => main.NextButton;
        public TextMeshProUGUI GetNextButtonText => main.NextButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        public AudioSource GetBackgroundAudioSource => main.backgroundMusicAudioSource;
        public AudioSource GetSoundsAudioSource => main.soundsAudioSource;
        public OptionsNavigator GetOptionsNavigator => main.optionsNavigatorPrefab;
        public Button GetHelpButton => main.HelpButton;
        public GameObject GetHelpButtonParent => main.HelpButton.transform.parent.gameObject;
        
        #endregion

        #region Setters
        
        public void SetCurrentAudioClip(AudioClip clip) => main.currentSound = clip;
        public void SetOnUpdate(Action func) => onUpdate = func;
        
        #endregion
        
        
        public void PlayCurrentAudioClip() => main.soundsAudioSource.PlayOneShot(main.currentSound);
        
        public void TaskCompleted()
        {
            RegisterLessonEnd();
            finalSceneParent = new GameObject("Final Scene");
            finalSceneParent.transform.SetParent(this.transform);
            finalScene.Init(this, finalSceneParent);
        }

        public async void RestartGame()
        {
            await finalScene.SceneOut();
            Destroy(finalSceneParent);
            await new WaitForSeconds(0.5f);
            GetSoundsAudioSource.Stop();
            Init();
        }

        public bool CheckTaskComplete() => questions.Count == currentQuestion;
        private int maxBal;
        public int MaxScore
        {
            get => maxBal;
            set => maxBal = value;
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

    [Serializable]
    public class MainElements
    {
        #region Serialized fields 

        [HideInInspector] public AudioClip currentSound;
        
        [Required]
        public AudioSource backgroundMusicAudioSource;
        [Required]
        public AudioSource soundsAudioSource;
        
        public OptionsNavigator optionsNavigatorPrefab;
        
        [HorizontalGroup, OdinSerialize]
        private Dictionary<string, AudioClip> CorrectAnswerDictionary;
        [HorizontalGroup, OdinSerialize]
        private Dictionary<string, AudioClip> WrongAnswerDictionary;

        [HorizontalGroup("CorrectIncorrect Prefabs"), OdinSerialize]
        private TextFrame correctTextFrame;
        
        [HorizontalGroup("CorrectIncorrect Prefabs"), OdinSerialize]
        private TextFrame wrongTextFrame;

        [HorizontalGroup("CorrectIncorrect Position"), OdinSerialize]
        public Position2D textFramePosition;
        
        [OdinSerialize, HorizontalGroup("Checkboxes")] public bool HasIntroduction;
        [OdinSerialize, HorizontalGroup("Checkboxes")] private bool HasBackgroundMusic;

        [Required]
        public Button NextButton;

        public Button HelpButton;
        [SuffixLabel("Optional"), Tooltip("The main text object used throughout the game.")]
        public TextMeshPro MainTextObject;
        [SuffixLabel("Optional"), Tooltip("The header text object mostly used in introduction scene. Check igraemsa.ru games' first scene")]
        public TextMeshPro HeaderTextObject;
        
        [OdinSerialize, ShowIf(nameof(HasBackgroundMusic))]
        private AudioClip backgroundMusic;

        [OdinSerialize, ShowIf(nameof(HasIntroduction)), HideReferenceObjectPicker, HideLabel]
        private Introduction introduction;
        
        #endregion
        
        private GameConstructor gameConstructor;
        public TextFrame CorrectAnswerTextFrame { get; private set; }
        public TextFrame WrongAnswerTextFrame { get; private set; }
        
        public MainElements()
        {
            CorrectAnswerDictionary = new Dictionary<string, AudioClip>();
            WrongAnswerDictionary = new Dictionary<string, AudioClip>();
            HasIntroduction = false;
        }
        public ISceneInOut Init(GameConstructor constructor)
        {
            GameObject answerEffectsParent = new GameObject("Answer Effect Text Frames");
            CorrectAnswerTextFrame = GameObject.Instantiate(correctTextFrame, answerEffectsParent.transform);
            WrongAnswerTextFrame = GameObject.Instantiate(wrongTextFrame, answerEffectsParent.transform);
            CorrectAnswerTextFrame.transform.localPosition = new Vector3(textFramePosition.X, textFramePosition.Y);
            WrongAnswerTextFrame.transform.localPosition = new Vector3(textFramePosition.X, textFramePosition.Y);
            CorrectAnswerTextFrame.gameObject.SetActive(false);
            WrongAnswerTextFrame.gameObject.SetActive(false);
            
            answerEffectsParent.transform.SetParent(constructor.transform);
            
            
            if (HasBackgroundMusic && !backgroundMusicAudioSource.isPlaying)
            {
                backgroundMusicAudioSource.clip = backgroundMusic;
                backgroundMusicAudioSource.Play();
            }
            if (HasIntroduction)
            {
                gameConstructor = constructor;
                GameObject introductionParent = new GameObject("Introduction");
                introductionParent.transform.SetParent(gameConstructor.transform);
                introduction.Init(introductionParent, gameConstructor);
                
                return introduction;
            }

            return null;
        }

        public (string text, AudioClip audio) GetRandomWrongAnswerEffect()
        {
            KeyValuePair<string, AudioClip> randomElement =
                WrongAnswerDictionary.ElementAt(Random.Range(0, WrongAnswerDictionary.Count));
            return (randomElement.Key, randomElement.Value);
        }

        public (string text, AudioClip audio) GetRandomCorrectAnswerEffect()
        {
            KeyValuePair<string, AudioClip> randomElement =
                CorrectAnswerDictionary.ElementAt(Random.Range(0, CorrectAnswerDictionary.Count));
            return (randomElement.Key, randomElement.Value);
        }
    }
}
