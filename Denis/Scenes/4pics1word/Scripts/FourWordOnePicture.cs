using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;
using Work_Directory.Denis.Scripts.Tasks;
using Random = UnityEngine.Random;

namespace Work_Directory.Denis.Scenes._4pics1word.Scripts
{
    public class FourWordOnePicture : TaskParent, ILessonStatsObservable
    {
        //задний спрайт
        public Transform boxBackFonSprite;
        //Выполняются ли асинхронные действия
        [HideInInspector]public bool inTask;
        //список уровней
        ////один уровень - правильный ответ, количество букв из которых будет составлять слово, список изображений
        public List<FourWordOnePictureLevel> fourWordOnePictureLevels;
        //индекс текущего уровня
        private int _currentLevel;
        //родительский объект для всех объектов текущего уровня
        private Transform _currentLevelParent;
        //размер картинок
        public Vector2 picsSize;
        //отступ между картинками
        public Vector2 picsMargin;
        //префаб текста
        public Transform textMeshProPrefab;
        //отступ между букв
        public Vector2 charsMargins;
        //Размер ячейки для буквы
        private float _charBoxSize;
        //рамка для слова
        public Sprite wordFrame;
        //выбранные буквы на текущем уровне
        private readonly List<char?> _curLevelChars = new List<char?>();
 
        private void Start()
        {
            _charBoxSize = textMeshProPrefab.GetComponent<TextMeshPro>().rectTransform.sizeDelta.x;
            RegisterLessonStart();
            LoadNewLevel();
        }

