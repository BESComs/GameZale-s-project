using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
   public delegate void Container();
   public static event Container INeedRole;
   public enum PresedButton
   {
      LoginButton,
      SigninButton
   }
   public enum Name
   {
      Login,
      Logout,
      Signin
   }

   private static readonly Dictionary<Name, string> btnName = new Dictionary<Name, string>
   {
      {Name.Login, "Войти используя <b>Facebook</b>"},
      {Name.Logout, "Выйти из <b>Facebook</b>"},
      {Name.Signin, "Зарегистрироваться с <b>Facebook</b>"}
   };
   
   private static readonly Dictionary<PresedButton, List<Button>> btnList = new Dictionary<PresedButton,  List<Button>>
   {
      {PresedButton.LoginButton, new List<Button>()},
      {PresedButton.SigninButton, new List<Button>()}
   };
   static string Role;
   public static void GetRole(string role) => Role = role;
   private void Awake()
   {
      if (!FB.IsInitialized)
         FB.Init(() =>
            {
               if (FB.IsInitialized)
                  FB.ActivateApp();
               else
                  print("Couldn't initialize");
            },
            isGameShown =>
            {
               if (!isGameShown)
                  Time.timeScale = 0;
               else
                  Time.timeScale = 1;
            });
      else
         FB.ActivateApp();
      foreach (var btnType in btnList)
         btnType.Value.Clear();
   }
   public static void FillInList(GameObject obj, PresedButton pbtn)
   {
      btnList[pbtn].Add(obj.GetComponent<Button>());
      SetButtonName(
         obj.GetComponent<Button>(),
         pbtn == PresedButton.LoginButton ? 
            FB.IsLoggedIn ? Name.Logout :
            Name.Login : Name.Signin);
   }
   public static void ThisButtonPressed(PresedButton pbtn)
   {
      if (pbtn == PresedButton.LoginButton)
         FacebookAuthorization();
      else
         FacebookRegistration();
   }

   private static void FacebookAuthorization()
   {
      if (FB.IsLoggedIn)
      {
         FacebookLogout();
         SetButtonName(EventSystem.current.currentSelectedGameObject.GetComponent<Button>(), Name.Login);
      }
      else
         FacebookLogin(0);
   }
   private static void FacebookRegistration()
   {
      FacebookLogout();
      FacebookLogin(1);
   }
   private static void FacebookLogin(int i)
   {
      var btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
      var permissions = new List<string>{"public_profile"};
      FB.LogInWithReadPermissions(permissions, result =>
      {
         if (FB.IsLoggedIn)
         {
            switch (i)
            {
               case 0:
                  SetButtonName(btn, Name.Logout);
                  LoginRequest(AccessToken.CurrentAccessToken);
                  break;
               case 1:
                  SigninRequest(AccessToken.CurrentAccessToken);
                  SetButtonName(btn, Name.Signin);
                  break;
            }
         }
         else
            Toaster.ShowMessage(RequestsKeys.FailedLoginWithFacebook, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);
      });
   }

   private static async void LoginRequest(AccessToken aToken)
   {
      Toaster.ShowMessage(RequestsKeys.WaitingResponse, Toaster.Position.Bottom, Toaster.Time.HalfSecond);
      var responseJson = await ServerRequests.AuthWithFacebook(new AuthFacebookData{accessToken = aToken.TokenString});
      Toaster.ShowMessage(responseJson.message, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);
      if (responseJson.status != RequestsKeys.StatusSuccess) return;
      BackgroundToast.ActivateToast();
      await SceneManager.LoadSceneAsync("Launcher Scene 2019");
      BackgroundToast.DeactivateToast();
   }


   private static async void SigninRequest(AccessToken aToken)
   {
      BackgroundToast.ActivateToast();

      Toaster.ShowMessage(RequestsKeys.WaitingResponse, Toaster.Position.Bottom, Toaster.Time.HalfSecond);
      print("FBTOKEN\n" + aToken.TokenString);
      var fbSignUp = new FBSignUpData
      {
         accessToken = aToken.TokenString,
         userRole = "Player"
      };
      var responseJson = await ServerRequests.SignUpWithFacebook(fbSignUp);
      print("RESP_MESSAGE\n" + responseJson.message);
      print("RESP_STATUS\n" + responseJson.status);
      Toaster.ShowMessage(responseJson.message, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);

      if (responseJson.status == RequestsKeys.StatusSuccess)
         await SceneManager.LoadSceneAsync("Launcher Scene 2019");
      BackgroundToast.DeactivateToast();
   }

   private static void FacebookLogout()
   {
      FB.LogOut();
      Toaster.ShowMessage(RequestsKeys.LeftFacebook, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);
   }

   private static void SetButtonName(Button btn, Name name)
   {
      if (!btn) return;
      if (btn.transform.GetChild(0) == null) return;
      btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = btnName[name];
   }
   
   
}
