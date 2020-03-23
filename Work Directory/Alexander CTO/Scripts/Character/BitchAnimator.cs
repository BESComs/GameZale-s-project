using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BitchAnimator : MonoBehaviour
{


    [Button]
    public void RunBitch()
    {
        GetComponent<Animator>().CrossFadeInFixedTime("run", 0.2f);
    }

    [Button]
    public void Walk()
    {
        GetComponent<Animator>().CrossFadeInFixedTime("walk", 0.2f);
    }
    
    [Button]
    public void StopWaitAMinute()
    {
        GetComponent<Animator>().CrossFadeInFixedTime("idle", 0.2f);
    }
    
    
}