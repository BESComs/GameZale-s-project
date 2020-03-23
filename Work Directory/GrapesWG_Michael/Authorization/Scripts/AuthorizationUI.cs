using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Work_Directory.Denis.Scenes.NewLauncher.Scripts;
using Button = UnityEngine.UI.Button;

public class AuthorizationUI : MonoBehaviour
{
    #region Serialized fields

    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public Button loginButton;

    #endregion


    private async void Start()
    {
        BackgroundToast.ActivateToast();
        RestClient.SetHeader(new KeyValuePair<string, string>(RequestsKeys.Api, RequestsKeys.ApiVer));
        await CheckToken();
        BackgroundToast.DeactivateToast();
    }

    public async void CreateAuthRequest()
    {
        BackgroundToast.ActivateToast();
        loginButton.interactable = false;
        var Auth = new AuthData
        {
            login = loginInput.text,
            password = passwordInput.text
        };
        passwordInput.text = "";
        loginInput.interactable = false;
        passwordInput.interactable = false;
        Toaster.ShowMessage(RequestsKeys.WaitingResponse, Toaster.Position.Bottom, Toaster.Time.HalfSecond);
       
        var responseJson = await ServerRequests.AsyncAuth(Auth);
        Toaster.ShowMessage(responseJson.message, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);

        if (responseJson.status == RequestsKeys.StatusSuccess) 
            await SceneManager.LoadSceneAsync("Launcher Scene 2019");
        else {
            loginInput.interactable = true;
            passwordInput.interactable = true;
            passwordInput.text = "";
        }
        BackgroundToast.DeactivateToast();
    }

    public static async Task CheckToken()
    {
        if (!AccessTokenCache.HasAccessToken()) return;
        var result = await ServerRequests.ConfirmationRefresh();
        if (result.isSuccess)
        {
            RestClient.SetAuthorizationToken(AccessTokenCache.Scheme, AccessTokenCache.AccessToken);
            await SceneManager.LoadSceneAsync("Launcher Scene 2019");
            return;
        }

        Toaster.ShowMessage(result.response, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);
    }


    private void Update()
    {
        loginButton.interactable = passwordInput.text.Length > 0 && loginInput.text.Length > 0;
    }
}

public static class PlayerProfile
{
    public static PlayerData Profile;

    public static async Task InitProfile()
    {
        if (Profile != null) return;
        await UpdateProfile();
    }
    
    public static async Task UpdateProfile()
    {
        var response = await ServerRequests.GetPlayerProfileAsync();
        PlayerProfile.Profile = response;
    }
    

    public static void ClearProfile()
    {
        Profile = null;
    }

}