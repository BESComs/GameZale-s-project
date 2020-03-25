using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

public class CloudScreenFader : SerializedMonoBehaviour, IScreenFader
{

    public Animator animator;

    private float crossFadeTime = .1f;
    
    private float fadeInTime;
    private float fadeOutTime;

    private string fadeInName = "Fade In";
    private string fadeOutName = "Fade Out";
    
    

    private void Awake()
    {
        var clips = animator.runtimeAnimatorController.animationClips;

        fadeInTime = clips.First(t => t.name == fadeInName).length;
        fadeOutTime = clips.First(t => t.name == fadeOutName).length;       
    }

    public async Task FadeInAsync()
    {
        animator.CrossFadeInFixedTime(fadeInName, crossFadeTime);
        int fadeTime = (int) (fadeInTime * 1000);
        await Task.Delay(fadeTime);
        Debug.Log("Fade in ended");
    }

    public async Task FadeOutAsync()
    {
        animator.CrossFadeInFixedTime(fadeOutName, crossFadeTime);
        int fadeTime = (int) (fadeOutTime * 1000);
        await Task.Delay(fadeTime);
    }
}
