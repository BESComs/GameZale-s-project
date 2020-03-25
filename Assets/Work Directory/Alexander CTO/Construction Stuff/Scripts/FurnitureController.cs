using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FurnitureController : MonoBehaviour
{
    private Quaternion desiredAngle = Quaternion.identity;
    private Vector3 desiredPosition;

    private FurnitureMeshController furnitureMeshController;
    private FurnitureBaseCollider[] baseColliders;
    public int currentMeshIndex;
    
    private Transform t;
    private Transform meshParent;

    public float rotationSpeed = 720f;


    private void Awake()
    {
        t = transform;
        meshParent = transform.Find("Mesh Parent");
        baseColliders = transform.Find("Base Colliders").GetComponentsInChildren<FurnitureBaseCollider>();
        desiredPosition = t.position;
        furnitureMeshController = GetComponentInChildren<FurnitureMeshController>();
    }

    private void Update()
    {
        t.rotation = Quaternion.RotateTowards(t.rotation, desiredAngle, rotationSpeed * Time.deltaTime);
        t.position = Vector3.Lerp(t.position, desiredPosition, 0.5f);
    }

    public void RotateLeft()
    {
        desiredAngle = desiredAngle * Quaternion.Euler(0f, -90f, 0f);
    }

    public void RotateRight()
    {
        desiredAngle = desiredAngle * Quaternion.Euler(0f, 90f, 0f);
    }

    public void SelectPrevMesh()
    {
        meshParent.GetChild(currentMeshIndex).gameObject.SetActive(false);
        currentMeshIndex--;
        if (currentMeshIndex < 0) currentMeshIndex = meshParent.childCount - 1;
        meshParent.GetChild(currentMeshIndex).gameObject.SetActive(true);
    }

    public void SelectNextMesh()
    {
        meshParent.GetChild(currentMeshIndex).gameObject.SetActive(false);
        currentMeshIndex = (currentMeshIndex + 1) % meshParent.childCount;
        meshParent.GetChild(currentMeshIndex).gameObject.SetActive(true);
    }

    public void SelectMesh(int meshNumber)
    {
        foreach (Transform mesh in meshParent)
            mesh.gameObject.SetActive(false);
        currentMeshIndex = meshNumber;
        meshParent.GetChild(currentMeshIndex).gameObject.SetActive(true);
    }
    
    
    public void SetDesiredPosition(Vector3 p)
    {
        desiredPosition = p;
    }

    public void DisableColliders()
    {
        foreach (var boxCollider in baseColliders)
            boxCollider.DisableCollider();

    }

    public void EnableColliders()
    {
        foreach (var boxCollider in baseColliders)
            boxCollider.EnableCollider();
    }


    public void LiftMesh()
    {
        furnitureMeshController.LiftMesh();
    }

    public void LowerMesh()
    {
        furnitureMeshController.LowerMesh();
    }


    public bool OverlapColored()
    {
        bool hasCollisions = false;
        foreach (var baseCollider in baseColliders)
        {
            hasCollisions |= baseCollider.OverlapColored();
        }

        return hasCollisions;
    }

    
}