using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    public enum Orientation { Clockwise, Anticlockwise }

    public Orientation orientation;

    private Animator animator;
    private NavMeshObstacle obstacle;

    private bool doorClosed = true;

    private string OpenAnimationName => orientation == Orientation.Clockwise ? "clockwise_open" : "anticlockwise_open";

    private string CloseAnimationName => orientation == Orientation.Clockwise ? "clockwise_close" : "anticlockwise_close";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
    }

    public void OpenDoor()
    {
        animator.Play(OpenAnimationName);
        obstacle.enabled = false;
        doorClosed = false;
    }

    public void CloseDoor()
    {
        animator.Play(CloseAnimationName);
        obstacle.enabled = true;
        doorClosed = true;
    }

    public void ToggleDoor()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            ClickProvider.UseClick();
            if (doorClosed) OpenDoor();
            else CloseDoor();
        }
    }
}

public static class ClickProvider
{

    private static int lastClickedFrame;
    
    public static void UseClick()
    {
        lastClickedFrame = Time.frameCount;
    }

    public static bool ClickAvaliable => lastClickedFrame != Time.frameCount;



}