using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    public static ProfileController Instance;
    
    public OutfitControllerNew boy;
    public OutfitControllerNew girl;

    public List<OutfitItemData> purchasedItems = new List<OutfitItemData>();
    
    public void Awake()
    {
        Instance = this;
        LoadShit();
    }

    public async void LoadShit()
    {
        const string femaleGender = "Female";
        await PlayerProfile.InitProfile();
        purchasedItems = await ServerRequests.GetPurchasedItems(PlayerProfile.Profile.playerId);
        
        var selectedChar = PlayerProfile.Profile.gender == femaleGender  ? girl : boy;
        selectedChar.gameObject.SetActive(true);
        OutfitContent.Instance.outfitController = selectedChar;
        
    }
    
}