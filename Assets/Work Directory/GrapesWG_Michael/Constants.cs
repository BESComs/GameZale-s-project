using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class ApiUrl
{
    public const string BaseUrl = "https://api.edumarket.uz/";
    public const string Signup = BaseUrl + "auth/signup/";
    public const string SignupFacebook = BaseUrl + "auth/signup/facebook/";
    public const string Signin = BaseUrl + "auth/signin/";
    public const string SigninFacebook = BaseUrl + "auth/signin/facebook/"; 
    public const string Refresh = BaseUrl + "auth/refresh/";
    //public const string available = baseUrl + "edu/available/";
    //public const string exercise = baseUrl + "exercise/";
    public const string Statistics = BaseUrl + "statistic";
    public const string StatisticsMany = BaseUrl + "statistics/many";
    public const string PlayerStatistic = BaseUrl + "statistic/player/";

    public const string PlayerProfile = BaseUrl + "people/profile";
    public const string PlayerPurchasedItems = BaseUrl + "item/player/";
    public const string PurchaseOutfitItem = BaseUrl + "item/buy/";
}

class RequestsKeys
{
    public const string MediaType = "application/json";
    public const string StatusFail = "fail";
    public const string StatusSuccess = "Success";
    public const string StatusError = "Error";
    //public const string Edu = "X-Edu";
    public const string Api = "api-version";
    public const string ApiVer = "1.0";
    public const string HasNotToken = "HasNotToken";
    public const string TokenNotExpired = "TokenNotExpired";
    public const string HasNewToken = "HasNewToken";
    public const string ServerIsNotAvailable = "Не могу подключиться, проверьте интернет соединение";
    public const string FailedLoginWithFacebook = "Не удалось войти используя Фейсбук";
    public const string LeftFacebook = "Программа покинула Фейсбук";
    public const string WaitingResponse = "Жду окончания проверки";
}
