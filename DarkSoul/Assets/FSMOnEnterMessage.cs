using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnEnterMessage : StateMachineBehaviour
{
    //MotionEvent类型消息的集合
    public MotionEvent[] motionMessages;
    //BattleEvent类型消息的集合
    public BattleEvent[] battleMessages;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //遍历所有想要发送的消息
        if (motionMessages.Length > 0)
            foreach (var msg in motionMessages)
            {
                MessageCenter.Instance.SendMessage(msg, animator.gameObject);
            }

        if (battleMessages.Length > 0)
            foreach (var msg in battleMessages)
            {
                MessageCenter.Instance.SendMessage(msg, animator.gameObject);
            }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
