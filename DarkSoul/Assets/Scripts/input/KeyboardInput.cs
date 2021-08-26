using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput
{
    //Variable
    [Header("==== key settings ====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyRun;
    public string keyJump;
    public string keyRightAttack;
    public string keyLeftAttack;
    public string keyDefense;
    public string keyLockOn;
    public string keyShift;

    public string keyJup;
    public string keyJdown;
    public string keyJright;
    public string keyJleft;

    [Header("==== mouse setting ====")]
    //鼠标水平和垂直灵敏度
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    // Update is called once per frame
    void Update()
    {
        //更新各类按钮的状态，这些对象在父类中IUserInput实例化了
        ButtonJump.Tick(Input.GetKey(keyJump));
        ButtonRightAttack.Tick(Input.GetKey(keyRightAttack));
        ButtonLeftAttack.Tick(Input.GetKey(keyLeftAttack));
        ButtonRun.Tick(Input.GetKey(keyRun));
        ButtonLockOn.Tick(Input.GetKey(keyLockOn));
        ButtonShift.Tick(Input.GetKey(keyShift));
        ButtonDefense.Tick(Input.GetKey(keyDefense));
    
        //run信号为长按,黑魂中run是长按才会触发的
        run = (ButtonRun.isPressing && !ButtonRun.isDelaying) || ButtonRun.isExtending;
        //在奔跑过程中再按一次奔跑键才能跳跃
        jump = ButtonJump.onPressed && ButtonRun.isPressing;
        //按下奔跑键立马释放就后跳，如果此时还按着方向键就向前翻滚，方向键在状态机中判断
        roll = ButtonRun.onReleased && ButtonRun.isDelaying ;

        //锁定敌人
        lockOn = ButtonLockOn.onPressed;

        rightAttack = ButtonRightAttack.onPressed;
        
        leftAttack = ButtonLeftAttack.onPressed;
        defense = ButtonDefense.isPressing;
        shift = ButtonShift.isPressing;

        onAction = Input.GetKeyDown(KeyCode.H);


        //鼠标控制摄像机旋转
        Jup = Input.GetAxis("Mouse Y") * mouseSensitivityY;
        Jright = Input.GetAxis("Mouse X") * mouseSensitivityX;

        //将wasd输入转化微signal。
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        //关闭输入
        if (inputEnable == false)
        {
            targetDup = 0;
            targetDright = 0;
        }

        //通过smoothDamp让singal平滑过渡
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        if (Mathf.Abs(Dup) < 0.001)
        {
            Dup = 0;
        }
        if (Mathf.Abs(Dright) < 0.001)
        {
            Dright = 0;
        }

        //更新Dmag和forwardDirection
        UpdateDmagDforward();
    }

}
