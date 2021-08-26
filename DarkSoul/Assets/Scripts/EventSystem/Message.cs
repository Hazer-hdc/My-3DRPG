/// <summary>
/// 消息类
/// </summary>
public class Message
{
    public int Type  //发送的消息类型
    {
        get;
        private set;
    }
    public object Body  //消息主体
    {
        get;
        set;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <param name="body">消息体</param>
    public Message(int type, object body)
    {
        Type = type;
        Body = body;
    }
}

