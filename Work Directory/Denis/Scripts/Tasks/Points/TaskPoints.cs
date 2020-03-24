using System.Collections;
using UnityEngine;
using Work_Directory;
using Work_Directory.Denis.Scripts.Tasks;

namespace Tasks.Points
{
    public class TaskPoints : TaskParent, ITask, ILessonStatsObservable
    {   
        private void Awake()
        {
            _drawPointsMax = GetComponentsInChildren<PointsPoint>().Length;
            Object.gameObject.SetActive(false);
            RegisterLessonStart();
        }
        
        [Space]
        public int ActiveNumber = 0;
        public Color PointSecondColor = Color.red;
        public LineRenderer Lines;
        public SpriteRenderer Object;
        public GameObject FirstGuide;
        
        private bool _complete;
        
        private int _drawPointsMax;

        public void DrawLine(PointsPoint point)
        {
            if (FirstGuide.activeInHierarchy)
                FirstGuide.SetActive(false);
            ActiveNumber++;
            Lines.positionCount = ActiveNumber;
            StartCoroutine(DrawLine(ActiveNumber - 1, Lines.GetPosition((ActiveNumber > 1) ? ActiveNumber - 2 : 0), point.transform.position - new Vector3(0,0,1)));
            if (ActiveNumber == _drawPointsMax)
            {
                _complete = true;
                Lines.loop = true;
                Timeout.Set(this, 1, () =>
                {
                    Object.gameObject.SetActive(true);
                    Timeout.Set(this, 2, () =>
                    {
                        Lines.gameObject.SetActive(false);
                        RegisterAnswer(true);
                        RegisterLessonEnd();
                        LoadFinalScene();
                    });
                });
            }
        }

        IEnumerator DrawLine(int id, Vector3 from, Vector3 to)
        {
            float dur = 0.5f;
            float elp = 0f;
            while (elp < dur)
            {
                elp = Mathf.Clamp(elp + Time.deltaTime, 0, dur);
                Lines.SetPosition(id, Vector3.Lerp(from, to, elp / dur));
                yield return null;
            }
        }
        
        public bool CheckTaskComplete()
        {
            return _complete;
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
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
		
		public void RegisterLessonStart(int lessonNumber)  //?
		{
			throw new System.NotImplementedException();
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
