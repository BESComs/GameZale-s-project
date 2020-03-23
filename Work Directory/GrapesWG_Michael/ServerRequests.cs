using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
// ReSharper disable StringLiteralTypo

public class ServerRequests
{
    private static async Task<string> RefreshToken()
    {
        if (!AccessTokenCache.HasAccessToken()) return RequestsKeys.HasNotToken;
        if (!AccessTokenCache.IsAccessTokenExpired()) return RequestsKeys.TokenNotExpired;

        //запрос на сервер
        var content = JsonTools.CreateContent(new RefreshData {refreshToken = AccessTokenCache.RefreshToken});
        var response = await RestClient.PostAsync(ApiUrl.Refresh, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);

        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON.message;
        //если запрос прошел установка токена
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);
        JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data);

        AccessTokenCache.SetAuthorizationData(
            JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return RequestsKeys.HasNewToken;
    }
    
    public static async Task<string> ForceRefreshToken()
    {
        //запрос на сервер
        var content = JsonTools.CreateContent(new RefreshData {refreshToken = AccessTokenCache.RefreshToken});
        var response = await RestClient.PostAsync(ApiUrl.Refresh, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);

        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON.message;
        //если запрос прошел установка токена
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);
        JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data);

        AccessTokenCache.SetAuthorizationData(
            JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return RequestsKeys.HasNewToken;
    }

    public static async Task<RefreshResponseBoolPair> ConfirmationRefresh()
    {
        var refreshResponse = await RefreshToken();
        var response = new RefreshResponseBoolPair {response = refreshResponse};
        response.isSuccess = refreshResponse == RequestsKeys.TokenNotExpired
                             || refreshResponse == RequestsKeys.HasNewToken;
        return response;
    }

    public static async Task<JsendWrap<AuthResponseData>> AsyncAuth(AuthData json)
    {
        Debug.Log(json.login);
        Debug.Log(json.password);
        var content = JsonTools.CreateContent(json);
        var response = await RestClient.PostAsync(ApiUrl.Signin, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);

        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON;
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);

        AccessTokenCache.SetAuthorizationData(
            JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return jsonResponse.Result.JSON;
    }
    public static async Task<JsendWrap<AuthResponseData>> AsyncSignUp(SignUpData json)
    {
        var content = JsonTools.CreateContent(json);
        var response = await RestClient.PostAsync(ApiUrl.Signup, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);
        
        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON;
       
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);

        AccessTokenCache.SetAuthorizationData(
            JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return jsonResponse.Result.JSON;
    }
    
    public static async  Task<JsendWrap<AuthResponseData>> AuthWithFacebook( AuthFacebookData json)
    {
        var content = JsonTools.CreateContent(json);
        var response = await RestClient.PostAsync(ApiUrl.SigninFacebook, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);
        
        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON;
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);

        AccessTokenCache.SetAuthorizationData(
           JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return jsonResponse.Result.JSON;
    }
    
    public static async  Task<JsendWrap<AuthResponseData>> SignUpWithFacebook( FBSignUpData json)
    {
        var content = JsonTools.CreateContent(json);
        var response = await RestClient.PostAsync(ApiUrl.SignupFacebook, content);
        var jsonResponse = JsonTools.ResponseToTModel<AuthResponseData>(response);
        
        if (!jsonResponse.Result.isSuccess) return jsonResponse.Result.JSON;
        RestClient.SetAuthorizationToken(jsonResponse.Result.JSON.data.type, jsonResponse.Result.JSON.data.accessToken);

        AccessTokenCache.SetAuthorizationData(
            JsonTools.ConvertAuthResponseToAuthorizationData(jsonResponse.Result.JSON.data));
        return jsonResponse.Result.JSON;
    }
    
    

    public static async Task<string> GetStudentNameAsync()
    {
        var response = await RestClient.GetAsync(ApiUrl.PlayerProfile);
        var jsonResponse = JsonUtility.FromJson<JsendWrap<PlayerData>>(response);
        return jsonResponse.data.firstName + " " + jsonResponse.data.lastName;
    }
    
    public static async Task<string> GetStudentPhotoPathAsync()
    {
        var response = await RestClient.GetAsync(ApiUrl.PlayerProfile);
        var jsonResponse = JsonUtility.FromJson<JsendWrap<PlayerData>>(response);
        return jsonResponse.data.photoPath;
    }

    
    public static async Task<PlayerData> GetPlayerProfileAsync()
    {
        var response = await RestClient.GetAsync(ApiUrl.PlayerProfile);
        var jsonResponse = JsonUtility.FromJson<JsendWrap<PlayerData>>(response);
        return jsonResponse.data;
    }

    public static async Task<PlayerStatisticData> GetPlayerStatisticAsync()
    {
        await PlayerProfile.InitProfile();
        var response = await RestClient.GetAsync(ApiUrl.PlayerStatistic + PlayerProfile.Profile.playerId);
        var jsonResponse = JsonUtility.FromJson<JsendWrap<PlayerStatisticData>>(response);
        return jsonResponse.data;
    }
    
    
    
    public static async Task<List<OutfitItemData>> GetPurchasedItems(int playerId)
    {
        var response = await RestClient.GetAsync(ApiUrl.PlayerPurchasedItems + playerId);
        var jsonResponse = JsonUtility.FromJson<JsendWrap<List<OutfitItemData>>>(response);
        return jsonResponse.data;
    }
    
    

    public static async Task<bool> PurchaseItem(int itemId)
    {
        Debug.Log("Purchasing item id: " + itemId);
        var response = await RestClient.PostAsync(ApiUrl.PurchaseOutfitItem + itemId, new StringContent(""));
        var jsonResponse = JsonUtility.FromJson<JsendWrap<OutfitItemData>>(response);
        return jsonResponse.status == "Success";

    }

    public static async void PostStatistics()
    {
        if (!(await ConfirmationRefresh()).isSuccess) return;
        var statistic = new StatisticData
        {
            exerciseId = LessonsDatabase.Instance.GetCurrentLessonId(),
            createdDate = LessonStatistic.GetStartLessonTimeStr(),
            spentTime = (int) LessonStatistic.GetLessonUnityDuration(),
            score = LessonStatistic.GetScore(),
            isRight = LessonStatistic.GetRight()
        };

        await PlayerProfile.InitProfile();

        statistic.playerId = PlayerProfile.Profile.playerId;
        
        var content = JsonTools.CreateContent(statistic);
        var response = await RestClient.PostAsync(ApiUrl.Statistics, content);
        var responseData = JsonUtility.FromJson<JsendWrap<nullResponse>>(response);
        
        Debug.Log(response);
        Debug.Log(responseData.data + " " + responseData.message);
        if (responseData.message == RequestsKeys.ServerIsNotAvailable)
        {
            StatisticsCache.AddToCache(statistic);
        }

    }

    public static async Task PostCachedStatistics()
    {
        if (!(await ConfirmationRefresh()).isSuccess) return;

        var statistics = StatisticsCache.GetCachedStatistics();
        var statisticsList = new StatisticsList(statistics);

        var content = JsonTools.CreateFromString(statisticsList.GetJsonString());

        var response = await RestClient.PostAsync(ApiUrl.StatisticsMany, content);
        var responseData = JsonUtility.FromJson<JsendWrap<nullResponse>>(response);

        Debug.Log(responseData.data + " " + responseData.message);
        if (responseData.message != RequestsKeys.ServerIsNotAvailable)
        {
            StatisticsCache.ClearCache();
        }

    }
    
    public static async Task<Texture2D> GetAthletePhoto(string photoUrl)
    {
        var response = await RestClient.GetPhotoAsync(photoUrl);
        var imageDate = await response.Content.ReadAsByteArrayAsync();
        var t = new Texture2D(1,1);
        t.LoadImage(imageDate);
        return t;   
    }

    

    private class JsonTools
    {
        public static AuthorizationData ConvertAuthResponseToAuthorizationData(AuthResponseData authResponse)
        {
            return new AuthorizationData
            {
                type = authResponse.type,
                accessToken = authResponse.accessToken,
                expiresAt = DateTime.Parse(authResponse.expiresAt, CultureInfo.InvariantCulture),
                refreshToken = authResponse.refreshToken
            };
        }

        public static async Task<ResponseBoolPair<TModel>> ResponseToTModel<TModel>(string response)
        {
            var pair = new ResponseBoolPair<TModel>();
            var jsonResponse = JsonUtility.FromJson<JsendWrap<TModel>>(response);
            pair.JSON = jsonResponse;
            pair.isSuccess = jsonResponse.status == RequestsKeys.StatusSuccess;
            return pair;
        }

        public static async Task<ResponseBoolPair<TModel>> ResponseToTModelArray<TModel>(string response)
        {
            var pair = new ResponseBoolPair<TModel>();
            var jsonResponse = JsonUtility.FromJson<JsendWrap<TModel>>(response);

            if (jsonResponse.status != RequestsKeys.StatusSuccess)
            {
                pair.isSuccess = false;
                return pair;
            }

            pair.isSuccess = true;
            if (response.IndexOf("[]") != -1)
                return pair;

            response = response.Substring(response.IndexOf("[{") + 1);
            response = response.Substring(0, response.Length - 2);
            jsonResponse.data = JsonUtility.FromJson<TModel>(response);

            pair.JSON = jsonResponse;
            return pair;
        }

        public static StringContent CreateContent(ModelData model)
        {
            var json = JsonUtility.ToJson(model);
            return new StringContent(json, System.Text.Encoding.UTF8, RequestsKeys.MediaType);
        }

        public static StringContent CreateFromString(string jsonString)
        {
            return new StringContent(jsonString, System.Text.Encoding.UTF8, RequestsKeys.MediaType);
        }
    }

}


class ResponseBoolPair<T>
{
    public bool isSuccess;
    public JsendWrap<T> JSON;
}


public class RefreshResponseBoolPair
{
    public bool isSuccess;
    public string response;
}