using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIConfirmPanel : MonoBehaviour, IConfirmableRequest
{    
    private bool? confirmResult;

    private void Awake()
    {
        ConfirmRequesters.SceneTransitionRequester = this;
        gameObject.SetActive(false);
    }
    
    public void ConfirmRequest()
    {
        confirmResult = true;
    }

    public void DeclineRequest()
    {
        confirmResult = false;
    }
    
    public async Task<bool> RequestConfirmationAsync()
    {
        confirmResult = null;
        gameObject.SetActive(true);

        while (confirmResult.HasValue == false)
            await new WaitForUpdate();
        
        gameObject.SetActive(false);
        return confirmResult.Value;
    }
}
