using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using AnimType = _Scripts.Utility.AnimationUtility.AnimationType;

namespace Work_Directory.Bobur._Scripts.IQSha_Games
{
    [Serializable]
    public class Introduction : ISceneInOut
    {
        #region Serializied fields

        [OdinSerialize, FoldoutGroup("Introduction"), HorizontalGroup("Introduction/Checkboxes2")] private bool hasIntroductionText;
        [OdinSerialize, FoldoutGroup("Introduction"), HorizontalGroup("Introduction/Checkboxes2")] private bool hasHeaderText;
        [OdinSerialize, FoldoutGroup("Introduction"), HorizontalGroup("Introduction/Checkboxes3")] private bool hasIntroductionAudio;
        [OdinSerialize, FoldoutGroup("Introduction"), HorizontalGroup("Introduction/Checkboxes3")] private bool hasIntroductionImage;

        [OdinSerialize, SuffixLabel("Optional"), FoldoutGroup("Introduction")]
        private string NextButtonText = string.Empty;
        
        [Multiline, FoldoutGroup("Introduction"), OdinSerialize, ShowIf(nameof(hasIntroductionText))]
        private string IntroductionText;

        [FoldoutGroup("Introduction"), OdinSerialize, ShowIf(nameof(hasIntroductionText))]
        private Position2D introductionTextPosition;
        
        [Multiline, FoldoutGroup("Introduction"), OdinSerialize, ShowIf(nameof(hasHeaderText))]
        private string headerText;

        [FoldoutGroup("Introduction"), OdinSerialize, ShowIf(nameof(hasHeaderText))]
        private Position2D headerTextPosition;
        
        [OdinSerialize, FoldoutGroup("Introduction"), ShowIf(nameof(hasIntroductionImage))]
        private Sprite introductionImage;
        
        [OdinSerialize, FoldoutGroup("Introduction"), ShowIf(nameof(hasIntroductionImage))]
        private Position2D introductionImagePosition;
        
        [ShowIf(nameof(hasIntroductionAudio)), OdinSerialize, FoldoutGroup("Introduction")]
        private AudioClip introductionAudio;
        
        [FoldoutGroup("Introduction"), HorizontalGroup("Introduction/AnimationType"), OdinSerialize]
        private AnimType animationInType;
        
        [FoldoutGroup("Introduction"), HorizontalGroup("Introduction/AnimationType"), OdinSerialize]
        private AnimType animationOutType;
        
        
        #endregion
        
        private List<Renderer> animatedObjects;
        private GameObject parentObject;
        private GameConstructor gameConstructor;
        
        public async void Init(GameObject introductionParent, GameConstructor constructor)
        {
            parentObject = introductionParent;
            gameConstructor = constructor;
            animatedObjects = new List<Renderer>();
            
            gameConstructor.GetNextButtonText.text = NextButtonText == String.Empty ? "Далее" : NextButtonText;
            
            if (hasIntroductionText)
            {
                TextMeshPro mainTextObject = GameObject.Instantiate(gameConstructor.GetMainTextObject, parentObject.transform);
                mainTextObject.text = IntroductionText;
                mainTextObject.gameObject.SetActive(false);
                animatedObjects.Add(mainTextObject.renderer);
                
                if (introductionTextPosition is Position2D pos)
                {
                   mainTextObject.transform.localPosition = new Vector3(pos.X, pos.Y);
                }
            }

            if (hasIntroductionImage)
            {
                SpriteRenderer image = new GameObject("Introduction Image").AddComponent<SpriteRenderer>();
                image.sprite = introductionImage;
                image.transform.SetParent(introductionParent.transform);
                image.gameObject.SetActive(false);
                animatedObjects.Add(image);
                
                if (introductionImagePosition is Position2D pos)
                {
                    image.transform.localPosition = new Vector3(pos.X, pos.Y);
                }
            }
            
            if (hasHeaderText)
            {
                TextMeshPro headerTextObject = GameObject.Instantiate(gameConstructor.GetHeaderTextObject, parentObject.transform); 
                headerTextObject.text = headerText;
                headerTextObject.gameObject.SetActive(false);
                animatedObjects.Add(headerTextObject.renderer);
                
                if (headerTextPosition is Position2D pos)
                {
                    headerTextObject.transform.localPosition = new Vector3(pos.X, pos.Y);
                }
            }

            // TODO Delete later
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            await SceneIn();
            
            if (hasIntroductionAudio)
            {
                gameConstructor.GetSoundsAudioSource.PlayOneShot(introductionAudio);
            }
            
        }
            
        #region SceneInOut
        public async Task SceneIn()
        {
            foreach (Renderer ob in animatedObjects)
            {
                ob.gameObject.SetActive(true);
                
                if (animationInType == AnimType.Fade)
                {
                    AnimationUtility.FadeIn(ob, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                }
                else if (animationInType == AnimType.Scale)
                {
                    
                    AnimationUtility.ScaleIn(ob.gameObject, animCurve: AnimationCurve.EaseInOut(0,0,1,1));
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public async Task SceneOut()
        {
            foreach (Renderer ob in animatedObjects)
            {
                if (animationOutType == AnimType.Fade)
                {
                    AnimationUtility.FadeOut(ob, animCurve: AnimationCurve.EaseInOut(0, 0, 1, 1));
                }
                else if (animationOutType == AnimType.Scale)
                {
                    AnimationUtility.ScaleOut(ob.gameObject, animCurve: AnimationCurve.EaseInOut(0,0,1,1));
                }
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(false);
            }
            
            GameObject.Destroy(parentObject);
        }
        
        #endregion
    }
}