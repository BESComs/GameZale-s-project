using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FindInterfaces
{
    public static List<T> Find<T>(  )
    {
        var interfaces = new List<T>();
        var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach( var rootGameObject in rootGameObjects )
        {
            var childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
            interfaces.AddRange(childrenInterfaces);
        }

        return interfaces;
    }
}