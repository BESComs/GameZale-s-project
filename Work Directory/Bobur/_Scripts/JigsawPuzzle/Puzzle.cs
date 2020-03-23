using System;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using _Scripts.Utility;

namespace Work_Directory.Bobur._Scripts.JigsawPuzzle
{
    public class Puzzle : MonoBehaviour, ITask
    {
        [SerializeField] private GameObject PuzzleTemplate;
        [SerializeField] private GameObject CompletePicture;
        [SerializeField] private GameObject ShapeObjectsParent;
        [SerializeField] private GameObject EmptyObjectsParent;
        [SerializeField] private GameObject[] Shapes;
        [SerializeField] private Transform[] EmpShapePositions;
        [SerializeField] private Transform[] ShapePositions;
        [SerializeField] private RectTransform buttonPosition;
        private int score;
        private SpriteRenderer completePictureRenderer;

        private void Start()
        {
            completePictureRenderer = CompletePicture.GetComponent<SpriteRenderer>();
            JigsawPuzzleGameController.Instance.ParticleSystem.transform.position = CompletePicture.transform.position;
            Init();
        }

        public void Init()
        {
            
            JigsawPuzzleGameController.Instance.RegisterLessonStart();
            foreach (Transform shapeObject in ShapeObjectsParent.transform)
            {
                Destroy(shapeObject.gameObject);
            }

            completePictureRenderer.color = new Color(1,1,1,0);
            CompletePicture.SetActive(false);
        
            JigsawPuzzleGameController.Instance.PlayParticleSystem(false);
            InstantiateEmptyShapes();
            InstantiateShapes();
            JigsawPuzzleGameController.Instance.GetContinueButton().SetActive(false);
            (JigsawPuzzleGameController.Instance.GetContinueButton().transform as RectTransform).anchoredPosition = buttonPosition.anchoredPosition;
            score = 0;
        }
    
        private void InstantiateShapes()
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                GameObject ob = Instantiate(Shapes[i]);
                ob.transform.position = ShapePositions[i].position;
                ob.GetComponent<ShapeController>().role = Roles.ACTIVE;
                ob.AddComponent<Animator>().runtimeAnimatorController = JigsawPuzzleGameController.Instance.getShapeAniamtor();
                ob.GetComponent<Animator>().enabled = false;
                ob.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                ob.transform.SetParent(ShapeObjectsParent.transform, false);
            }
        }


        private void InstantiateEmptyShapes()
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                GameObject ob = Instantiate(Shapes[i]);
                Destroy(ob.GetComponent<SpriteRenderer>());
                Destroy(ob.GetComponent<SetSpriteToTopOnHover>());
                Destroy(ob.GetComponent<CapsuleCollider2D>());
                Destroy(ob.GetComponent<PolygonCollider2D>());
                Destroy(ob.GetComponent<Animator>());
                ob.transform.position = EmpShapePositions[i].position;
                ob.GetComponent<ShapeController>().role = Roles.PASSIVE;
                ob.transform.SetParent(EmptyObjectsParent.transform, false);
            }
        }

        public GameObject GetEmptyObjectsParent()
        {
            return EmptyObjectsParent;
        }

        public GameObject GetShapeObjectsParent()
        {
            return ShapeObjectsParent;
        }

        public void IncrementScore()
        {
            score++;
            if (CheckTaskComplete())
            {
                TaskCompleted();
            }
        }

        public async void TaskCompleted()
        {
            JigsawPuzzleGameController.Instance.RegisterLessonEnd();
            JigsawPuzzleGameController.Instance.RegisterAnswer(true);
            CompletePictureAppear();
            JigsawPuzzleGameController.Instance.PlayParticleSystem(true);
            await Task.Delay(TimeSpan.FromSeconds(2.3));
            JigsawPuzzleGameController.Instance.GetContinueButton().SetActive(true);
            AnimationUtility.ScaleIn(JigsawPuzzleGameController.Instance.GetContinueButton(), 0.8f);
        
        }

        private async Task CompletePictureAppear()
        {
            CompletePicture.SetActive(true);
            for (float i = 0; i <= 1f; i += Time.deltaTime / 4)
            {
                i = i > 1f ? 1f : i;
                float colorTransparency = AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(i);
                var originalColor = completePictureRenderer.color;
                Color color = new Color(originalColor.r, originalColor.g, originalColor.b, colorTransparency);
                originalColor = color;
                completePictureRenderer.color = originalColor;
                await Awaiters.NextFrame;
            }
        }
    
        public bool CheckTaskComplete()
        {
            return (score == Shapes.Length);
        }
    }
}
