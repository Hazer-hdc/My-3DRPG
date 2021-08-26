using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnUpdate : StateMachineBehaviour
{
    //MotionEvent类型消息的集合
    public MotionEvent[] motionMessages;
    //BattleEvent类型消息的集合
    public BattleEvent[] battleMessages;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //发送这个消息的Handler，例如PlayerHandler，EnemyHandler
        GameObject handler = animator.transform.parent.gameObject;
        //遍历所有想要发送的消息
        if (motionMessages.Length > 0)
            foreach (var msg in motionMessages)
            {
                MessageCenter.Instance.SendMessage(msg, handler);
            }

        if (battleMessages.Length > 0)
            foreach (var msg in battleMessages)
            {
                MessageCenter.Instance.SendMessage(msg, handler);
            }
    }

    
}
