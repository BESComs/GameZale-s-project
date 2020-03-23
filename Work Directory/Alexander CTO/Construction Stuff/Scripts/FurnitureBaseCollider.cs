using UnityEngine;

public class FurnitureBaseCollider : MonoBehaviour
{
    private GameObject greenPlate;
    private GameObject redPlate;

    private Collider boxCollider;
    

    private void Awake()
    {
        greenPlate = transform.Find("green").gameObject;
        redPlate = transform.Find("red").gameObject;
        boxCollider = GetComponent<BoxCollider>();
    }

    public LayerMask fLayer;

    public bool OverlapColored()
    {
        var boxSize = Vector3.one / 3f;
        var hasCollision = Physics.CheckBox(transform.position, boxSize / 2f, Quaternion.identity, fLayer, QueryTriggerInteraction.Ignore);
        if (hasCollision) MakeRed();
        else MakeGreen();

        return hasCollision;
    }


    public void DisableCollider()
    {
        boxCollider.enabled = false;
    }

    public void EnableCollider()
    {
        boxCollider.enabled = true;
        greenPlate.SetActive(false);
        redPlate.SetActive(false);
    }
    
    public void MakeGreen()
    {
        greenPlate.SetActive(true);
        redPlate.SetActive(false);
    }

    public void MakeRed()
    {
        redPlate.SetActive(true);
        greenPlate.SetActive(false);
    }

#if UNITY_EDITOR
    
#endif
    
}