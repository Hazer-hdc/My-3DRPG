using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //在攻击动画事件中起作用，
    public void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }
}
