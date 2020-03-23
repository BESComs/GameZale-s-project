using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{

    private void OnEnable()
    {
        GetScore();
    }

    public async void GetScore()
    {
        await Task.Delay(1000);
        await PlayerProfile.UpdateProfile();
        GetComponent<TextMeshProUGUI>().text = $"{PlayerProfile.Profile.coins} монет";

    }
    
    
}
