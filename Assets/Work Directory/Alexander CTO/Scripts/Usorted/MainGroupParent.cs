using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainGroupParent : MonoBehaviour
{
    public List<MainGroupButton> buttons; 
    
    public void UnfocusButtons()
    {
        foreach (var button in buttons)
        {
            button.OnFocusLost();
        }
    }

    [Button]
    public void GrabButtons()
    {
        var buttonsParent = transform.GetChild(0);
        var btns = buttonsParent.GetComponentsInChildren<MainGroupButton>();
        buttons = new List<MainGroupButton>(btns);
        foreach (var button in buttons)
        {
            button.SetGroupParent(this);
            button.GrabImageSprite();
        }
    }
    
    
}
