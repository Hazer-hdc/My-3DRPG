using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public actorController ac;
    public BettleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    public DirectorManager dm;
    public InteractionManager im;
    // Start is called before the first frame update
    void Awake()
    {
        ac = GetComponent<actorController>();
        GameObject manager = transform.Find("Manager").gameObject;

        bm = Bind<BettleManager>(manager);
        wm = Bind<WeaponManager>(manager);
        sm = Bind<StateManager>(manager);
        dm = Bind<DirectorManager>(manager);
        im = Bind<InteractionManager>(manager);

        Addlistener();
    }

   //这里where的作用是告诉编译器T必须是一个manager或其子类，不然报错
   private T Bind<T> (GameObject obj) where T : Manager
    {
        T tempInstance;
        tempInstance = obj.GetComponent<T>();
        if(tempInstance == null)
        {
            tempInstance = obj.AddComponent<T>();
        }
        tempInstance.am = this;
        return tempInstance;
    }

    //传递对方的Weaponcontroller进来
    public void TryDoDemage(WeaponController targetWc, bool counterBackValid)
    {
        //处于有效盾反的角度内，盾反才会奏效
        if (sm.isCounterBacker && counterBackValid)
        {
            //触发敌人的被盾反状态
            targetWc.wm.am.BeCounterBack();
        }
        //如果处于无敌状态，什么都不要做
        //如果处于防御状态，那就触发bloacked格挡
        //否则扣血
        else if (sm.isInvincible)
        {
            return;
        }
        else if (sm.isDefense)
        {
            Blocked();
        }
        else
        {
            HitOrDie(targetWc);
        }
    }

    private void HitOrDie(WeaponController targetWc)
    {
        sm.ChangeHP(-1 * targetWc.GetATK());
        if (sm.HP <= 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }

    public void Blocked()
    {
        ac.IssueTrigger("blocked");
    }

    public void Hit()
    {
        ac.IssueTrigger("hit");
        MessageCenter.Instance.SendMessage(MotionEvent.InputDisable, this.gameObject);
    }

    public void Die()
    {
        ac.IssueTrigger("die");
        
        switch (this.gameObject.layer)
        {
            //Enemy层
            case 11:
                MessageCenter.Instance.SendMessage(MotionEvent.EnemyDie, this.gameObject);
                CapsuleCollider capCol = bm.GetComponent<CapsuleCollider>();
                capCol.enabled = false;
                break;
            //Player层
            case 12:
                MessageCenter.Instance.SendMessage(MotionEvent.PlayerDie, this.gameObject);
                break;
        }

    }

    //被盾反了
    public void BeCounterBack()
    {
        ac.IssueTrigger("beCounterBack");
    }

    //在终结技Timeline期间锁定动画状态机
    public void LockUnLockActorController(bool value)
    {
        ac.SetBool("lock", value);
    }

    public actorController GetActorController()
    {
        return ac;
    }

    private void OnStabAction(Message message)
    {
        //print("OnStabAction");
        //print(im.overlapECsteMs.Count);
        if(im.overlapECsteMs.Count > 0)
        {
            if (im.overlapECsteMs[0].eventName == "frontStab")
            {
                //向DirectorManager发送消息，开始播放剧本
                dm.Play("frontStab", this, im.overlapECsteMs[0].am);
            }
            else if (im.overlapECsteMs[0].eventName == "openBox")
            {
                //向DirectorManager发送消息，开始播放剧本
                dm.Play("openBox", this, im.overlapECsteMs[0].am);
            }

        }
    }

    private void Addlistener()
    {
        MessageCenter.Instance.AddListener(InteractionEvent.OnStabAction, OnStabAction);
    }
}
