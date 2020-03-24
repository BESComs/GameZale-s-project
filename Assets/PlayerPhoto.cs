using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerPhoto : MonoBehaviour
{
    private string lastPhotoUrl = "";
    private Sprite defaultSprite;

    private async void Awake()
    {
        var photoUrl = await ServerRequests.GetStudentPhotoPathAsync();
        Init(photoUrl);
    }


    public void Init(string photoUrl)
    {
        if (defaultSprite == null) defaultSprite = GetComponent<Image>().sprite;
        if (string.IsNullOrEmpty(photoUrl))
        {
            GetComponent<Image>().sprite = defaultSprite;
            lastPhotoUrl = photoUrl;
            return;
        }

        if (lastPhotoUrl == photoUrl) return;
        lastPhotoUrl = photoUrl;
        DownloadImage(photoUrl);
    }

    public async void DownloadImage(string photoUrl)
    {
        try
        {
            var photo = await ServerRequests.GetAthletePhoto(photoUrl);
            GetComponent<Image>().sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector3.up / 2f);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
