using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCollider : MonoBehaviour
{
    public Transform interactivePopup;
    public float popupSpeed = 3f;

    private float currentPopupSize = 0f;
    private bool inCollision;

    private void OnTriggerEnter(Collider other)
    {
        inCollision = true;// other.CompareTag(GameTags.Player);
    }

    private void OnTriggerExit(Collider other)
    {
        inCollision = ! other.CompareTag(GameTags.Player);
    }


    private void Update()
    {
        var cam = CameraOperator.Instance?.camera?.transform;
        if (cam == null) return;
        
        if (inCollision)
            currentPopupSize += popupSpeed * Time.deltaTime;
        else
            currentPopupSize -= popupSpeed * Time.deltaTime;
            
        currentPopupSize = Mathf.Clamp(currentPopupSize, 0.01f, 1f);

        interactivePopup.localScale = Vector3.one * currentPopupSize;

        interactivePopup.LookAt(cam);
    }
}
