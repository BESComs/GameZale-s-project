using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{
    public void CallTriggers()
    {
        var triggerTargets = GetComponents<ITriggerTarget>();
        foreach (var triggerTarget in triggerTargets)
        {
            triggerTarget.OnTriggerCall();
        }
    }
}