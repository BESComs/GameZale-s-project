using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Multiple_Choice
{
    [Serializable]
    public class MultipleChoice : IGame
    {
        [TitleGroup("Multiple Choice game properties", GroupID = "MultipleChoice")]
        
        #region Options
        
        [OdinSerialize, BoxGroup("MultipleChoice/Options"), HideReferenceObjectPicker]
        //список содержащий элементы которые лежат на одной линии назначается пользователем
        private List<List<CellWithID>> options = new List<List<CellWithID>>();
        
        //генерация id элементам игры
        [Button("Generate IDs", ButtonSizes.Medium), BoxGroup("MultipleChoice/Options", false)]
        private void SetOptionsID()
        {
            int count = 0;
            foreach (List<CellWithID> list in options)
            {
                foreach (CellWithID cell in list)
                {
                    cell.ID = count++;
                }
            }
        }
        
        #endregion

        [OdinSerialize] private GameObject TextPrefab;
        [OdinSerialize]
        //стартовая позиция
        private Vector2 StartPosition;
        [OdinSerialize]
        //отступ
        private Vector2 Margin;

        //стандартные размеры элементов используется если не назначили размер
        [OdinSerialize] private Vector2 DefaultSpriteSize = new Vector2(2f,2f);
        [OdinSerialize] private Vector2 DefaultTextSize = new Vector2(2f,1.2f);
        
        
        #region Answers
        
        [OdinSerialize, OnValueChanged(nameof(SetAnswersListSize)), ValidateInput(nameof(ValidateMaximumChoicesInput), "There must be at least 1 choice.")]
        [BoxGroup("MultipleChoice/Answers", false)]
        //максимальное количество элементов которые можно выбрать
        private int maximumChoices = 1;
        [OdinSerialize, BoxGroup("MultipleChoice/Answers", false), ValidateInput(nameof(ValidateCorrectAnswerNoDuplicate), "Answers must have unique ID")]
        //правильные ответы 
        private List<int> correctAnswers = new List<int>();
        //ответы игрока
        private List<int> usersAnswers;
        //список элементов
        private List<OptionsNavigator> usersChosenOptions;
        
        #endregion
        
        private GameConstructor gameConstructor;
        private GameObject parentObject;
        
        private GameObject allObjects;
        private List<OptionsNavigator> optionNavigatorsList;
       
        //метод который вызывается в начале игры
        public IGame Init(GameConstructor constructor, GameObject questionParent)
        {
            gameConstructor = constructor;
            parentObject = questionParent;
            
            usersAnswers = new List<int>();
            usersChosenOptions = new List<OptionsNavigator>();
            optionNavigatorsList = new List<OptionsNavigator>();
         
            allObjects = new GameObject("Options Parent");
            allObjects.transform.SetParent(parentObject.transform);
            
            CreateStartScene();
            
            return this;
        }
        
        //создание начальной сцены 
         private void CreateStartScene()
        {
            var resetPos = new List<bool>();
            var pos = new List<Vector2>();
            var marginY = new List<float>();
            var marginX = new List<float>();
            
            //создание выбираемых элементов игры
            foreach (var cellsInLine in options)
            {
                //текущий сдвиг
                var curSumX = 0f;
                var curMaxY = 0f;
                foreach (var cell in cellsInLine)
                {
                    GameObject tmpGo;
                    //выбираемый элемент игры имеет текст или спрайт
                    switch (cell.textOrSprite)
                    {
                        //cоздание элемента с текстом
                        case TextOrSprite.Text:
                            tmpGo = GameObject.Instantiate(TextPrefab);
                            tmpGo.GetComponent<TextMeshPro>().text = cell.String;
                            var rt = tmpGo.GetComponent<RectTransform>();
                            //изменение размера если вкл опция
                            rt.sizeDelta = cell.AdvancedOptions ? cell.TextSize : DefaultTextSize;
                            //подсчет текущего сдвига
                            curSumX += cell.AdvancedOptions ? 0 : rt.sizeDelta.x + Margin.x;
                            curMaxY = cell.AdvancedOptions ? curMaxY : Mathf.Max(rt.sizeDelta.y, curMaxY);
                            //сохранение размера
                            cell.CellSize = rt.sizeDelta;
                            
                            break;
                        case TextOrSprite.Sprite:
                            //cоздание элемента с спрайтом
                            tmpGo = new GameObject();
                            var sr = tmpGo.AddComponent<SpriteRenderer>();
                            sr.sprite = cell.Sprite;
                            sr.drawMode = SpriteDrawMode.Sliced;
                            //изменение размера с учетом соотношения сторон
                            sr.size *= cell.AdvancedOptions
                                ? cell.UseSpriteSize
                                    ? 1
                                    : Mathf.Min(cell.SpriteSize.x / sr.size.x,
                                        cell.SpriteSize.y / sr.size.y)
                                : Mathf.Min(DefaultSpriteSize.x / sr.size.x, DefaultSpriteSize.y / sr.size.y);

                            curSumX += cell.AdvancedOptions && cell.Position.HasValue ? 0: sr.size.x + Margin.x;
                            curMaxY = cell.AdvancedOptions && cell.Position.HasValue ? curMaxY : Mathf.Max(sr.size.y, curMaxY);
                            cell.CellSize = sr.size;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    tmpGo.transform.SetParent(allObjects.transform, false);
                    tmpGo.AddComponent<ID>().Id = cell.ID;
                    tmpGo.SetActive(false);
                    
                    var optionNavigator = GameObject.Instantiate(gameConstructor.GetOptionsNavigator.gameObject).GetComponent<OptionsNavigator>();
                    optionNavigator.SetSize(cell.CellSize.x + 0.2f, cell.CellSize.y + 0.2f);
                    optionNavigator.transform.SetParent(tmpGo.transform, false);
                    optionNavigator.SetOnMuseDown(OnOptionClick);
                    
                    resetPos.Add(cell.AdvancedOptions && cell.Position.HasValue);
                    
                    pos.Add(cell.Position ?? new Vector2(0,0));
                }
                
                marginX.Add(curSumX - Margin.x);
                marginY.Add(curMaxY + Margin.y + (marginY.Count == 0 ? 0 : marginY[marginY.Count - 1]));
            }
            //Корректирование позиций
            var counter = 0;
            for (var i = 0; i < options.Count; i++)
            {
                var startMarginX = -marginX[i]/2f;
                var lastXPos = 0f;
                foreach (var cell in options[i])
                {
                    var tmpGo = allObjects.transform.GetChild(counter);
                    tmpGo.localPosition = resetPos[counter] ? pos[counter] : new Vector2(startMarginX + lastXPos +
                            cell.CellSize.x / 2f, i != 0 ? (marginY[i] + marginY[i - 1] - Margin.y) / -2f : cell.CellSize.y / -2f);
                    lastXPos = tmpGo.localPosition.x - startMarginX + Margin.x + cell.CellSize.x / 2f;
                    counter++;
                }
            }
            allObjects.transform.localPosition = StartPosition;
        }
         
         //нажатие на выбираемый элемент
        private void OnOptionClick(OptionsNavigator option)
        {
            //если не выбран - выбрать
            if (!option.isPainted)
            {
                //если выбрано максимальное число элементов удалить первый из выбранных
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
                //если выбран удалить из выбранных
                int optionsIndex = usersChosenOptions.IndexOf(option);
                usersChosenOptions.RemoveAt(optionsIndex);
                usersAnswers.RemoveAt(optionsIndex);
            }

            gameConstructor.GetNextButton.interactable = usersAnswers.Count == maximumChoices;
        }

        //появление на сцену
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

            await new WaitForSeconds(1f);
        }

        //исчезновение из сцены
        public async Task SceneOut()
        {
            for (int i = allObjects.transform.childCount - 1; i >= 0; i--)
            {
                AnimationUtility.ScaleOut(allObjects.transform.GetChild(i).gameObject, 2.7f);
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }

            await new WaitForSeconds(1f);
        }

        //изменения состояни после ответа игрока
        public void ChangeStateAfterAnswer()
        {
            foreach (OptionsNavigator navigator in optionNavigatorsList)
            {
                navigator.isDisabled = true;
            }
        }

        //проверка ответа
        public bool CheckAnswer()
        {
            
            if(usersAnswers.Count != maximumChoices)
                throw new Exception($"Expected total {maximumChoices} answers. But got {usersAnswers.Count} answers.");
            //попарное сравнение с списком правильных ответов 
            var totalCorrect = correctAnswers.Where((t, i) => correctAnswers.Contains(usersAnswers[i])).Count();

            return totalCorrect == maximumChoices;
        }
        
        #region Odin Related
        
        //если пользователь ввел некорректные данные относительно максимального количества элементов которые можно выбрать и списком правельных ответов
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
        
        //используется когда игроку не нужно ничего выбирать
        private bool ValidateMaximumChoicesInput(int value) => value > 0;

        //есть ли одинаковые элементы в списке используется для проверки ввода правильных ответов
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
    
    public enum TextOrSprite
    {
        Text, Sprite
    }

    [Serializable]
    public class CellWithID
    {
        [HideInInspector] public Vector2 CellSize;
        public TextOrSprite textOrSprite;
        
        [ShowIf(nameof(textOrSprite), TextOrSprite.Text)]
        public string String;
        [ShowIf(nameof(textOrSprite), TextOrSprite.Sprite), PreviewField(50)]
        public Sprite Sprite;
        
        [HorizontalGroup("bools")]
        public bool AdvancedOptions;
        [HorizontalGroup("bools"), ShowIf(nameof(AdvancedOptions)), ShowIf(nameof(textOrSprite), TextOrSprite.Sprite)]
        public bool UseSpriteSize;
        
        [ShowIf(nameof(AdvancedOptions))]
        public Vector2? Position;
        [ShowIf(nameof(AdvancedOptions)), ShowIf(nameof(textOrSprite), TextOrSprite.Sprite), HideIf(nameof(UseSpriteSize))]
        public Vector2 SpriteSize;
        [ShowIf(nameof(AdvancedOptions)), ShowIf(nameof(textOrSprite), TextOrSprite.Text)]
        public Vector2 TextSize;
        [ReadOnly]
        public int ID;
    }
}