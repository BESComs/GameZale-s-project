using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SubscriptionPayment : MonoBehaviour
{
    public static readonly HttpClient _client = new HttpClient();
    public List<PriceData> prices;
    public SubscriptionUI subUI;

    private string _apiVersion = "1.0";
    private string _merchantID = "5dc11e758591256625145fee";
    private string _getProfileEndpoint = "https://api.edumarket.uz/people/profile";
    private string _getPriceEndpoint = "https://api.edumarket.uz/price?type=";
    private string _createPaymentEndpoint = "https://api.edumarket.uz/payment/create?priceId=";
    private bool resumed = false;

    public string GetEbuchiyToken()
    {
        return AccessTokenCache.AccessToken;
       // return
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcCI6IjIiLCJybCI6IlBsYXllciIsInN1YiI6IjIiLCJpYXQiOjE1NzQ2NzQ0MjIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlBsYXllciIsIm5iZiI6MTU3NDY3NDQyMiwiZXhwIjoxNTc0NzYwODIyLCJpc3MiOiJnYW1lemFsZS5lZHVtYXJrZXQiLCJhdWQiOiJnYW1lemFsZS5lZHVtYXJrZXQifQ.U1BDv2f91vQA4qIakQKNWvCKD3BKVXdRlz7O3v4lR2M";
    }

    void Awake()
    {
        subUI = GetComponent<SubscriptionUI>();
    }

    void Start()
    {
        prices = new List<PriceData>();
        GetPrices("Player");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetEbuchiyToken());
        _client.DefaultRequestHeaders.Add("api-version", _apiVersion);
    }

    private async void OnEnable()
    {
        await GetPrices("Player");
        subUI.SetAllPrices();
    }

    public async void Pay(int type)
    {
        AsyncPost(_createPaymentEndpoint + prices[type - 1].id.ToString(), prices[3].id);
        resumed = false;
        Application.OpenURL(await GenerateCheckoutLink(prices[type - 1].id-1));
        await new WaitUntil(() => resumed == true);
        await Task.Delay(10000);
        await ServerRequests.ForceRefreshToken();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        resumed = hasFocus;
    }

    public async Task<string> AsyncPost(string uri, int priceId)
    {
        string response;
        var stringContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("priceId", priceId.ToString()),
        });
       
        try
        {
            var result = await _client.PostAsync(uri, stringContent);
            response = await result.Content.ReadAsStringAsync();
        }
        catch
        {
            var toJson = new JsendWrap<nullResponse>
            {
                status = RequestsKeys.StatusFail,
                message = RequestsKeys.ServerIsNotAvailable
            };
            response = JsonUtility.ToJson(toJson);
        }
        Debug.Log(response);
        return response;
    }

    public async Task<string> AsyncGet(string uri)
    {
        string response;
        try
        {
            var result = await _client.GetAsync(uri);
            response = await result.Content.ReadAsStringAsync();
        }
        catch
        {
            var toJson = new NoInternetError();
            response = JsonUtility.ToJson(toJson);
        }
        return response;
    }

    public async Task<Profile> GetProfile()
    {
        string s = await AsyncGet(_getProfileEndpoint);
        Profile profileData = JsonUtility.FromJson<Profile>(s);
        return profileData;
    }
    
    public async Task<List<PriceData>> GetPrices(string type)
    {
        string s = await AsyncGet(_getPriceEndpoint + type);
        Price priceData = JsonUtility.FromJson<Price>(s);
        foreach (var p in priceData.data)
        {
            //Debug.Log(p.amount);
            prices.Add(p);
        }
        
        return prices;
    }

    private async Task<string> GenerateCheckoutLink(int priceId)
    {
        string link = "https://checkout.paycom.uz/";
        Profile profile = await GetProfile();
        string a = prices[priceId].amount.ToString() + "00";
        string people_id = profile.data.peopleId.ToString();
        string toEncode = "m=" + _merchantID + ";" + "ac.user_id=" + people_id + ";" + "a=" + a;
        byte[] bytesToEncode = Encoding.UTF8.GetBytes (toEncode);
        string encodedText = Convert.ToBase64String (bytesToEncode);
        return link + encodedText;
    }

    [System.Serializable]
    public class Price
    {
        public List<PriceData> data;
        public string status;
        public string message;
    }
    
    [System.Serializable]
    public class Profile
    {
        public ProfileData data;
        public string status;
    }
    
    [System.Serializable]
    public class ProfileData
    {
        public int adminId;
        public int peopleId;
        public int userId;
        public string firstName;
        public string lastName;
        public string middleName;
        public string gender;
        public string phoneMobile;
        public bool phoneMobileConfirmed;
        public bool emailConfirmed;
        public string photoPath;
        public string login;
        public string role;
    }
    
    [System.Serializable]
    public class PriceData
    {
        public int id;
        public string name;
        public int amount;
        public int months;
        public string type;
    }
}


    