using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FortuneWheelManager : MonoBehaviour
{
	public static readonly HttpClient _client2 = new HttpClient();

    public Button TurnButton;
    public GameObject Circle;
    public GameObject particle_1;
    public GameObject particle_2;
    public TextMeshProUGUI text;
    public GameObject panelForText;

    private string _spinEndpoint = "https://api.edumarket.uz/roulette/spin";
    private bool _isStarted;
    private float[] _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    private ItemData lastCatchedItem = new ItemData();

    private void Start()
    {
	    _client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
		    AccessTokenCache.AccessToken);
	    _client2.DefaultRequestHeaders.Add("api-version", "1.0");
	    
/*	    if (PlayerPrefs.GetInt("firstStart") == 0)
	    {
		    PlayerPrefs.SetInt("firstStart", 1);
		    PlayerPrefs.SetInt("currentFortuneRound", 1);
		    giveRewardOn = Random.Range(3, 9);
		    PlayerPrefs.SetInt("giveRewardOn", giveRewardOn);
		    currentFortuneRound = 1;
	    }
	    else
	    {
		    currentFortuneRound = PlayerPrefs.GetInt("currentFortuneRound");
		    giveRewardOn = PlayerPrefs.GetInt("giveRewardOn");
	    }*/
	    particle_1.SetActive(false);
	    particle_2.SetActive(false);
	    text.gameObject.SetActive(false);
	    panelForText.SetActive(false);
    }

    public async void TurnWheel ()
    {
	    particle_1.SetActive(false);
	    particle_2.SetActive(false);
	    text.gameObject.SetActive(false);
	    panelForText.SetActive(false);
	    _currentLerpRotationTime = 0f;
	    _sectorsAngles = new float[] { 60, 180, 120, 300, 240, 0 };
	    int fullCircles = 20;

	    float randomFinalAngle = await GetRandomItem();

	    _finalAngle = -(fullCircles * 360 + randomFinalAngle);
	    _isStarted = true;
    }


    private void GiveAwardByAngle ()
    {
	    text.gameObject.SetActive(true);
	    panelForText.SetActive(true);
	    TurnButton.gameObject.SetActive(false);
	    switch (_startAngle)
	    {
		    case -60:
			    text.text = "Ты выиграл\n 1 монету!";
			    break;
		    case -120:
			    text.text = "Ты выиграл\n 3 монеты!";
			    break;
		    case -180:
			    text.text = "Ты выиграл\n 2 монеты!";
			    break;
		    case -240:
			    text.text = "Ты выиграл\n 5 монет!";
			    break;
		    case -300:
			    text.text = "Ты выиграл\n 4 монеты!";
			    break;
		    case 0:
			    text.text = "Ты выиграл\n предмет!";
			    break;
	    }
	    AvailabilityVar.Instance.AddTimeInSeconds(600);
	    
    }

    public void OK()
    {
	    text.gameObject.SetActive(false);
	    panelForText.SetActive(false);
	    TurnButton.gameObject.SetActive(true);
	    this.gameObject.SetActive(false);
    }

    void Update ()
    {
	    if (_isStarted) {
    	    TurnButton.interactable = false;
    	    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
    	} else {
    	    TurnButton.interactable = true;
    	    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);
    	}

    	if (!_isStarted)
    	    return;

    	float maxLerpRotationTime = 4f;
        
    	_currentLerpRotationTime += Time.deltaTime;
    	if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
    	    _currentLerpRotationTime = maxLerpRotationTime;
    	    _isStarted = false;
    	    _startAngle = _finalAngle % 360;
			particle_1.SetActive(true);
			particle_2.SetActive(true);
    	    GiveAwardByAngle ();
        }
        float t = _currentLerpRotationTime / maxLerpRotationTime;
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
    	Circle.transform.eulerAngles = new Vector3 (0, 0, angle);
    }
    
    public async Task<float> GetRandomItem()
    {
	    int i = await Spin();
	    float randomFinalAngle;
	    if (i == -1)
	    {
		    randomFinalAngle = 0;
	    }
	    else
	    {
		    randomFinalAngle = _sectorsAngles[i - 1];
	    }
	    return randomFinalAngle;
    }
    
    public async Task<int> Spin()
    {
	    string s = await AsyncPost(_spinEndpoint);
	    Item itemData = JsonUtility.FromJson<Item>(s);
	    CoinData coinData = JsonUtility.FromJson<CoinData>(s);
	    Debug.Log(coinData.data);
	    if (coinData.data == 0)
	    {
		    lastCatchedItem = new ItemData();
		    lastCatchedItem.id = itemData.data.id;
		    lastCatchedItem.name = itemData.data.name;
		    lastCatchedItem.price = itemData.data.price;
		    return -1;
	    }
	    else
	    {
		    return coinData.data;
	    }
    }
    
    public async Task<string> AsyncPost(string uri)
    {
	    string response;

	    try
	    {
		    var result = await _client2.PostAsync(uri, new StringContent(""));
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
	    return response;
    }
    
    [System.Serializable]
    public class Item
    {
	    public ItemData data;
	    public string status;
    }
    
    [System.Serializable]
    public class ItemData
    {
	    public int id;
	    public string name;
	    public int price;
    }

    [System.Serializable]
    public class CoinData
    {
	    public int data;
	    public string status;
    }
    
    
    
    /*public float GetRandomItem()
{
	float randomFinalAngle = 60;
	int p = Random.Range(0, 99);

	if (currentFortuneRound == 11)
	{
		giveRewardOn = Random.Range(3, 9);
		currentFortuneRound = 1;
		PlayerPrefs.SetInt("currentFortuneRound", 1);
		PlayerPrefs.SetInt("giveRewardOn", giveRewardOn);
	}
	
	if (currentFortuneRound == giveRewardOn)
	{
		randomFinalAngle = _sectorsAngles [5];
	}
	else
	{
		if (p <= 80)
			randomFinalAngle = _sectorsAngles [0];
		else if (p <= 86 && p > 80)
			randomFinalAngle = _sectorsAngles [2];
		else if (p <= 91 && p > 86)
			randomFinalAngle = _sectorsAngles [1];
		else if (p <= 95 && p > 91)
			randomFinalAngle = _sectorsAngles [3];
		else 
			randomFinalAngle = _sectorsAngles [4];
	}
	
	currentFortuneRound++;
	PlayerPrefs.SetInt("currentFortuneRound", currentFortuneRound);
	return randomFinalAngle;
}*/
}
