using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider.value = AudioListener.volume;
    }

    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
}