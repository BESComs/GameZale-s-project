using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

class RestClient
{
   private static readonly HttpClient Client;

    static RestClient()
    {
        Client = new HttpClient();
        
        Client.DefaultRequestHeaders.Add(RequestsKeys.Api, RequestsKeys.ApiVer);
    }
    
    public static void SetHeader(KeyValuePair<string, string> keyValuePair)
    {
        if (Client.DefaultRequestHeaders.Contains(keyValuePair.Key))
            Client.DefaultRequestHeaders.Remove(keyValuePair.Key);
        Client.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
    }
    public static void SetHeaders([NotNull] KeyValuePair<string, string>[] keyValuePairs)
    {
        if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));
        foreach (var pair in keyValuePairs)
            Client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
    }
    public static async Task<string> PostAsync(string uri, StringContent content)
    {
        string response;
        Debug.Log(await content.ReadAsStringAsync());
        try
        {
            var result = await Client.PostAsync(uri, content);
            response = await result.Content.ReadAsStringAsync();
            Debug.Log("PostSuccess\n" + result.StatusCode + "\n" + response);
        }
        catch
        {
            Debug.Log("PostError");
            var toJson = new JsendWrap<nullResponse>
            {
                status = RequestsKeys.StatusFail,
                message = RequestsKeys.ServerIsNotAvailable
            };
            response = JsonUtility.ToJson(toJson);
        }

        return response;
    }

    public static async Task<string> GetAsync(string uri)
    {
        string response;
        try
        {
            var result = await Client.GetAsync(uri);
            response = await result.Content.ReadAsStringAsync();
            Debug.Log("GetSuccess\n" +result.StatusCode + "\n" + result.Headers);
        }
        catch (Exception e)
        {
            Debug.Log("GetFailed\n" + e.Message);
            var toJson = new NoInternetError();
            response = JsonUtility.ToJson(toJson);
        }

        Debug.Log(uri + "\n" +response);
        return response;
    }
    
    public static async Task<HttpResponseMessage> GetPhotoAsync(string url)
    {
        return await Client.GetAsync(url);
    }


    public static void SetAuthorizationToken(string scheme, string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
    }

}
