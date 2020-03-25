using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstraper : SerializedMonoBehaviour
{
    private const string PrevSceneKey = "PrevSceneKey";
    
    public static Bootstraper Instance;

    public string initialSceneName;
    public string currentLoadedSceneName;
    public string PrevSceneName => PlayerPrefs.GetString(PrevSceneKey);
    
    [OdinSerialize]
    public IScreenFader screenFader;  
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLoadedSceneName = initialSceneName;
        PlayerPrefs.SetString(PrevSceneKey, currentLoadedSceneName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(initialSceneName, LoadSceneMode.Additive);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    
    public async Task LoadAnotherSceneAsync(string sceneName)
    {
        
        await screenFader.FadeInAsync();
        await Task.Delay(1000);
        PlayerPrefs.SetString(PrevSceneKey, currentLoadedSceneName);
        PlayerPrefs.Save();
        await SceneManager.UnloadSceneAsync(currentLoadedSceneName);

        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        await screenFader.FadeOutAsync();

        currentLoadedSceneName = sceneName;
    }
}