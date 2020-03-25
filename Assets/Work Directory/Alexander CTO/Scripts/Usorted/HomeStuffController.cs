using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeStuffController : MonoBehaviour
{
    public GameObject wallsParent;

    public GameObject constructionTutorial;
    public GameObject constructionGrid;
    public GameObject interactiveShit;
    
    public void ToggleWalls()
    {
        wallsParent.SetActive(! wallsParent.activeSelf);
    }

    public void ToggleConstructionMode()
    {
        var nextState = ! ConstructionDirector.Instance.gameObject.activeSelf;
        
        ConstructionDirector.Instance.gameObject.SetActive(nextState);
        constructionTutorial.SetActive(nextState);
        constructionGrid.SetActive(nextState);
        interactiveShit.SetActive(!nextState);
    }
    
}