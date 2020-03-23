using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class AvailabilityVar : MonoBehaviour
{
    private const float _TIME = 1800;
    
    public static AvailabilityVar Instance;
    public bool canPlay = true;
    public TextMeshProUGUI text;
    
    private int currentDay = 0;
    private int lastDayPlayed = 0;
    private float timeLeft = _TIME;
    private bool enabled = true;

    private bool gotPodpiska = false;
    
    void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;
    }
    
    void Start()
    {
        if(PlayerPrefs.GetInt("first") == 0)
        {
            Debug.Log("first");
            timeLeft = _TIME;
            PlayerPrefs.SetInt("timeLeft", (int)_TIME);
            PlayerPrefs.SetInt("first", 1);
        }
        else
        {
            timeLeft =  PlayerPrefs.GetInt("timeLeft");
        }

        lastDayPlayed = PlayerPrefs.GetInt("lastDayPlayed");
        currentDay = DateTime.Now.Day;
        if (currentDay != lastDayPlayed) 
        {
            timeLeft = _TIME;
            PlayerPrefs.SetInt("timeLeft", (int)_TIME);
            PlayerPrefs.SetInt("lastDayPlayed", currentDay);
            lastDayPlayed = currentDay;
            canPlay = true;
            text.text = "30:00";
        }

        var token = AccessTokenCache.AccessToken;
        var start = token.IndexOf(".", StringComparison.Ordinal) + 1;
        var end = token.LastIndexOf(".", StringComparison.Ordinal);
        var payload = token.Substring(start, token.Length - start - (token.Length - end));
        
        var basePadding = payload.Length % 4;
        var paddingString = new string('=', basePadding);
        var data = Convert.FromBase64String(payload + paddingString);
        var dec = Encoding.UTF8.GetString(data);

        if (dec.Contains("pp_acc"))
        {
            gotPodpiska = true;
            text.text = "Подписка куплена";
        }

    }

    private void OnEnable()
    {
        enabled = true;
    }

    private void OnDisable()
    {
        enabled = false;
    }

    void Update()
    {
        if (gotPodpiska) return;
        
        if (canPlay && enabled)
        {
            timeLeft -= Time.deltaTime;
            int mins = ((int) timeLeft) / 60;
            int secs = ((int) timeLeft) % 60;

            if (secs == 0)
            {
                PlayerPrefs.SetInt("timeLeft", ((int) timeLeft));
            }
            
            text.text = mins.ToString() + ":" + ((secs < 10) ? "0" : "") + secs.ToString();

            if (timeLeft <= 0)
            {
                canPlay = false;
            }
        }
        else if(!canPlay)
        {
            text.text = "Время кончилось"; //"00:00";
        }
    }

    public void AddTimeInSeconds(int sec)
    {
        timeLeft += sec;
        canPlay = true;
        PlayerPrefs.SetInt("timeLeft", ((int) timeLeft));
    }
}
