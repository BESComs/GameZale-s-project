using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Drag_and_Drop;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Difference;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Pair;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Input_Answer;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Multiple_Choice;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using AnimType = _Scripts.Utility.AnimationUtility.AnimationType;

namespace Work_Directory.Bobur._Scripts.IQSha_Games
{
    [Serializable]
    public class Question : IGame
    {
        #region Serialized Fields
        
        [FoldoutGroup("$questionType"), OdinSerialize]
        private QuestionType questionType;
        
        [HorizontalGroup("$questionType/Animations/AnimationType"), OdinSerialize, PropertyOrder(99), TitleGroup("Animations", GroupID = "$questionType/Animations")]
        private AnimType animationInType;
        
        [HorizontalGroup("$questionType/Animations/AnimationType"), OdinSerialize, PropertyOrder(99), TitleGroup("Animations", GroupID = "$questionType/Animations")]
        private AnimType animationOutType;
        
        [HorizontalGroup("$questionType/CheckBoxes1"), OdinSerialize]
        private bool hasQuestionAudio;
        
        [HorizontalGroup("$questionType/CheckBoxes1"), OdinSerialize]
        private bool hasQuestionText;
        
        [HorizontalGroup("$questionType/CheckBoxes2"), OdinSerialize]
        private bool hasQuestionImage;
        
        [HorizontalGroup("$questionType/CheckBoxes2"), OdinSerialize]
        private bool hasAnswer;
        
        [HorizontalGroup("$questionType/CheckBoxes3"), OdinSerialize]
        public bool sceneInWithAll;
        
