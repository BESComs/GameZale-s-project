using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGroupButton : MonoBehaviour, ISelectableButton
{

    public GameObject subGroupObject;

    public Image imageSprite;
    public MainGroupParent groupParent;

    public void GrabImageSprite()
    {
        imageSprite = GetComponent<Image>();
    }

    public void SetGroupParent(MainGroupParent groupParent)
    {
        this.groupParent = groupParent;
    }

    public void OnClick()
    {
        groupParent.UnfocusButtons();
        OnFocusGain();
    }

    public void OnFocusGain()
    {
        imageSprite.color = Color.green;
        subGroupObject.SetActive(true);
    }

    public void OnFocusLost()
    {
        imageSprite.color = Color.white;
        subGroupObject.SetActive(false);
    }

}