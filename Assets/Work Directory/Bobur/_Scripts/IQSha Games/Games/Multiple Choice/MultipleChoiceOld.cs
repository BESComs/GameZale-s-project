using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.MultipleChoice
{
    //старая версия 
    [Serializable]
    public class MultipleChoiceOld : IGame
    {
        [TitleGroup("Multiple Choice game properties", GroupID = "MultipleChoice")]
        
        #region Options
        
        [OdinSerialize, BoxGroup("MultipleChoice/Options"), HideReferenceObjectPicker]
        private List<List<SpriteID>> options = new List<List<SpriteID>>();
        
        [Button("Generate IDs", ButtonSizes.Medium), BoxGroup("MultipleChoice/Options", false)]
        private void SetOptionsID()
        {
            int count = 0;
            foreach (List<SpriteID> list in options)
            {
                foreach (SpriteID spriteId in list)
                {
                    spriteId.ID = count++;
                }
            }
        }
        
        #endregion
        
        [OdinSerialize]
        private Position2D OptionsPosition;

        [OdinSerialize, HorizontalGroup("MultipleChoice/Margins")]
        private float MarginX, MarginY;
        
        [OdinSerialize, HorizontalGroup("MultipleChoice/HeightWidth")]
        private float OptionsHeight, OptionsWidth;
        
        #region Answers
        
        [OdinSerialize, OnValueChanged(nameof(SetAnswersListSize)), ValidateInput(nameof(ValidateMaximumChoicesInput), "There must be at least 1 choice.")]
        [BoxGroup("MultipleChoice/Answers", false)]
        private int maximumChoices = 1;
        
        [OdinSerialize, BoxGroup("MultipleChoice/Answers", false), ValidateInput(nameof(ValidateCorrectAnswerNoDuplicate), "Answers must have unique ID")]
        private List<int> correctAnswers = new List<int>();
        private List<int> usersAnswers;
        private List<OptionsNavigator> usersChosenOptions;
        
        #endregion
        
        private GameConstructor gameConstructor;
        private GameObject parentObject;
        
        // It is optional. You can delete it in future if you don't use it
        private List<List<GameObject>> lines;
        private GameObject allObjects;
        private List<OptionsNavigator> optionNavigatorsList;
        
        
        public IGame Init(GameConstructor constructor, GameObject questionParent)
        {
            gameConstructor = constructor;
            parentObject = questionParent;
            
            usersAnswers = new List<int>();
            usersChosenOptions = new List<OptionsNavigator>();
            optionNavigatorsList = new List<OptionsNavigator>();
            
            ThrowElementsOnScreen throwElements = new ThrowElementsOnScreen();
            throwElements.Setup(OptionsPosition, options, OptionsHeight, OptionsWidth, MarginX, MarginY);
            
            allObjects = throwElements.Throw();
            allObjects.transform.SetParent(parentObject.transform);
            
            lines = throwElements.ObjectsList;
            
            foreach (Transform option in allObjects.transform)
            {
                option.gameObject.SetActive(false);
                OptionsNavigator optionNavigator = GameObject.Instantiate(gameConstructor.GetOptionsNavigator.gameObject).GetComponent<OptionsNavigator>();
                optionNavigatorsList.Add(optionNavigator);
                optionNavigator.SetSize(OptionsHeight + 0.2f, OptionsWidth + 0.2f);
                optionNavigator.transform.SetParent(option, false);
                optionNavigator.SetOnMuseDown(OnOptionClick);
            }
            
            return this;
        }

        private void OnOptionClick(OptionsNavigator option)
        {
            if (!option.isPainted)
            {
                if (usersAnswers.Count == maximumChoices)
                {
                    usersChosenOptions[0].Clear();
                    
                    usersChosenOptions.RemoveAt(0);
                    usersAnswers.RemoveAt(0);
                }
                
                usersChosenOptions.Add(option);
                usersAnswers.Add(option.transform.parent.GetComponent<ID>().Id);
            }
            else
            {
                int optionsIndex = usersChosenOptions.IndexOf(option);
                usersChosenOptions.RemoveAt(optionsIndex);
                usersAnswers.RemoveAt(optionsIndex);
            }

            gameConstructor.GetNextButton.interactable = usersAnswers.Count == maximumChoices;
        }

        public async Task SceneIn()
        {
            /* Used For randomizing
             
            List<Transform> tmpListForRandomize = new List<Transform>();
            */
            
            foreach (Transform transform in allObjects.transform)
            {
                // Used for randomizing
                // tmpListForRandomize.Add(transform);

                transform.gameObject.SetActive(true);
                AnimationUtility.ScaleIn(transform.gameObject, 2f);
                
                await Task.Delay(TimeSpan.FromSeconds(0.2f));
            }
            
            /** Used for randomly add elements to scene
             * Use it if you need
             *
            System.Random random = new System.Random();
            while (tmpListForRandomize.Count != 0)
            {
                GameObject randomElement = tmpListForRandomize[random.Next(0, tmpListForRandomize.Count - 1)].gameObject;
                randomElement.SetActive(true);
                AnimationUtility.ScaleIn(randomElement);
                
                gameConstructor.GetCurrentQuestion()?.animatedObjects.Add(randomElement.GetComponent<Renderer>());
                
                tmpListForRandomize.Remove(randomElement.transform);
                await Task.Delay(TimeSpan.FromSeconds(0.2f));
            }
            */
        }

        public async Task SceneOut()
        {
            for (int i = allObjects.transform.childCount - 1; i >= 0; i--)
            {
                AnimationUtility.ScaleOut(allObjects.transform.GetChild(i).gameObject, 2.7f);
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        public void ChangeStateAfterAnswer()
        {
            foreach (OptionsNavigator navigator in optionNavigatorsList)
            {
                navigator.isDisabled = true;
            }
        }

        public bool CheckAnswer()
        {
            if(usersAnswers.Count != maximumChoices)
                throw new Exception($"Expected total {maximumChoices} answers. But got {usersAnswers.Count} answers.");
            
            int totalCorrect = 0;
            for (int i = 0; i < correctAnswers.Count; i++)
            {
                if (correctAnswers.Contains(usersAnswers[i]))
                    totalCorrect++;
            }

            return totalCorrect == maximumChoices;
        }
        
        #region Odin Related
        
        private void SetAnswersListSize()
        {
            if (correctAnswers == null)
            {
                correctAnswers = new List<int>(maximumChoices);
                return;
            }

            if (correctAnswers.Count < maximumChoices)
            {
                while (correctAnswers.Count != maximumChoices)
                {
                    correctAnswers.Add(0);    
                }
            }
            else if(correctAnswers.Count > maximumChoices)
            {
                while (correctAnswers.Count != maximumChoices)
                {
                    correctAnswers.RemoveAt(correctAnswers.Count - 1);    
                }
            }
        }

        private bool ValidateMaximumChoicesInput(int value) => value > 0;

        private bool ValidateCorrectAnswerNoDuplicate(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i] == list[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        #endregion
        
    }
}