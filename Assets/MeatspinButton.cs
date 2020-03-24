using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatspinButton : MonoBehaviour
{
    public GameObject wheel;
    
    public void OnClick()
    {
        AdvertisementController.Instance.ShowRewardedAd();
        //wheel.SetActive(true);
    }
    
}
