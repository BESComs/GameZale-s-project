using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRequester : SerializedMonoBehaviour, ITriggerTarget
{
    
    public IConfirmableRequest confirmableRequest;

    [ReadOnly]
    public string sceneName;

    private void Start()
    {
        confirmableRequest = ConfirmRequesters.SceneTransitionRequester;
    }

    
    [Button]
    public async void RequestScene()
    {
        var confirmed = await confirmableRequest.RequestConfirmationAsync();
        if (! confirmed) return;
        await Bootstraper.Instance.LoadAnotherSceneAsync(sceneName);
    }

    public void OnTriggerCall()
    {
        RequestScene();
    }
    
    
#if UNITY_EDITOR
    
    [OnValueChanged("GrabSceneName"), AssetList(Path = "Scenes")]
    public SceneAsset sceneAsset;

    
    private void GrabSceneName()
    {
        sceneName = sceneAsset.name;
    }
    
#endif
    
}