        //Генерация нового уровня
        private async void LoadNewLevel()
        {
            _curLevelChars.Clear();
            _currentLevelParent = new GameObject().transform;
            fourWordOnePictureLevels[_currentLevel].correctAnswer =
                fourWordOnePictureLevels[_currentLevel].correctAnswer.ToUpper();
            //создание изображений на текущий уровень
            foreach (var t in fourWordOnePictureLevels[_currentLevel].pics)
            {
                var tmpGo = new GameObject();
                var sr = tmpGo.AddComponent<SpriteRenderer>();
                sr.sprite = t;
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size *= Mathf.Min(picsSize.x / sr.size.x, picsSize.y / sr.size.y);
                tmpGo.transform.position = Vector3.zero;
                tmpGo.transform.SetParent(_currentLevelParent,false);
            }
            //todo можно сделать более коротко
            //ставит изображения на правильные позиции
            switch (_currentLevelParent.childCount)
            {
                case 1:
                    _currentLevelParent.GetChild(0).localPosition = new Vector2(0, 2);
                    break;
                case 2:
                    _currentLevelParent.GetChild(0).localPosition = new Vector2(-(picsSize.x + picsMargin.x) / 2f, 2);
                    _currentLevelParent.GetChild(1).localPosition = new Vector2((picsSize.x + picsMargin.x) / 2f, 2);
                    break;
                case 3:
                    _currentLevelParent.GetChild(0).localPosition =
                        new Vector2(-(picsSize.x + picsMargin.x) / 2f, 2 + (picsSize.y + picsMargin.y) / 2f);
                    _currentLevelParent.GetChild(1).localPosition =
                        new Vector2((picsSize.x + picsMargin.x) / 2f, 2 + (picsSize.y + picsMargin.y) / 2f);
                    _currentLevelParent.GetChild(2).localPosition = new Vector2(0, 2 - (picsSize.y + picsMargin.y) / 2f);
                    break;
                case 4:
                    _currentLevelParent.GetChild(0).localPosition =
                        new Vector2(-(picsSize.x + picsMargin.x) / 2f, 2 + (picsSize.y + picsMargin.y) / 2f);
                    _currentLevelParent.GetChild(1).localPosition =
                        new Vector2((picsSize.x + picsMargin.x) / 2f, 2 + (picsSize.y + picsMargin.y) / 2f);
                    _currentLevelParent.GetChild(2).localPosition =
                        new Vector2(-(picsSize.x + picsMargin.x) / 2f, 2 - (picsSize.y + picsMargin.y) / 2f);
                    _currentLevelParent.GetChild(3).localPosition =
                        new Vector2((picsSize.x + picsMargin.x) / 2f, 2 - (picsSize.y + picsMargin.y) / 2f);
                    break;
            }
            //создание боксов в которые будут закидываться букву при выборе
            var startMarginX1 =
                fourWordOnePictureLevels[_currentLevel].correctAnswer.Length * (charsMargins.x + _charBoxSize) -
                charsMargins.x;
            var startMarginX2 = (fourWordOnePictureLevels[_currentLevel].numberOfChars - 5) * (charsMargins.x + _charBoxSize) - charsMargins.x;
            
            var tmpGo2 = new GameObject();
            var sr2 = tmpGo2.AddComponent<SpriteRenderer>();
            sr2.sprite = wordFrame;
            sr2.drawMode = SpriteDrawMode.Sliced;
            sr2.size = new Vector2(startMarginX1, _charBoxSize);
            tmpGo2.transform.position = new Vector3(0,-.75f,0);
            for (var i = 0; i < fourWordOnePictureLevels[_currentLevel].correctAnswer.Length; ++i)
            {
                var tmpGo = Instantiate(textMeshProPrefab, _currentLevelParent, false);
                tmpGo.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                var rc = tmpGo.gameObject.AddComponent<RemoveChar>();
                rc.fourWordOnePicture = this;
                rc.curCharBoxId = i + fourWordOnePictureLevels[_currentLevel].pics.Count;
                tmpGo.localPosition = new Vector3(-startMarginX1/2f + _charBoxSize / 2f + i * (charsMargins.x + _charBoxSize),-.75f,0);
                _curLevelChars.Add(null);
            }
            //создание боксов и букв для выбора
            startMarginX1 = Mathf.Min(fourWordOnePictureLevels[_currentLevel].numberOfChars, 5) * (charsMargins.x + _charBoxSize) - charsMargins.x;
        
            for (var i = 0; i < fourWordOnePictureLevels[_currentLevel].numberOfChars; ++i)
            {
                var tmpGo = Instantiate(textMeshProPrefab, _currentLevelParent, false);
                tmpGo.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                var sr = boxBackFonSprite.GetComponent<SpriteRenderer>();
                boxBackFonSprite.localScale = new Vector2(1f / sr.size.x, 1f / sr.size.y);
                sr.color = Color.black;
                var srColor = sr.color;
                srColor.a = .5f;
                sr.color = srColor;
                Instantiate(boxBackFonSprite).SetParent(tmpGo);
                var csc = tmpGo.gameObject.AddComponent<CharSelectController>();
                csc.fourWordOnePicture = this;
                csc.childId = i;
                var textMeshPro = tmpGo.GetComponent<TextMeshPro>();
                var color = textMeshPro.color;
                color.r = (Random.value + 1) / 2f;
                color.b = (Random.value + 1) / 2f;
                color.g = (Random.value + 1) / 2f;
                textMeshPro.color = color;
                //рандомное создание букв
                var с = (char) ('А' + (int)(Random.value * 32));
                if (Random.value > .9696f)
                    с = 'Ё';

                if (i < fourWordOnePictureLevels[_currentLevel].correctAnswer.Length)
                    с = fourWordOnePictureLevels[_currentLevel].correctAnswer[i];
                
                csc.curChar = с;
                
                textMeshPro.text = с.ToString();

                tmpGo.transform.localPosition = i < 5
                    ? new Vector3(-startMarginX1 / 2f + _charBoxSize / 2f + i * (charsMargins.x + _charBoxSize), -2, 0)
                    : new Vector3(-startMarginX2 / 2f + _charBoxSize / 2f + (i - 5) * (charsMargins.x + _charBoxSize),
                        -2 - (charsMargins.y + _charBoxSize), 0);
            }
            RandomPermutation();
            tmpGo2.transform.SetParent(_currentLevelParent);
            inTask = true;
            _currentLevelParent.localScale = new Vector3(.1f,.1f,.1f);
            //анимированное появление уровня
            await new Work_Directory.Denis.Scripts.Scaling(_currentLevelParent,false,AnimationCurve.EaseInOut(0,1,.75f,10f)).RunTask();
            
            inTask = false;
        }

