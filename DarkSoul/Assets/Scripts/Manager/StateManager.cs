using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : Manager
{
    public float HP = 20;
    private float HPmax = 20;

    [Header("1ts order state flags")]
    public bool isGround;
    public bool isJump;
    public bool isRoll;
    public bool isFall;
    public bool isJab;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    //是否在防御时被攻击到
    public bool isBolcked;
    public bool isDefense;
    public bool isCounterBacker = false;

    [Header("2ts order state flags")]
    //无敌状态
    public bool isInvincible;


    private void Start()
    {
        HP = HPmax;
    }
    private void Update()
    {
        isGround = am.ac.checkState("ground");
        isJump = am.ac.checkState("jump");
        isRoll = am.ac.checkState("roll");
        isFall = am.ac.checkState("fall");
        isJab = am.ac.checkState("jab");
        isAttack = am.ac.checkStateTag("attackR") || am.ac.checkStateTag("attackL");
        isHit = am.ac.checkState("fromImpace");
        isDie = am.ac.checkState("die");
        isBolcked = am.ac.checkState("blocked");
        isDefense = am.ac.checkState("defense1h") || isBolcked;

        isInvincible = isRoll || isJab;
    }

    public void ChangeHP(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HPmax);    
    }
    
    public void setIsCounterBack(bool value)
    {
        isCounterBacker = value;
    }
}
