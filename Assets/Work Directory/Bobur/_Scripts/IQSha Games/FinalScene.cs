using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using AnimType =_Scripts.Utility.AnimationUtility.AnimationType;

namespace Work_Directory.Bobur._Scripts.IQSha_Games
{
    [Serializable]
    public class FinalScene : IGame
    {
        [OdinSerialize] private ParticleSystem particleSystemPrefab;
        [OdinSerialize] private Button resetButton;
        
        // TODO add close game button

        [OdinSerialize, HorizontalGroup("bools")] private bool hasText;
        [OdinSerialize, HorizontalGroup("bools")] private bool hasAudio;

        [OdinSerialize, ShowIf(nameof(hasText))] private string finalSceneText;
        [OdinSerialize, ShowIf(nameof(hasText))] private Vector2 textPosition;
        [OdinSerialize, ShowIf(nameof(hasText))] private float? textSize;

        [OdinSerialize, ShowIf(nameof(hasAudio))]
        private AudioClip audioClip;
        
        /*
        [HorizontalGroup("Animations/AnimationType"), OdinSerialize, PropertyOrder(99), TitleGroup("Animations", GroupID = "Animations")]
        private AnimType animationInType;
        
        [HorizontalGroup("Animations/AnimationType"), OdinSerialize, PropertyOrder(99), TitleGroup("Animations", GroupID = "Animations")]
        private AnimType animationOutType;
        
        */
        
        private GameConstructor gameConstructor;
        private GameObject parentObject;
        private ParticleSystem particleSystem;
        private List<Renderer> animatedObjects;
        
        public async void Init(GameConstructor constructor, GameObject parentObject)
        {
            gameConstructor = constructor;
            this.parentObject = parentObject;
            animatedObjects = new List<Renderer>();
            
            if (hasText)
            {
                TextMeshPro textObject = GameObject.Instantiate(gameConstructor.GetMainTextObject);
                textObject.text = finalSceneText;
                
                textObject.gameObject.SetActive(false);
                
                textObject.transform.localPosition = textPosition;
                textObject.transform.SetParent(parentObject.transform);
                
                animatedObjects.Add(textObject.renderer);

                if (textSize.HasValue)
                {
                    textObject.fontSize = textSize.Value;
                }
            }
            
            if (hasAudio)
            {
                gameConstructor.SetCurrentAudioClip(audioClip);
                gameConstructor.PlayCurrentAudioClip();
            }
            
            if(particleSystemPrefab != null)
            {
                particleSystem = GameObject.Instantiate(particleSystemPrefab);
                particleSystem.transform.SetParent(parentObject.transform);

                particleSystem.Play();
            }
            constructor.GetNextButton.interactable = false;

            SceneIn();
        }

        public bool CheckAnswer() => true;

        public async Task SceneIn()
        {
            resetButton.gameObject.SetActive(true);
            AnimationUtility.ScaleIn(resetButton.gameObject);
            foreach (Renderer animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(true);
                AnimationUtility.FadeIn(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        public async Task SceneOut()
        {
            AnimationUtility.ScaleOut(resetButton.gameObject);
            foreach (Renderer animatedObject in animatedObjects)
            {
                AnimationUtility.FadeOut(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        public void ChangeStateAfterAnswer()
        {
            
        }
    }
}