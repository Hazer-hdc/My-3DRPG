using System;
using System.Reflection;

public abstract class SingleTon<T> where T : class
{
    private static T instance = null;

    //多线程安全机制
    private static readonly object locker = new object();

    public static T Instance
    {
        get
        {
            //线程锁
            lock (locker)
            {
                if (null == instance)
                {
                    //反射获得T的构造函数。
                    var octors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    //获得T的无参构造函数
                    var octor = Array.Find(octors, c => c.GetParameters().Length == 0);

                    if (null == octor)
                    {
                        throw new Exception("No NonPublic constructor without 0 parameter");
                    }

                    instance = octor.Invoke(null) as T;
                }
                return instance;
            }
        }
        
    }

    /// <summary>
    /// 构造函数
    /// 避免外界new
    /// </summary>
    protected SingleTon() { }
}
