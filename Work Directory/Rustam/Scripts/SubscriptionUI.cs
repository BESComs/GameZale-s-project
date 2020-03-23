using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscriptionUI : MonoBehaviour
{
    public List<SubscriptionUIButton> buttons;

    private SubscriptionPayment subscriptionPayment;


    private void Awake()
    {
        subscriptionPayment = GetComponent<SubscriptionPayment>();
    }

    private void Start()
    {
        //SetAllPrices();
    }

    public async void SetAllPrices()
    {
        List<SubscriptionPayment.PriceData> prices = await subscriptionPayment.GetPrices("Player");
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetText(prices[i].months, prices[i].amount);
        }
    }
}
