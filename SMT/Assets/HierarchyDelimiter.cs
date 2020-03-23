using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class HierarchyDelimiter : MonoBehaviour
{
    private const int DashCount = 8;

    public string groupName;

    private void UpdateName(int nameCount)
    {
        name = (new string('-', DashCount) + groupName).PadRight(nameCount, '-');
    }
    
    private void OnValidate()
    {
        var delimiters = transform.parent.GetComponentsInChildren<HierarchyDelimiter>();
        var maxGroupName = delimiters.Max(d => d.groupName.Length) + DashCount * 2;
        foreach (var delimiter in delimiters)
        {
            delimiter.UpdateName(maxGroupName);
        }
        
    }
}