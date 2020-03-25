using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChildCollider : MonoBehaviour
{
    private DoorController doorController;

    private void Awake()
    {
        doorController = GetComponentInParent<DoorController>();
    }

    private void OnMouseDown()
    {
        doorController.ToggleDoor();
    }
    
}