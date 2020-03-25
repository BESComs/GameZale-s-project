using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundToast : MonoBehaviour
{
    private static bool active;

    private void Update()
    {
        gameObject.GetComponent<Image>().enabled = active;
    }

    public static void ActivateToast() => active = true;
    public static void DeactivateToast() => active = false;
    


}
