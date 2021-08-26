using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInput 
{

    public bool isPressing = false;
    //刚刚按下
    public bool onPressed = false;
    //刚刚释放
    public bool onReleased = false;
   
    private bool currenState = false;
    private bool lastState = false;

    //======================== 双击功能 ================================
    //     ============== 黑魂版 ======================
    //是否处于连按延长时间内,用来判断double trigger
    public bool isExtending = false;
    //计时上限
    public float extendingDuration = 0.15f;
    //连按延长时间的计时器，用来实现double trigger
    private ButtonTimer extTimer = new ButtonTimer();


    //     ============== 正常版（额外加上以下变量） ======================
    //累计的连续按下按键的次数，用来判断double trigger
    //public int accumulatedCount = 0;
    //public bool doublePressed = false;


    //======================== 长按功能 ===================================
    //实现长按功能
    public bool isDelaying = false;
    public float delayingDuration = 0.15f;
    private ButtonTimer delayTimer = new ButtonTimer();


    public void Tick(bool input)
    {
        extTimer.Tick();
        delayTimer.Tick();

        currenState = input;

        isPressing = currenState;

        onPressed = false;
        onReleased = false;
        isDelaying = false;
        if (currenState != lastState)
        {
            if(currenState == true)
            {
                onPressed = true;

                //====================== 长按功能 ========================
                StartTimer(delayTimer, delayingDuration);

                //====================== 双击功能 ========================
                //      ============ 正常版 ===============
                //增加累计数
                //accumulatedCount++;
                ////从按下按键那一刻开始计时，如果正在计时中，就不要重新开启计时器
                //if(!isExtending)
                //    StartTimer(extTimer, extendingDuration);
            }
            else
            {
                onReleased = true;
                //====================== 双击功能 ========================
                //      ============ 黑魂版 ===============
                StartTimer(extTimer, extendingDuration);
            }
        }

        lastState = currenState;

        //====================== 长按功能 ========================
        if (delayTimer.state == ButtonTimer.STATE.RUN)
        {
            isDelaying = true;
        }

        //========================== 双击功能 =============================

        //    ================== 黑魂版 ========================
        //判断是否处于连按延长时间
        isExtending = (extTimer.state == ButtonTimer.STATE.RUN);

        //    ================== 正常版（额外加上以下代码） ================
        //判断是否在连按延长时间内连续按两次
        //doublePressed = false;
        //if (!isExtending)
        //{
        //    doublePressed = false;
        //    accumulatedCount = 0;
        //}
        //else if(accumulatedCount == 2)
        //{
        //    doublePressed = true;
        //    accumulatedCount = 0;
        //}
    }

    public void StartTimer(ButtonTimer timer, float duration)
    {
        timer.duration = duration;
        timer.GO();
    }
}
