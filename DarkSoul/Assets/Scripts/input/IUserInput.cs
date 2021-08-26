using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    //public ButtonInput Button


    [Header("==== output signals ====")]
    public float Dup;
    public float Dright;
    //Dup和Dright各自的平方相加，再开方,用来控制角色的移动速度和动画
    public float Dmag;
    //改变角色的朝向，用来旋转角色
    public Vector3 forwardDirection;
    //摄像头移动方向
    public float Jup;
    public float Jright;


    
    
    //1、pressing signal
    public bool run;
    public bool defense;
    //2、trigger once signal
    public bool jump;
    public bool rightAttack;
    public bool leftAttack;
    public bool roll;
    public bool lockOn;
    public bool shift;   //换档键，用来凑组合键，例如重攻击
    public bool onAction; //用来触发各种Timeline
    
    public ButtonInput ButtonJump = new ButtonInput();
    public ButtonInput ButtonRightAttack = new ButtonInput();
    public ButtonInput ButtonLeftAttack = new ButtonInput();
    public ButtonInput ButtonRun = new ButtonInput();
    public ButtonInput ButtonLockOn = new ButtonInput();
    public ButtonInput ButtonShift = new ButtonInput();
    public ButtonInput ButtonDefense = new ButtonInput();

    [Header("==== others ====")]
    public bool inputEnable = true;

    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;

    protected Vector2 sphereToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2);

        return output;
    }

    //计算玩家Dmag和forwardDirection的值
    public void UpdateDmagDforward()
    {
        
        //将水平输入和垂直输入映射到圆上
        Vector2 tempDAxis = sphereToCircle(new Vector2(Dright, Dup));
        float newDup = tempDAxis.y;
        float newDright = tempDAxis.x;

        Dmag = Mathf.Sqrt(newDup * newDup + newDright * newDright);
        forwardDirection = newDup * transform.forward + newDright * transform.right;
    }
    //计算AI的Dmag和forwardDirection的值
    public void UpdateDmagDforward(float Dup, float Dright)
    {
        Dmag = Mathf.Sqrt(Dup * Dup + Dright * Dright);
        forwardDirection = Dup * transform.forward + Dright * transform.right;
    }
}