        [HorizontalGroup("$questionType/CheckBoxes3"), OdinSerialize]
        public bool sceneOutWithAll;
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionAudio))]
        private AudioClip questionAudio;
        
        [FoldoutGroup("$questionType"), OdinSerialize, Multiline, ShowIf(nameof(hasQuestionText))]
        private string questionText;
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionText))]
        private float questionTextSize = 4f;

        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasAnswer))]
        private string nextButtonText = "Ответить";
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionText))]
        private Vector2 questionTextPosition;
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionImage)), OnValueChanged(nameof(SetSizeOfImagePositions)), PreviewField(50)]
        private List<Sprite> questionImages;
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionImage))]
        private List<Vector2> imagePositions;

        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionImage)), HorizontalGroup("$questionType/Size")]
        private float? maxHeight;
        
        [FoldoutGroup("$questionType"), OdinSerialize, ShowIf(nameof(hasQuestionImage)), HorizontalGroup("$questionType/Size")]
        private float? maxWidth;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.MultipleChoice), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private MultipleChoice multipleChoice;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.Puzzle), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private Games.Puzzle.Puzzle puzzle;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.DragAndDrop), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private DragAndDrop dragAndDrop;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.FindPair), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private FindPair findPair;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.CardMatch), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private CardMatch cardMatch;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.InputAnswer), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private InputAnswer inputAnswer;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.FindDifference), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private FindDifference findDifference;

        [FoldoutGroup("$questionType"), ShowIf(nameof(questionType), QuestionType.Paint), ShowIf(nameof(hasAnswer))]
        [OdinSerialize, HideReferenceObjectPicker, HideLabel]
        private Paint paint;

        #endregion

        public bool resizeQuestionImage;
        public Vector2 questionImageSize;
        private IGame question;
        private GameConstructor gameConstructor;
        private GameObject questionHolder;
        public List<Renderer> animatedObjects { get; private set; }
        
        public async Task<ISceneInOut> Init(GameConstructor constructor)
        {   
            if (constructor == null) return null;
            gameConstructor = constructor;
            animatedObjects = new List<Renderer>();
            
            questionHolder = new GameObject("Question holder");

            questionHolder.transform.SetParent(constructor.transform);
            
            gameConstructor.GetNextButtonText.text = (hasAnswer && (nextButtonText != null && nextButtonText.Trim() != string.Empty)) ? nextButtonText : "Далее";
            
            if (hasQuestionText)
            {
                var mainTextObject =
                    GameObject.Instantiate(gameConstructor.GetMainTextObject, questionHolder.transform); 
                mainTextObject.text = questionText;
                mainTextObject.gameObject.SetActive(false);
                mainTextObject.fontSize = questionTextSize;
                animatedObjects.Add(mainTextObject.renderer);

                    mainTextObject.transform.localPosition = new Vector3(questionTextPosition.x, questionTextPosition.y);
            }
            
            if (hasQuestionImage)
            {
                for (int i = 0; i < questionImages.Count; i++)
                {
                    GameObject ob = new GameObject("Question Image");
                    ob.transform.SetParent(questionHolder.transform);
                    ob.AddComponent<SpriteRenderer>().sprite = questionImages[i];
                    ob.transform.localPosition = new Vector3(imagePositions[i].x,imagePositions[i].y);
                    ob.SetActive(false);
                    if (resizeQuestionImage)
                    {
                        ob.transform.localScale = questionImageSize;
                        Debug.Log(123);
                    }
                    else
                    {
                        //var rescale = ImageScaleHelper.NormalizeScale(questionImages[i]);
                        //ob.transform.localScale = questionImageSize * rescale;
                        //Debug.Log("Image size normalized: " + questionImageSize * rescale);
                    }
                    
                    ob.SetActive(false);
                    animatedObjects.Add(ob.GetComponent<SpriteRenderer>());
                }
            }
            
            if (hasAnswer)
            {
                gameConstructor.GetNextButton.interactable = false;
                switch (questionType)
                {
                    case QuestionType.MultipleChoice : question = multipleChoice.Init(constructor, questionHolder); break;
                    case QuestionType.Puzzle : question = puzzle.Init(constructor, questionHolder); break;
                    case QuestionType.DragAndDrop : question = dragAndDrop.Init(constructor, questionHolder); break;
                    case QuestionType.FindPair : question = findPair.Init(constructor, questionHolder); break;
                    case QuestionType.CardMatch : question = cardMatch.Init(constructor, questionHolder); break;
                    case QuestionType.InputAnswer : question = inputAnswer.Init(constructor, questionHolder); break;
                    case QuestionType.FindDifference : question = findDifference.Init(constructor, questionHolder); break;
                    case QuestionType.Paint : question = paint.Init(constructor, questionHolder); break;
                }
            }
            
            await SceneIn();
            
            if (hasQuestionAudio)
            {
                gameConstructor.SetCurrentAudioClip(questionAudio);
                gameConstructor.PlayCurrentAudioClip();
            }
            
            return this;
        }

        public async Task SceneIn()
        {
            foreach (Renderer animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(true);

                if (animationInType == AnimType.Fade)
                {
                    AnimationUtility.FadeIn(animatedObject, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                } 
                else if (animationInType == AnimType.Scale)
                {
                    AnimationUtility.ScaleIn(animatedObject.gameObject, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                }
            }

            if(!sceneInWithAll)
                await new WaitForSeconds(1);
            
            
            question?.SceneIn();
        }

        public async Task SceneOut()
        {
            if (hasAnswer)
            {
                if (sceneOutWithAll)
                    question?.SceneOut();
                else
                    await question?.SceneOut();
            }
            
            foreach (Renderer animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(true);

                if (animationOutType == AnimType.Fade)
                {
                    AnimationUtility.FadeOut(animatedObject, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                } 
                else if (animationOutType == AnimType.Scale)
                {
                    AnimationUtility.ScaleOut(animatedObject.gameObject, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                }
            }
            
            await new WaitForSeconds(1.2f);
            GameObject.Destroy(questionHolder);
        }

        public bool HasAnswer => hasAnswer;

        public void ChangeStateAfterAnswer() => question?.ChangeStateAfterAnswer();

        public bool CheckAnswer() => question?.CheckAnswer() ?? true;

        private void SetSizeOfImagePositions()
        {
            if (imagePositions == null)
            {
                imagePositions = new List<Vector2>(questionImages.Count);
            }

            if (imagePositions.Count > questionImages.Count)
            {
                while (imagePositions.Count != questionImages.Count)
                {
                    imagePositions.RemoveAt(imagePositions.Count - 1);
                }
            } 
            else if (imagePositions.Count < questionImages.Count)
            {
                while (imagePositions.Count != questionImages.Count)
                {
                    imagePositions.Add(new Vector2());
                }
            }
        }
    }

    public enum QuestionType
    {
        MultipleChoice,
        DragAndDrop,
        InputAnswer,
        Puzzle,
        Paint,
        FindPair,
        FindDifference,
        CardMatch,
    }
}


public static class ImageScaleHelper
{
    public static Vector3 NormalizeScale(Sprite s)
    {
        var ppu = s.pixelsPerUnit;
        var height = s.bounds.size.y;
        var scaleFactor = height / ppu;
        return Vector3.one / scaleFactor;
    }
}