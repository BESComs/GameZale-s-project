using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{

    public string sceneName;
    
    public void ChangeScene()
    {
        Bootstraper.Instance.LoadAnotherSceneAsync(sceneName);
    }
    
}

    