using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadProgress : MonoBehaviour
{
    public static SceneLoadProgress Instance { get; private set; }

    public TextMeshProUGUI loadProgressText;
    
    private void Awake()
    {
        Instance = this;
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }


    public async Task StartLoadingProgress(AsyncOperation loadingProgress)
    {
        gameObject.SetActive(true);
        while (!loadingProgress.isDone)
        {
            var percentDone = (int) (loadingProgress.progress * 100);
            loadProgressText.text = $"{percentDone}%";
            await new WaitForUpdate();
        }
        gameObject.SetActive(false);
    }
    
}