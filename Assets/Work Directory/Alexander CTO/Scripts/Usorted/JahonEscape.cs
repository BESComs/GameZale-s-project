using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JahonEscape : MonoBehaviour
{
    public GameObject l;
    public GameObject b;

    
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            b.SetActive(false);
            l.SetActive(true);
        }        
    }
}
