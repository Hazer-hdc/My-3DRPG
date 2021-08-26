using System;
using System.Collections.Generic;
using System.Diagnostics;

//移动事件
public enum MotionEvent
{
    OnUpdateRM = 1000,

    InputDisable,


    IsGround,
    IsNotGround,
    OnGroundEnter,
    OnGroundExit,

    OnJumpEnter,

    OnFallEnter,

    OnRollEnter,

    OnJabEnter,
    OnJabUpdate,

    PlayerDie,
    EnemyDie
}

//战斗事件
public enum BattleEvent
{
    OnAttack1hEnter = 10000,
    OnHitEnter,
    OnDenfenseEnter,
    OnStunnedEnter,
    OnCounterBackEnter,
    WeaponEnable,
    WeaponDisable,
    OnCounterBackExit
}

public enum InteractionEvent
{
    OnStabAction = 20000
}

//消息中心
public class MessageCenter : SingleTon<MessageCenter>
{

    //消息委托
    public delegate void messageDelHandle(Message message = null);
    //消息字典
    public static Dictionary<int, messageDelHandle> messageMap = new Dictionary<int, messageDelHandle>();

    /// <summary>
    /// 构造函数
    /// 避免外界new
    /// </summary>
    private MessageCenter() { }

    //注册监听，让del监听messageType
    public void AddListener(int messageType, messageDelHandle del)
    {
        if (del == null) return;
        //获得messageType当前的全部监听者
        messageMap.TryGetValue(messageType, out messageDelHandle temp);
        //将新的监听者接在后面
        messageMap[messageType] = (messageDelHandle)Delegate.Combine(temp, del);
        
    }

    public void RemoveListener(int messageType, messageDelHandle del)
    {
        if (del == null) return;
        
        messageMap[messageType] = (messageDelHandle)Delegate.Remove(messageMap[messageType], del);
    }

    
    public void Clear()
    {
        messageMap.Clear();
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="messageType">消息类型 </param>
    /// <param name="body"> 发送消息主体 </param>
    public void SendMessage(int messageType, object body = null)
    {
        
        messageDelHandle handle;
        if (messageMap.TryGetValue(messageType, out handle))
        {
            Message evt = new Message(messageType, body);
            try
            {
                if(handle != null)
                {
                    handle.Invoke(evt);
                }
            }
            catch (System.Exception e)
            {
                Debug.Print("SendMessage:", evt.Type.ToString(), e.Message, e.StackTrace, e);
            }
        }

    }



    #region 枚举类型接口

    #region MessageType
    public void AddListener(MotionEvent messageType, messageDelHandle handle)
    {
        AddListener((int)messageType, handle);
    }
    public void RemoveListener(MotionEvent messageType, messageDelHandle handle)
    {
        RemoveListener((int)messageType, handle);
    }
    public void SendMessage(MotionEvent messageType, object body = null)
    {
        SendMessage((int)messageType, body);
    }
    #endregion


    #region BattleEvent
    public void AddListener(BattleEvent messageType, messageDelHandle handle)
    {
        AddListener((int)messageType, handle);
    }
    public void RemoveListener(BattleEvent messageType, messageDelHandle handle)
    {
        RemoveListener((int)messageType, handle);
    }
    public void SendMessage(BattleEvent messageType, object body = null)
    {
        SendMessage((int)messageType, body);
    }
    #endregion

    #region BattleEvent
    public void AddListener(InteractionEvent messageType, messageDelHandle handle)
    {
        AddListener((int)messageType, handle);
    }
    public void RemoveListener(InteractionEvent messageType, messageDelHandle handle)
    {
        RemoveListener((int)messageType, handle);
    }
    public void SendMessage(InteractionEvent messageType, object body = null)
    {
        SendMessage((int)messageType, body);
    }
    #endregion



    #endregion



}
