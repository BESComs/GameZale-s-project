using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Toaster : MonoBehaviour
{
    private static GameObject clone;
    public enum Time
    {
        ThreeSecond,
        TwoSecond,
        OneWithHalfSecond,
        OneSecond,
        HalfSecond
    };
    public enum Position
    {
        Top,
        Bottom
    };
    public static void ShowMessage ( string message, Toaster.Position position, Toaster.Time time )
    {
        var messagePrefab = Resources.Load ( "Message" ) as GameObject;
        var containerObject = messagePrefab.transform.GetChild ( 0 );
        var backObject = containerObject.GetChild(0);
        var textObject = backObject.GetChild ( 0 ).gameObject;
        var messageText = textObject.GetComponent<Text> ( );
        messageText.text = message;
        SetPosition ( containerObject.GetComponent<RectTransform> ( ), position );
        if(clone) Destroy(clone);
        clone = Instantiate ( messagePrefab );
        RemoveClone ( clone, time );

    }

    private static void SetPosition ( RectTransform rectTransform, Position position )
    {
        if (position == Position.Top)
        {
            rectTransform.anchorMin = new Vector2 ( 0.5f, 1f );
            rectTransform.anchorMax = new Vector2 ( 0.5f, 1f );
            rectTransform.anchoredPosition = new Vector3 ( 0.5f, -100f, 0 );
        }
        else
        {
            rectTransform.anchorMin = new Vector2 ( 0.5f, 0 );
            rectTransform.anchorMax = new Vector2 ( 0.5f, 0 );
            rectTransform.anchoredPosition = new Vector3 ( 0.5f, 100f, 0 );
        }
    }

    private static void RemoveClone ( GameObject clone, Time time )
    {
        switch (time)
        {
            case Time.HalfSecond:
                Destroy(clone,0.5f);
                break;
            case Time.OneSecond:
                Destroy ( clone.gameObject, 1f );
                break;
            case Time.OneWithHalfSecond:
                Destroy ( clone.gameObject, 1.5f );
                break;
            case Time.TwoSecond:
                Destroy ( clone.gameObject, 2f );
                break;
            default:
                Destroy ( clone.gameObject, 3f );
                break;
        }
    }
    
    
}
