using UnityEngine;
using UnityEngine.SceneManagement;
//using Facebook.Unity;

public class LogoutButton : MonoBehaviour
{
    public void Logout()
    {
        AccessTokenCache.ClearCache();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        //FB.LogOut();
        PlayerProfile.ClearProfile();
        SceneManager.LoadScene("AuthorizationScene");
    }
}