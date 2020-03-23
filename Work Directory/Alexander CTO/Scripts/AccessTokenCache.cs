using System;
using System.Globalization;
using UnityEngine;



public static class AccessTokenCache
{
    public static string Scheme => PlayerPrefs.GetString(PrefsKey.Scheme);
    public static string AccessToken => PlayerPrefs.GetString(PrefsKey.AccessToken);
    public static string RefreshToken => PlayerPrefs.GetString(PrefsKey.RefreshToken);
    
    /// <summary>
    /// Checks if access token expired
    /// </summary>
    public static bool IsAccessTokenExpired()
    {
        var tokenExpirationDateString = PlayerPrefs.GetString(PrefsKey.AccessTokenExpirationDateUtc);
        var tokenExpirationDate = DateTime.Parse(tokenExpirationDateString, CultureInfo.InvariantCulture);
        
        return tokenExpirationDate < DateTime.UtcNow;
    }

    /// <summary>
    /// Check if access token exist
    /// </summary>
    public static bool HasAccessToken()
    {
        return PlayerPrefs.HasKey(PrefsKey.AccessToken);
    }

    /// <summary>
    /// Clears access token data
    /// </summary>
    public static void ClearCache()
    {
        PlayerPrefs.DeleteKey(PrefsKey.Scheme);
        PlayerPrefs.DeleteKey(PrefsKey.AccessToken);
        PlayerPrefs.DeleteKey(PrefsKey.RefreshToken);
        PlayerPrefs.DeleteKey(PrefsKey.AccessTokenExpirationDateUtc);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Set new authorization data
    /// </summary>
    /// <param name="authorizationData">AuthorizationData to set</param>
    public static void SetAuthorizationData(AuthorizationData authorizationData)
    {
        PlayerPrefs.SetString(PrefsKey.Scheme, authorizationData.type);
        PlayerPrefs.SetString(PrefsKey.AccessToken, authorizationData.accessToken);
        PlayerPrefs.SetString(PrefsKey.RefreshToken, authorizationData.refreshToken);
        PlayerPrefs.SetString(PrefsKey.AccessTokenExpirationDateUtc, authorizationData.expiresAt.ToString(CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }
    
    private class PrefsKey
    {
        public const string Scheme = "Scheme";
        public const string AccessToken = "AccessToken";
        public const string RefreshToken = "RefreshToken";
        public const string AccessTokenExpirationDateUtc = "AccessTokenExpirationDate";
    }
    
}


public class AuthorizationData
{
    public string type;
    public string accessToken;
    public string refreshToken;
    public DateTime expiresAt;
}