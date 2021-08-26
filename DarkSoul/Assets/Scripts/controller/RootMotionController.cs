using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator anim;
    public actorController ac;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = transform.parent.GetComponent<actorController>();
    }
    private void OnAnimatorMove()
    {
        //更新动画的根运动
        ac. OnUpdateRM((object)anim.deltaPosition);
    }
}
