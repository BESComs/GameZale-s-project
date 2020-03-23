using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ConstructionDirector : MonoBehaviour
{
    public static ConstructionDirector Instance { get; private set; }
    
    public Camera cam;

    public Transform ground;

    private FurnitureController currentFurnitureController;

    private bool isDragging;
    private Vector3 dragShift;

    private Plane groundPlane;

    public LayerMask furnitureLayerMask;

    private void Awake()
    {
        Instance = this;
        groundPlane = new Plane(Vector3.up, ground.position);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        MakeRaycastStuff();

        if (currentFurnitureController != null)
        {
            if (Input.GetKeyDown(KeyCode.Q)) currentFurnitureController.RotateLeft();
            if (Input.GetKeyDown(KeyCode.E)) currentFurnitureController.RotateRight();
            if (Input.GetKeyDown(KeyCode.A)) currentFurnitureController.SelectPrevMesh();
            if (Input.GetKeyDown(KeyCode.D)) currentFurnitureController.SelectNextMesh();
        }
    }

    public void MakeRaycastStuff()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        ClickProvider.UseClick();
        if (!isDragging)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) &&Physics.Raycast(ray, out var hit, Mathf.Infinity, furnitureLayerMask))
            {
                var hitCube = hit.transform.GetComponentInParent<FurnitureController>();

                if (hitCube != null)
                {
                    isDragging = true;
                    currentFurnitureController = hitCube;
                    currentFurnitureController.DisableColliders();
                    currentFurnitureController.LiftMesh();

                    var groundPoint = groundPlane.RaycastPoint(ray);
                    dragShift = hitCube.transform.position - groundPoint;
                }
            }
        }
        else
        {
            const float cubeSize = 0.5f;

            var targetPosition = groundPlane.RaycastPoint(ray) + dragShift;
            targetPosition = LimitPosition(targetPosition, ground, 3);
            targetPosition = AlignPosition(targetPosition, cubeSize);

            currentFurnitureController.SetDesiredPosition(targetPosition);

            var canPlace = !currentFurnitureController.OverlapColored();

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                currentFurnitureController.EnableColliders();
                currentFurnitureController.LowerMesh();
                currentFurnitureController = null;
                isDragging = false;
            }
        }
    }

    public Vector3 AlignPosition(Vector3 position, float gridSize = 1f)
    {
        var xMin = Mathf.Floor(position.x / gridSize) * gridSize;
        var yMin = Mathf.Floor(position.y / gridSize) * gridSize;
        var zMin = Mathf.Floor(position.z / gridSize) * gridSize;

        float halfGrid = gridSize / 2f;

        return new Vector3(xMin + halfGrid, yMin + halfGrid, zMin + halfGrid);
    }

    public Vector3 LimitPosition(Vector3 position, Transform groundLimit, float boxSize)
    {
        var p = position;
        var gp = groundLimit.position;
        boxSize -= 0.1f;
        p.x = Mathf.Clamp(p.x, gp.x - boxSize, gp.x + boxSize);
        p.z = Mathf.Clamp(p.z, gp.z - boxSize, gp.z + boxSize);
        return p;
    }
}

public static class PlaneExtensions
{
    public static Vector3 RaycastPoint(this Plane p, Ray ray)
    {
        p.Raycast(ray, out var enterDistance);
        return ray.GetPoint(enterDistance);
    }
}