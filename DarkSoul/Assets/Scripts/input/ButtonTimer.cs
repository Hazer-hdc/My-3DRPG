using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTimer 
{
    //计时器当前的状态
    public enum STATE
    {
        FINISHED,
        RUN
    }
    public STATE state;

    //计时上限
    public float duration = 1.0f;
    //已持续的时间
    private float elapsedTime = 0;

    public void Tick()
    {
        //开始计时
        if(state == STATE.RUN)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
                state = STATE.FINISHED;
        }
    }

    //启动计时器
    public void GO()
    {
        elapsedTime = 0;
        state = STATE.RUN;
    }
}
