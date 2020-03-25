using DefaultNamespace;
using UnityEngine;

namespace Work_Directory.Bobur._Scripts.JigsawPuzzle
{
    public class JigsawDragHandler : MonoBehaviour
    {
        private Vector3 originalPosition;
        private Vector3 previousMousePosition;
        public bool movingBack { get; private set; }
        private ShapeController _shapeController;
	
        // Use this for initialization
        private void Start ()
        {
            _shapeController = GetComponent<ShapeController>();
            movingBack = false;
        }

        private void Update()
        {
            if (movingBack)
            {
                float step = 15 * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, step);
                if (transform.position == originalPosition)
                {
                    movingBack = false;
                    _shapeController.SetSortingLayer(0);
                    _shapeController.SetScale(0.8f);
                }
            }
        }
	
        private void OnMouseDown()
        {
            if (movingBack)
                return;

            if (GetComponent<SetSpriteToTopOnHover>() != null)
            {
                GetComponent<SetSpriteToTopOnHover>().enabled = false;
            }
            
            if (_shapeController.role == Roles.ACTIVE)
            {
                DisableTriggerOfUnnecessaryEmptyObjects();
                _shapeController.SetScale(1);
                originalPosition = transform.position;
                previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _shapeController.SetSortingLayer(1);
            }
        }

        public void OnMouseDrag()
        {
            if (_shapeController.role == Roles.ACTIVE)
            {
                Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - previousMousePosition;
                transform.position += distance;
                previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public void OnMouseUp()
        {
            if (_shapeController.role == Roles.ACTIVE)
            {
                EnableTriggerOfEmptyObjects();
                GameObject collidedGameObject = _shapeController.CollidedGameObject;
                if (collidedGameObject != null && _shapeController.ID == collidedGameObject.GetComponent<ShapeController>().ID)
                {
                    transform.position = collidedGameObject.transform.position;

                    GetComponent<Animator>().enabled = true;
                    GetComponent<Animator>().SetBool("FoundPlace", true);
                    JigsawPuzzleGameController.Instance.GetCurrentPuzzle().GetComponent<Puzzle>().IncrementScore();
                    _shapeController.role = Roles.PASSIVE;
                    _shapeController.disableTrigger();
                    Destroy(collidedGameObject);
                    return;
                }
                movingBack = true;
                
                if (GetComponent<SetSpriteToTopOnHover>() != null)
                {
                    GetComponent<SetSpriteToTopOnHover>().enabled = true;
                }
            }
        }
        
        
        private void DisableTriggerOfUnnecessaryEmptyObjects()
        {
            GameObject emptyObjectsParent = JigsawPuzzleGameController.Instance.GetCurrentPuzzle().GetComponent<Puzzle>().GetEmptyObjectsParent();
            GameObject shapeObjectsParent = JigsawPuzzleGameController.Instance.GetCurrentPuzzle().GetComponent<Puzzle>().GetShapeObjectsParent();
            
            foreach (Transform emptyObject in emptyObjectsParent.transform)
            {
                if (emptyObject.GetComponent<ShapeController>().ID != _shapeController.ID)
                    emptyObject.gameObject.GetComponent<ShapeController>().disableTrigger();
            }

            foreach (Transform shapeObject in shapeObjectsParent.transform)
            {
                if (shapeObject.GetComponent<ShapeController>().ID != _shapeController.ID)
                    shapeObject.gameObject.GetComponent<ShapeController>().disableTrigger();
            }
        }
	
        private void EnableTriggerOfEmptyObjects()
        {
            GameObject emptyObjectsParent = JigsawPuzzleGameController.Instance.GetCurrentPuzzle().GetComponent<Puzzle>().GetEmptyObjectsParent();
            GameObject shapeObjectsParent = JigsawPuzzleGameController.Instance.GetCurrentPuzzle().GetComponent<Puzzle>().GetShapeObjectsParent();
            
            foreach (Transform emptyObject in emptyObjectsParent.transform)
            {
                emptyObject.GetComponent<ShapeController>().enableTrigger();
            }
            
            foreach (Transform shapeObject in shapeObjectsParent.transform)
            {
                if (shapeObject.GetComponent<ShapeController>().ID != _shapeController.ID)
                    shapeObject.gameObject.GetComponent<ShapeController>().enableTrigger();
            }
        }
    }
}