        //рандомная перестановка местами
        private void RandomPermutation()
        {
            var pref = fourWordOnePictureLevels[_currentLevel].pics.Count +
                       fourWordOnePictureLevels[_currentLevel].correctAnswer.Length;
            for (var i = 0; i < fourWordOnePictureLevels[_currentLevel].numberOfChars; ++i)
            {
                var a = Random.Range(0, i);
                var tmp = _currentLevelParent.GetChild(pref + a).localPosition;
                _currentLevelParent.GetChild(pref + a).localPosition =
                    _currentLevelParent.GetChild(pref + i).localPosition;
                _currentLevelParent.GetChild(pref + i).localPosition = tmp;
            }
        }

        //проверка ответа игрока вызывается когда игрок заполнил все ячейки для букв
        private async void CheckAnswer()
        {
            //ответ игрока
            var s = "";
            foreach (var curLevelChar in _curLevelChars)
            {
                if (curLevelChar != null)
                    s += curLevelChar;
                else
                    return;
                
            }
            //сравнение строк
            if (!string.Equals(s, fourWordOnePictureLevels[_currentLevel].correctAnswer,
                StringComparison.CurrentCultureIgnoreCase))
            {
                inTask = true;
                RegisterAnswer(false);
                await new Shaking(_currentLevelParent.transform, AnimationCurve.EaseInOut(0, 0, .5f, 1), Vector2.left, 1.3f).RunTask();
                inTask = false;
                return;
            }
            RegisterAnswer(true);
            //отгрузка текущего уровня
            _currentLevel++;
            inTask = true;
            await new Work_Directory.Denis.Scripts.Scaling(_currentLevelParent,false, AnimationCurve.EaseInOut(0,1,.75f,.01f)).RunTask();
            if(this == null) return;
            inTask = false;
            Destroy(_currentLevelParent.gameObject);
            //загрузка следующего уровня если текущий уровень не последний иначе загрузка финальной сцены
            if (_currentLevel == fourWordOnePictureLevels.Count)
            {
                RegisterLessonEnd();
                LoadFinalScene();
                return;
            }
            LoadNewLevel();
        }

        //добавляется выбранная буква
        public void AddChar(char c, int childId)
        {
            var tmpBool = false;
            //добавление буквы в первый пустой слот
            for (var i = 0; i < _curLevelChars.Count; i++)
            {
                if (_curLevelChars[i] != null) continue;
                _curLevelChars[i] = c;
                var tmpTransform = _currentLevelParent.GetChild(fourWordOnePictureLevels[_currentLevel].pics.Count + i);
                tmpTransform.GetComponent<TextMeshPro>().text = c.ToString();
                tmpTransform.GetComponent<RemoveChar>().charBoxId =
                    fourWordOnePictureLevels[_currentLevel].pics.Count +
                    fourWordOnePictureLevels[_currentLevel].correctAnswer.Length + childId;
                tmpBool = true;
                break;
            }
            //если буква была добавлена то она исчезает из выбранного места и проверяется ответ
            if (!tmpBool) return;
            _currentLevelParent
                .GetChild(fourWordOnePictureLevels[_currentLevel].pics.Count +
                          fourWordOnePictureLevels[_currentLevel].correctAnswer.Length + childId).gameObject
                .SetActive(false);
            CheckAnswer();
        }

        //удаление выбранной буквы
        public void RemoveChar(int childId1, int childId2)
        {
            var tmpTransform = _currentLevelParent.GetChild(childId1);
            tmpTransform.gameObject.SetActive(true);
            _currentLevelParent.GetChild(childId2).GetComponent<TextMeshPro>().text = "_";
            _curLevelChars[childId2 - fourWordOnePictureLevels[_currentLevel].pics.Count] = null;
        }

        public int MaxScore
        {
            get => fourWordOnePictureLevels.Count; 
            set {  }
        }

        public void RegisterAnswer(bool isAnswerRight){
            if (!isAnswerRight) return;
            LessonStatistic.SetScore(1);
            LessonStatistic.SetRight(isAnswerRight);
		}

		public void RegisterLessonStart(){
			LessonStatistic.SetStartLessonTime();
		}
		
		public void RegisterLessonStart(int lessonNumber)  //?
		{
			throw new NotImplementedException();
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
    public class FourWordOnePictureLevel
    {
        public List<Sprite> pics;
        public string correctAnswer;
        [Range(1,15)]
        public int numberOfChars;
    }
}