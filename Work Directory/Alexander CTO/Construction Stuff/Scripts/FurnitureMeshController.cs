using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureMeshController : MonoBehaviour
{
    public float liftHeight = 0.5f;
    [SerializeField]
    private float liftSpeed = 0.3f;
    
    private Transform parent;

    private bool isLifted;

    
    private void Awake()
    {
        parent = transform.parent;
    }


    // Update is called once per frame
    void Update()
    {
        var desiredPosition = parent.position;

        if (isLifted)
        {
            desiredPosition += Vector3.up * liftHeight;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, liftSpeed);
    }
    
    
    public void LiftMesh()
    {
        isLifted = true;
    }

    public void LowerMesh()
    {
        isLifted = false;
    }
    
    
}
