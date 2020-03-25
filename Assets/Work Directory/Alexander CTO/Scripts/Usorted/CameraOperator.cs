using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraOperator : MonoBehaviour
{
    public static CameraOperator Instance { get; private set; }
    
    public Camera camera;
    
    public Transform target;

    public float moveSpeed;
    public float moveZoneSize;

    public Vector3 mouseDragPoint;

    public int zoomLevel;
    public int maxZoomLevel;
    
    private bool dragStarted;

    private Vector3 camLocalDestination;
    public float zoomSpeed;
    public Transform zoomStartPoint;
    public Transform zoomEndPoint;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        camLocalDestination = Vector3.Lerp(zoomStartPoint.localPosition, zoomEndPoint.localPosition,
            (float) zoomLevel / maxZoomLevel);
    }


    public Vector3 CameraMoveDirection(Vector2 mousePosition, float moveZoneSize)
    {
        Vector3 v = new Vector3();
        
        if (mousePosition.x < moveZoneSize) v += -transform.right;
        else if (mousePosition.x > Screen.width - moveZoneSize) v += transform.right;

        if (mousePosition.y < moveZoneSize) v += -transform.forward;
        else if (mousePosition.y > Screen.height - moveZoneSize) v += transform.forward;

        return v.normalized;
    }


    void Update()
    {
        if (Math.Abs(Input.mouseScrollDelta.y) > 0.01f)
        {
            var zoom = (int)Input.mouseScrollDelta.y;
            zoomLevel = zoomLevel + zoom;
            zoomLevel = Mathf.Clamp(zoomLevel, 0, maxZoomLevel);
            camLocalDestination = Vector3.Lerp(zoomStartPoint.localPosition, zoomEndPoint.localPosition,
                (float) zoomLevel / maxZoomLevel);
        }

        camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, camLocalDestination, zoomSpeed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100))
            {
                mouseDragPoint = hit.point;
            }
            else
            {
                mouseDragPoint = Vector3.zero;
            }
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            if (mouseDragPoint == Vector3.zero) return;
            var camRay = camera.ScreenPointToRay(Input.mousePosition);
            var dragRay = new Ray(mouseDragPoint, -camRay.direction);
            var plane = new Plane(Vector3.up, camera.transform.position);
            plane.Raycast(dragRay, out var distanceToPlane);
            var virtCamPosition = dragRay.GetPoint(distanceToPlane);
            var virtCamRay = new Ray(virtCamPosition, camera.transform.forward);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            groundPlane.Raycast(virtCamRay, out var virtRayDist);
            var futurePosition = virtCamRay.GetPoint(virtRayDist);
            if (Physics.OverlapSphere(futurePosition + camera.transform.position - transform.position, 0.5f).Length == 0)
            {
                transform.position = futurePosition;
            }
            else
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 100))
                {
                    mouseDragPoint = hit.point;
                }
            }
        }
        else if(Input.GetKeyUp(KeyCode.Mouse2))
        {
            camera.transform.localPosition = camLocalDestination;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            lastMouseX = Input.mousePosition.x;
            startRotation = transform.rotation;
        } else if (Input.GetKey(KeyCode.Mouse1))
        {
            var deltaX = Input.mousePosition.x - lastMouseX;
            var futurePosition = camera.transform.position - transform.position;
            
            futurePosition = transform.position + Quaternion.Euler(0, deltaX, 0) * futurePosition;
            
            if (Physics.OverlapSphere(futurePosition, 0.5f).Length == 0)
            {
                transform.rotation *= Quaternion.Euler(0, deltaX, 0);
            }

            lastMouseX = Input.mousePosition.x;
        }



        var camShift = CameraMoveDirection(Input.mousePosition, moveZoneSize) * moveSpeed * Time.deltaTime;
        transform.position += camShift;
        
    }

    public bool disss;

    private float lastMouseX;

    private Vector3 startMousePosition;
    private Quaternion startRotation;

}






