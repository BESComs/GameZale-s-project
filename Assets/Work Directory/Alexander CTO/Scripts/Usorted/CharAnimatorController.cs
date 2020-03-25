using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharAnimatorController : MonoBehaviour
{
    private string lastPlayedAnimation;
    
    private Animator animator;
    private NavMeshAgent nvm;

    
    public float stopVelocity = 0.1f;
    private static readonly int SpeedId = Animator.StringToHash("speed");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        nvm = GetComponent<NavMeshAgent>();
    }

    public float speedd;
    
    private void Update()
    {
        var speed = nvm.velocity.magnitude / nvm.speed;
        speedd = speed;        
        animator.SetFloat(SpeedId, speed);
    }

    public void PlayAnimation(string animationName, float crossFadeTime = 0.22f)
    {
        if (animationName == lastPlayedAnimation) return;
        animator.CrossFadeInFixedTime(animationName, crossFadeTime);
        lastPlayedAnimation = animationName;
    }
    
    
}

public static class CharAnimation
{
    public const string Idle = "idle";
    public const string Walk = "walk";
    public const string Run = "idle";
    public const string BenchSeat = "walk";
    public const string BenchIdle = "";
    public const string BenchStandUp = "gruge";
    
}