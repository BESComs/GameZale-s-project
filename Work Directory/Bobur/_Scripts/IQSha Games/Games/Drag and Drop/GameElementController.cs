using System.Threading.Tasks;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Drag_and_Drop;

public class GameElementController : MonoBehaviour
{
    
    private SpriteRenderer _curSr;
    //курсорк над de, de Находится в движении
    private bool _onImage, _inMove;
    public DraggableElement CurrentDe;
    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _curSr = GetComponent<SpriteRenderer>();
        CurrentDe.CurrentFrameId = -1;
    }

    private void OnMouseEnter()
    {
        _onImage = true;
    }

    private void OnMouseExit()
    {
        _onImage = false;
    }

    //проверка на движение
    private async Task CheckOnMovement()
    {
        if (!DragAndDrop.Instance.canDrag)
            return;
        
        //проверка можно ли в данный момент взять de
        if (Input.GetKeyDown(KeyCode.Mouse0) && _onImage && DragAndDrop.Instance.TaskCompleted)
        {
            _inMove = true;
            if(_curSr != null)
                _curSr.sortingOrder = 2;
            //если находится в frame
            if (CurrentDe.CurrentFrameId != -1)
            {
                //вытащить из frame - a текущий de и сдвинуть все элементы в frame откуда был взят de 
                await DragAndDrop.Instance.PlacementOfObjects(CurrentDe, false);
            }
        }
        //Отсановка движения
        else if (_inMove && Input.GetKeyUp(KeyCode.Mouse0))
            await StopMove();
        //движение
        else if (_inMove)
            ImageMove();
    }

    //остановка движения
    private async Task StopMove()
    {
        if(_curSr != null)
            _curSr.sortingOrder = 1;
        _inMove = false;
        //положить в frame или вернуть обратно
        await DragAndDrop.Instance.PlacementOfObjects(CurrentDe, true);
    }

    //движение за курсором
    private void ImageMove()
    {
        var tmp = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - _mainCamera.ScreenToWorldPoint(_lasMousePos);
        tmp.z = 0;
        transform.position += tmp;
    }

    private Vector3 _lasMousePos;
    
    private async void Update()
    {
        await CheckOnMovement();
        _lasMousePos = Input.mousePosition;
    }
}
