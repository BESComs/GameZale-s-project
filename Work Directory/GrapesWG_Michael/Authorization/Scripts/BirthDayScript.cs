using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirthDayScript : MonoBehaviour
{
    [Obsolete] private Dropdown firstDay, secondDay, month, firstYear, secondYear;
    [SerializeField] private Dropdown Day, Month, Year;
    [SerializeField] private TextMeshProUGUI date;
    private const string txt = "Дата вашего рождениия : ";
    private const string parseError = "<color=#D95757>Такой даты не существует</color>";
    public DateTime CurrentDate;
    async void Start()
    {
        InitStart();
    }
    
    
    [Obsolete] async void InitStartOld()
    {
            month.options.Clear();
            for (var i = 12; i >= 1; i--)
                month.options.Add(new Dropdown.OptionData((i < 10 ? "0" : "") + i));
            await SetOptions(firstYear,(DateTime.Now.Year-3)/10, 195);
            await SetOptions(secondYear,(DateTime.Now.Year-3)%10, 0);
            await SetOptions(firstDay,3, 0);
            await SetOptions(secondDay,9, 0);
            firstYear.value = 1;
            secondYear.value = 1;
            month.value = 1;
            firstDay.value = 1;
            secondDay.value = 1;
    }

    public async void InitStart()
    {
        await Options(Day,31, 1);
        await Options(Month,12, 1);
        await Options(Year,DateTime.Now.Year-3, DateTime.Now.Year-65);
        Month.value = 1;
        Year.value = 1;
        Day.value = 1;
    }

    [Obsolete]async Task SetOptions(Dropdown drop, int start, int end)
    {
        drop.options.Clear();
        for(var i = start; i>=end;i--)
            drop.options.Add(new Dropdown.OptionData(i.ToString()));
    }
    async Task Options(Dropdown drop, int start, int end)
    {
        drop.options.Clear();
        for(var i = start; i>=end;i--)
            drop.options.Add(new Dropdown.OptionData((i < 10 ? "0" : "") + i));
    }
    
    void Update()
    {
        TryGetDate();
    }

    void TryGetDate()
    {
        try {
            CurrentDate = DateTime.ParseExact(MostLazy(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            date.text = txt + MostLazy();
        } catch {
            date.text = parseError;
            CurrentDate = DateTime.Today;
        }
    }

    string MostLazy()
    {
        return Lazy(Day) + "." + Lazy(Month) + "." + Lazy(Year);
    }
    
    [Obsolete]string MostLazyOld()
    {
        return Lazy(firstDay) + "" + Lazy(secondDay) + "." + Lazy(month) + "." + Lazy(firstYear) + "" +
               Lazy(secondYear);
    }
    string Lazy(Dropdown drop)
    {
        drop.captionText.text = drop.options[drop.value].text;
        return drop.options[drop.value].text;
    }

    public async void ResetDays()
    {
        var index = Day.value;
        var days = DateTime.DaysInMonth(int.Parse(Lazy(Year)), int.Parse(Lazy(Month)) );
        await SetOptions(Day, days, 1);
        Day.value = days <= index ? index : 0;
        TryGetDate();
    }
   [Obsolete] public async void ResetFirstDays()
    {
        var index = firstDay.value;
        var days = DateTime.DaysInMonth(int.Parse(Lazy(firstYear)+Lazy(secondYear)), int.Parse(Lazy(month)) ) / 10;
        await SetOptions(firstDay, days, 0 );
        firstDay.value = days <= index ? index : 0;
        TryGetDate();
    }
   [Obsolete] public async void ResetSecondDays()
    {
        var index = secondDay.value;
        var days = DateTime.DaysInMonth(int.Parse(Lazy(firstYear)+Lazy(secondYear)), int.Parse(Lazy(month)) ) ;
        var end = 0;
        if (int.Parse(Lazy(firstDay)) == 0)
        {
            end = 1;
            if (index == 10) index = 0;
        }
        days = int.Parse(Lazy(firstDay)) == Mathf.FloorToInt(days / 10f) ? days % 10 : 9;
        await SetOptions(secondDay, days, end );
        secondDay.value = days <= index ? 0 : index;
        TryGetDate();
    }
}
