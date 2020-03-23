using System;
using UnityEngine;

public partial class ShapeController : MonoBehaviour
{
    [SerializeField]
    private int _ID;
    [NonSerialized]
    public int role;
    public GameObject CollidedGameObject { get; private set; }
    
    public int ID
    {
        get { return _ID; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (role == Roles.ACTIVE && other is BoxCollider2D)
        {
            CollidedGameObject = other.gameObject;
        }
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (role == Roles.ACTIVE)
        {
            CollidedGameObject = null;
        }
    }

    public void disableTrigger()
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
    }
    
    public void enableTrigger()
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = true;
        }
    }

    public void SetSortingLayer(int layer)
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1);
    }
    
}

public static class Roles
{
    public const int ACTIVE = 1;
    public const int PASSIVE = 0;
}