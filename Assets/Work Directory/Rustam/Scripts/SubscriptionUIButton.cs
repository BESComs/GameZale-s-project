using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionUIButton : MonoBehaviour
{
    public Text text;

    public void SetText(int month, int price)
    {
        string m = "месяц";
        if (month > 1 && month < 5)
            m = "месяца";
        if (month > 4)
            m = "месяцев";
        this.text.text = month.ToString() + " " + m + "\n" + price.ToString() + " сум";
    }
}
