using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCollisionTrigger : TriggerBase
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(GameTags.Player))
        {
            CallTriggers();
        }
    }
}