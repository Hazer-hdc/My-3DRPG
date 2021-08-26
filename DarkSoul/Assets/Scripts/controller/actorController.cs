using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actorController : MonoBehaviour
{
    public GameObject model;
    //接收消息的模型
    private GameObject receivedMessageHandler;
    public IUserInput pi;
    private Animator anim;
    private Rigidbody rigid;
    private CapsuleCollider col;
    public CameraController cameraCtr;
    public float walkSpeed = 2.4f;
    //控制奔跑速度
    public float runMultiplier = 2.0f;
    //跳跃速度
    public float jumpVelocity = 3.0f;
    //翻滚速度
    public float rollVelocity = 2.0f;
    //后跳速度
    public float jabVelocity = 3.0f;
    //后跳冲量
    private Vector3 jabThrust ;

    [Space(10)]
    [Header("====  friction setting ====")]
    //为了解决下落时卡墙上，切换角色的物理材质的摩擦力
    public PhysicMaterial frictionNormal;
    public PhysicMaterial frictionZero;
    
    
    //角色水平移动速度
    [SerializeField]
    public Vector3 planarVec;
    //锁定水平移动速度
    private bool lockPlanar = false;
    //处于锁定敌人时，进行跳跃或翻滚就为true，可以让角色朝输入的方向进行跳跃
    private bool trackDirection = false;
    //跳跃冲量
    private Vector3 thrustVec;

    public bool canAttack;

    //动画每帧的根运动
    private Vector3 deltaPos;

    //左手是否装备盾
    public bool leftIsShield = true;

    //========================== 动画状态机层级 ======================

    // Start is called before the first frame update
    void Awake()
    {
        //挂载这个actorController脚本的Handler，例如PlayerHandler
        receivedMessageHandler = this.gameObject;
        pi = GetComponent<IUserInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        //注册各类监听者
        AddListener();

    }

    // Update is called once per frame
    void Update()
    {
        
        //锁定功能
        if (pi.lockOn)
        {
            cameraCtr.ChangeLockOn();
        }

        //锁定与未锁定状态的移动动画
        if(cameraCtr.lockOnState == false)
        {
            if (pi.Dmag > 0)
            {
                //切换角色行走动画，并且通过lerp让行走与奔跑之间有个平滑的过渡。
                anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.run ? 2.0f : 1.0f), 0.5f));
                anim.SetFloat("right", 0);
            }
        }
        else
        {
            anim.SetFloat("forward", pi.Dup * (pi.run ? 2.0f : 1.0f));
            anim.SetFloat("right", pi.Dright * (pi.run ? 2.0f : 1.0f));
        }


        //跳跃
        if (pi.jump)
        {
            canAttack = false;
            anim.SetTrigger("jump");   
        }
        //翻滚
        if (pi.roll || rigid.velocity.magnitude > 7f)
        {
            canAttack = false;
            anim.SetTrigger("roll");
        }
        
        //轻攻击，后面限制攻击条件
        if (canAttack && (pi.rightAttack || pi.leftAttack) && 
            (anim.GetBool("isGround") || checkStateTag("attackR") || checkStateTag("attackL")) )
        {
            if (!pi.shift)
            {
                //轻攻击
                if (pi.rightAttack)
                {
                    //右手轻攻击
                    anim.SetBool("mirrorAttack", false);
                    anim.SetTrigger("attack");
                }
                else if (!leftIsShield)  //如果左手没有装备盾，就可以左手攻击
                {
                    //左手轻攻击
                    anim.SetBool("mirrorAttack", true);
                    anim.SetTrigger("attack");
                }
            }
            else
            {
                //重攻击
                if (pi.rightAttack)
                {
                    //右手重攻击
                    anim.SetBool("mirrorAttack", false);
                    anim.SetTrigger("attack");
                }
                else  //如果左手没有装备盾，就可以左手攻击
                {
                    //左手重攻击
                    if (!leftIsShield)
                    {
                        anim.SetBool("mirrorAttack", true);
                        anim.SetTrigger("attack");
                    }
                    else
                    {
                        //左手盾反
                        anim.SetTrigger("counterBack");
                    }   
                }
            }

        }
        
        //举盾防御，左手得装备盾
        if (leftIsShield && pi.defense && anim.GetBool("isGround"))
        {
            //anim.SetLayerWeight(anim.GetLayerIndex("defense"), 1);
            anim.SetBool("defense", true);
            if (pi.shift)
            {
                anim.SetTrigger("counterBack");
            }
        }
        else
        {
            //anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
            anim.SetBool("defense", false);
        }
        

        //处于锁定敌人的状态与不锁定时移动方式不同
        if (cameraCtr.lockOnState == false)
        {
            //旋转角色模型。当Dup和Dright都为0，就不旋转角色了，否则会把0赋值给forward
            if (pi.Dmag > 0.1f)
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.forwardDirection, 0.3f);
            }
            //修改角色移动速度,
            if (!lockPlanar)
            {
                planarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.run ? runMultiplier : 1.0f);
            }
        }
        else
        {
            //角色要时刻面向被锁定的敌人,模型的前方为playerController的前方（指向敌人）。
            //除非角色在此期间进行跳跃或翻滚，就不让模型指向敌人了。
            if (trackDirection == false)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planarVec.normalized;
            }

            if (!lockPlanar)
            { 
                //此时移动的输入要分为两个轴了，所以不能用Dmag，而是forwardDirection
                planarVec = pi.forwardDirection * walkSpeed * (pi.run ? runMultiplier : 1.0f);
            }
        }

        //触发终结击Timeline
        if (pi.onAction)
        {
            //print("sendMessage");
            MessageCenter.Instance.SendMessage(InteractionEvent.OnStabAction);  
        }
    }

    private void FixedUpdate()
    {
        //rigid.position += planarVec * Time.fixedDeltaTime;
        //直接指派速度，不需要乘时间,后面加上个跳跃的冲量，加完后冲量立刻置为0
        //rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;

        //叠加动画的根运动。
        rigid.position += deltaPos;
        deltaPos = Vector3.zero;
    }

    //查看当前动画层所处的状态是否与stateName一致
    public bool checkState(string stateName, int layerIndex = 0)
    {
        return anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }

    //查看当前动画层所处的状态是否与stateName一致
    public bool checkStateTag(string tagName, int layerIndex = 0)
    {
        return anim.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tagName);
    }

    public void IssueTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void SetBool(string boolName, bool value)
    {
        anim.SetBool(boolName, value);
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    //**********************************************************************************
    //============================   消息系统   ========================================

    /// <summary>
    /// 先消息系统注册监听者
    /// </summary>
    private void AddListener()
    {
        MessageCenter.Instance.AddListener(MotionEvent.PlayerDie, PlayerDie);
        MessageCenter.Instance.AddListener(MotionEvent.EnemyDie, EnemyDie);
        MessageCenter.Instance.AddListener(MotionEvent.InputDisable, InputDisable);
        MessageCenter.Instance.AddListener(MotionEvent.OnJumpEnter, onJumpEnter);
        MessageCenter.Instance.AddListener(MotionEvent.IsGround, IsGround);
        MessageCenter.Instance.AddListener(MotionEvent.IsNotGround, IsNotGround);
        MessageCenter.Instance.AddListener(MotionEvent.OnGroundEnter, OnGroundEnter);
        MessageCenter.Instance.AddListener(MotionEvent.OnGroundExit, OnGroundExit);
        MessageCenter.Instance.AddListener(MotionEvent.OnFallEnter, OnFallEnter);
        MessageCenter.Instance.AddListener(MotionEvent.OnRollEnter, OnRollEnter);
        MessageCenter.Instance.AddListener(MotionEvent.OnJabEnter, OnJabEnter);
        MessageCenter.Instance.AddListener(MotionEvent.OnJabUpdate, OnJabUpdate);

        MessageCenter.Instance.AddListener(BattleEvent.OnAttack1hEnter, OnAttack1hEnter);
        MessageCenter.Instance.AddListener(BattleEvent.OnHitEnter, OnHitEnter);
        MessageCenter.Instance.AddListener(BattleEvent.OnDenfenseEnter, OnDenfenseEnter);
        MessageCenter.Instance.AddListener(BattleEvent.OnStunnedEnter, OnStunnedEnter);
        MessageCenter.Instance.AddListener(BattleEvent.OnCounterBackEnter, OnCounterBackEnter);
        MessageCenter.Instance.AddListener(BattleEvent.OnCounterBackExit, OnCounterBackExit);
    }

    //============================== 状态消息 =============================================

    //这个事件例外，暂时直接在ybot的animator下的RootMotionController中直接访问。
    //攻击时更新动画的根运动
    public void OnUpdateRM(object _deltaPos)
    {
        //当右手攻击、左手攻击且左手不是盾时，就更新根运动
        if (checkStateTag("attackR") || checkStateTag("attackL"))
        {
            deltaPos = (Vector3)_deltaPos;
        }
    }

    public void InputDisable(Message message)
    {
        if(receivedMessageHandler == (GameObject)message.Body)
        {
            pi.inputEnable = false;
        }
    }

    public void PlayerDie(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("PlayerDie");
            pi.inputEnable = false;
            if(true == cameraCtr.lockOnState)
            {
                cameraCtr.ChangeLockOn();
            }
            cameraCtr.enabled = false;
            planarVec = Vector3.zero;
            //发送关闭武器的消息，避免武器没有关的bug
            MessageCenter.Instance.SendMessage(BattleEvent.WeaponDisable, receivedMessageHandler);
        }
    }

    public void EnemyDie(Message message)
    {
        if(receivedMessageHandler == (GameObject)message.Body)
        {
            //print("EnemyDie");
            pi.inputEnable = false;
            if (true == cameraCtr.lockOnState)
            {
                cameraCtr.ChangeLockOn();
            }
            cameraCtr.enabled = false;
            planarVec = Vector3.zero;

            //发送关闭武器的消息，避免武器没有关的bug
            MessageCenter.Instance.SendMessage(BattleEvent.WeaponDisable, receivedMessageHandler);
        }
    }

    //============================== 移动消息 ======================================================

    /// <summary>
    /// Message processing block
    /// </summary>
    public void onJumpEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("onJumpEnter");

            lockPlanar = true;
            pi.inputEnable = false;
            thrustVec = new Vector3(0, jumpVelocity, 0);

            trackDirection = true;
        }
    }

    public void IsGround(Message message)
    { 
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("IsGround");
            anim.SetBool("isGround", true);
        }
    }

    public void IsNotGround(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("IsNotGround");
            anim.SetBool("isGround", false);
        }
    }

    public void OnGroundEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnGroundEnter" );
            pi.inputEnable = true;
            lockPlanar = false;
            canAttack = true;
            col.material = frictionNormal;

            trackDirection = false;

        }

    }
    public void OnGroundExit(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnGroundExit");
            //调整角色的材质的摩擦力为0，避免在空中的时候卡在墙上
            col.material = frictionZero;
        }
    }

    public void OnFallEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnFallEnter");
            pi.inputEnable = false;
            //当掉落时，锁定水平速度，以抛物线下落，以免直直地下落
            lockPlanar = true;
        }
    }

    public void OnRollEnter(Message message)
    {
        //判断该actorController的模型是否与发送消息的模型一致
        //不判断这个，就会导致场景中一个模型发送消息，所有挂载了actorController的模型都受到影响。
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnRollEnter");
            lockPlanar = true;
            pi.inputEnable = false;
            //thrustVec = new Vector3(0, rollVelocity, 0);

            trackDirection = true;
        }
        
    }

    public void OnJabEnter(Message message)
    {
        //判断该actorController的模型是否与发送消息的模型一致
        //不判断这个，就会导致场景中一个模型发送消息，所有挂载了actorController的模型都受到影响。
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnJabEnter");
            lockPlanar = true;
            pi.inputEnable = false;
            trackDirection = true;
        }
    }

    public void OnJabUpdate(Message message)
    {
        //判断该actorController的模型是否与发送消息的模型一致
        //不判断这个，就会导致场景中一个模型发送消息，所有挂载了actorController的模型都受到影响。
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnJabUpdate");
            thrustVec = model.transform.forward * jabVelocity * anim.GetFloat("jabVelocityRate");
        }
    }


    //======== 攻击相关 ===========================

    public void OnAttack1hEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("OnAttackEnter");
            pi.inputEnable = false;
        }
    }


    public void OnHitEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            pi.inputEnable = false;
            //发送关闭武器的消息，避免武器没有关的bug
            MessageCenter.Instance.SendMessage(BattleEvent.WeaponDisable, receivedMessageHandler);
        }
    }

    public void OnDenfenseEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("Denfense");
            pi.inputEnable = false;
        }
    }

    public void OnStunnedEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            
            pi.inputEnable = false;
            planarVec = Vector3.zero;
        }
    }
    //开始盾反
    public void OnCounterBackEnter(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("CounterBackEnter" );
            pi.inputEnable = false;
            planarVec = Vector3.zero;
        }
    }

    //盾反结束时
    public void OnCounterBackExit(Message message)
    {
        if (receivedMessageHandler == (GameObject)message.Body)
        {
            //print("CounterBackExit" );
            ActorManager am = this.GetComponent<ActorManager>();
            //调用weaponManager中的函数来设置sm的状态
            am.wm.CounterBackDiable();
        }
    }
}
