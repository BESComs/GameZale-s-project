using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StudentNameLabel : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        LoadName();
    }

    public async void LoadName()
    {
        await Task.Delay(1000);
        var fullName = await ServerRequests.GetStudentNameAsync();
        textMesh.text = fullName;
    }
}