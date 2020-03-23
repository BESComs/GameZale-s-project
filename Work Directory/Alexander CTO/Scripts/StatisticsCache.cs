using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatisticsCache
{
    private const string CacheKey = "StatisticsCache";
    
    public static void AddToCache(StatisticData task)
    {
        var cachedStatistics = new List<StatisticData>();
        
        if (HasCachedStatistics()) cachedStatistics = GetCachedStatistics();
        
        cachedStatistics.Add(task);
        
        var statisticsList = new StatisticsList(cachedStatistics);
        var statisticsCacheString = JsonUtility.ToJson(statisticsList);
        PlayerPrefs.SetString(CacheKey, statisticsCacheString);
        PlayerPrefs.Save();

    }

    public static List<StatisticData> GetCachedStatistics()
    {
        var cachedStatisticsString = PlayerPrefs.GetString(CacheKey);
        var statisticsList = JsonUtility.FromJson<StatisticsList>(cachedStatisticsString); 
        return statisticsList.statistics;
    }

    public static bool HasCachedStatistics()
    {
        return PlayerPrefs.HasKey(CacheKey);
    }

    public static void ClearCache()
    {
        PlayerPrefs.DeleteKey(CacheKey);
        PlayerPrefs.Save();
    }
    
    
    
}