using System.Collections.Generic;
using UnityEngine;
using Work_Directory.Denis.Scripts;
using Work_Directory.Denis.Scripts.Tasks;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    /*
     * script used in games "coloring"
     */
    public class Coloring : TaskParent,ILessonStatsObservable
    {
        public SpriteRenderer selectedColor;
        private List<GameObject> _gameObjects;
        private List<ColoringSetColor> _setColors;
        private Camera _cameraMain;
        private readonly Stack<ClassIndexColor> _undoStack = new Stack<ClassIndexColor>();
        private readonly Stack<ClassIndexColor> _redoStack = new Stack<ClassIndexColor>();
        public Color currentSelectColor;
        public Queue<ISequenceTask> tasks;
        public BrushAnimation currentSelectedBrush;
        private bool _easyMode;
        private Color _startColor;
        
        /*
         * Prepares game on start
         * добавляет все необходимые компоненты к игровым элементом
         */
        private void Awake()
        {
            tasks = new Queue<ISequenceTask>();
            TasksPlayer();
            var tmpParent = transform.parent;
            _setColors = new List<ColoringSetColor>();
            _cameraMain = Camera.main;
            var getColorPieces = tmpParent.GetChild(1);
            _easyMode = true;
            RegisterLessonStart();
            for (var i = 0; i < getColorPieces.childCount; i++)
            {
                var tmpChild = getColorPieces.GetChild(i);
                tmpChild.gameObject.AddComponent<BrushAnimation>().coloring = this;
                tmpChild.gameObject.AddComponent<PolygonCollider2D>();

                if (tmpChild.childCount == 0)
                {
                    tmpChild.gameObject.AddComponent<ColoringGetColor>().coloring = this;
                }
                else
                {
                    _easyMode = false;
                    for (var j = 0; j < getColorPieces.GetChild(i).childCount; ++j)
                    {
                        tmpChild.GetChild(j).gameObject.AddComponent<ColoringGetColor>().coloring = this;
                        tmpChild.GetChild(j).gameObject.AddComponent<PolygonCollider2D>();
                    }    
                }    
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                var tmpChild = transform.GetChild(i);
                _setColors.Add(tmpChild.gameObject.AddComponent<ColoringSetColor>());
                tmpChild.GetComponent<ColoringSetColor>().currentIndex = i;
            }

            _startColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            
            if(tmpParent.childCount < 5) return;
            var tmpButton = tmpParent.GetChild(3).gameObject;
            tmpButton.AddComponent<UndoRedoColoring>().undo = true;
            tmpButton.GetComponent<UndoRedoColoring>().coloring = this;
            tmpButton.AddComponent<PolygonCollider2D>();
            tmpButton = tmpParent.GetChild(4).gameObject;
            tmpButton.AddComponent<UndoRedoColoring>().coloring = this;
            tmpButton.AddComponent<PolygonCollider2D>();
        }

        //если easyMode проверяет все ли элементы окрашены
        private bool TaskChecker()
        {
            foreach (var coloringSetColor in _setColors)
                if (coloringSetColor.spriteRenderer.color == _startColor)
                    return false;
                
            return true;
        }

        
        private async void TasksPlayer()
        {
            while (true)
            {
                if (tasks.Count == 0)
                    await new WaitForUpdate();
                else
                    await tasks.Dequeue().RunTask();    
                
            }
            // ReSharper disable once FunctionNeverReturns
        }

        
        /*
         * game cycle
         * check if user push on mouse or touch screen
         * paints part of the coloring if it's possible 
         */
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            //определение самого верхнего спрайта по Sorting Order - у
            SpriteRenderer sr = null;
            var minSrSo = -1;
            var hits = new RaycastHit2D[50];
            Physics2D.RaycastNonAlloc(_cameraMain.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, hits, 11f);
            ColoringSetColor csc = null;
            foreach (var hit in hits)
            {
                if(hit.transform == null) continue;
                var tmpCsc = hit.transform.GetComponent<ColoringSetColor>();
                if (tmpCsc == null) continue;
                if (tmpCsc.spriteSo <= minSrSo) continue;
                minSrSo = tmpCsc.spriteSo;
                sr = tmpCsc.spriteRenderer;
                csc = tmpCsc;
            }
            //при попадании окрашивает элемент раскраски
            if(csc == null || sr == null || sr.color == currentSelectColor) return;
            foreach (var coloringSetColor in _setColors)
            {
                if (coloringSetColor != csc) continue;
                _undoStack.Push(new ClassIndexColor()
                {
                    color = sr.color,
                    index = csc.currentIndex
                });
                break;
            }
            _redoStack.Clear();
            sr.color = currentSelectColor;
            //игра заканчивается если все элементы были окрашены и выбран easyMode
            if (!_easyMode || !TaskChecker()) return;
            RegisterAnswer(true);
            RegisterLessonEnd();
            transform.parent.GetComponent<TaskParent>().LoadFinalScene();
        }
        
        /*
         * undo button using this method for return to go back one step
         */
        public void UndoColoring()
        {
            if(_undoStack.Count == 0) return;
            var tmp = _undoStack.Pop();
            _redoStack.Push(new ClassIndexColor(){color = _setColors[tmp.index].spriteRenderer.color, index = tmp.index});
            _setColors[tmp.index].spriteRenderer.color = tmp.color;
        }
        /*
         * redo button using this method for return to go one step ahead
         */
        public void RedoColoring()
        {
            if(_redoStack.Count == 0) return;
            var tmp = _redoStack.Pop();
            _undoStack.Push(new ClassIndexColor(){color = _setColors[tmp.index].spriteRenderer.color, index = tmp.index});
            _setColors[tmp.index].spriteRenderer.color = tmp.color;
        }

        /*
         * Show all palettes
         */
        public void ShowPalettes(List<GameObject> gos)
        {
            if (_gameObjects != null)
                foreach (var o in _gameObjects)
                    o.SetActive(false);
            
            if(gos == null) return;
            _gameObjects = gos;
            foreach (var o in _gameObjects)
                o.SetActive(true);
        }

        public int MaxScore
        {
            get => 1;
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

    public class ClassIndexColor
    {
        public int index;
        public Color color;
    }
}
