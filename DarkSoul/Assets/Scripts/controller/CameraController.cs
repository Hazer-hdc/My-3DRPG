using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //信号输入
    public IUserInput pi;
    public float smoothTime = 0.2f;
    //控制相机水平旋转
    private GameObject playerHandler;
    //控制相机垂直旋转
    private GameObject cameraHandler;
    private GameObject model;
    private GameObject mainCamera;
    private float currentVerticalAngle;
    private Vector3 currentModelEuler;
    private Vector3 cameraDampVelocity;

    //被锁定的object
    [SerializeField]
    private LockTarget lockTarget;
    public bool lockOnState;
    //锁定敌人时敌人身上的图标
    public Image lockIcon;

    public bool isAI = true;

    // Start is called before the first frame update
    void Awake()
    {
        cameraHandler = transform.parent.gameObject;
        playerHandler = cameraHandler.transform.parent.gameObject;
        currentVerticalAngle = 20;
        model = playerHandler.GetComponent<actorController>().model;

        //只有非AI才可以控制下面这三个
        if (!isAI)
        {
            mainCamera = Camera.main.gameObject;
            //开始运行游戏就隐藏鼠标光标
            Cursor.lockState = CursorLockMode.Locked;
            lockIcon.enabled = false;
        }

        lockOnState = false;
        lockTarget = null;
    }

    private void Start()
    {
        MessageCenter.Instance.AddListener(MotionEvent.EnemyDie, TargetDie);
    }



    private void FixedUpdate()
    {
        //没有锁定敌人时，才可以自由旋转相机，否则摄像机一直看向敌人
        if(lockTarget == null)
        {
            //保存模型当前旋转角度，确保相机旋转时，角色模型不动
            currentModelEuler = model.transform.eulerAngles;
            //相机水平旋转
            playerHandler.transform.Rotate(Vector3.up, pi.Jright);

            //相机垂直旋转
            currentVerticalAngle += -pi.Jup;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -40, 30);
            cameraHandler.transform.localEulerAngles = new Vector3(currentVerticalAngle, 0, 0);

            //确保相机旋转时，角色模型不动
            model.transform.eulerAngles = currentModelEuler;
        }
        else
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandler.transform.forward = tempForward;
            cameraHandler.transform.LookAt(lockTarget.obj.transform);
        }

        if (!isAI)
        {
            // 让摄像机平滑地追上角色
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, transform.position,
                ref cameraDampVelocity, smoothTime);
            mainCamera.transform.LookAt(cameraHandler.transform);

            // 让摄像机追上角色
            //mainCamera.transform.position = transform.position;
            //mainCamera.transform.eulerAngles = transform.eulerAngles;
        }


        //让锁定的图标一直在敌人的中央
        if (lockTarget != null)
        {
            if (!isAI)
            {
                lockIcon.rectTransform.position = Camera.main.WorldToScreenPoint(
                lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            }
            //超出一定范围自动取消锁定
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 8.0f)
            {
                _changeLockOn(null, false, false, isAI);
            }
            
        }
    }


    //反转LockOn状态
    public void ChangeLockOn()
    {
        Vector3 modelOrigin = model.transform.position + new Vector3(0, 1, 0);

        //因为用的是playerHandler的正前方为中心，所以永远在摄像机的正前方锁定敌人。
        Vector3 boxCenter = modelOrigin + playerHandler.transform.forward * 5.0f;

        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5.0f)
            , playerHandler.transform.rotation,LayerMask.GetMask(isAI? "Player" : "Enemy"));

        if(cols.Length > 0)
        {
            //判断目标是否已死亡，如果已死亡就不能锁定
            if (cols[0].gameObject.GetComponent<ActorManager>().sm.isDie)
            {
                return;
            }
            //如果连续对同一个物体按两次lockOn，就会取消锁定，否则锁定该物体
            else if(lockTarget == null || lockTarget.obj != cols[0].gameObject)
            {
                _changeLockOn(new LockTarget(cols[0].gameObject, cols[0].bounds.extents.y), true, true, isAI);
            }
            else
            {
                _changeLockOn(null, false, false, isAI);
            } 
        }
        else
        {
            _changeLockOn(null, false, false, isAI);
        }
    }

    //修改有关锁定状态的各个变量
    private void _changeLockOn(LockTarget lockTarget, bool lockIcon, bool lockState, bool isAI)
    {
        this.lockTarget = lockTarget;
        //非AI才可以修改锁定UI
        if(!isAI)
        {
            this.lockIcon.enabled = lockIcon;
        }
        this.lockOnState = lockState;
    }

    //如果被锁定的目标死亡，就取消锁定。
    public void TargetDie(Message message)
    {
        if(this.lockTarget!= null && this.lockTarget.obj == (GameObject)message.Body)
        {
            _changeLockOn(null, false, false, isAI);
        }
    }

    public class LockTarget
    {
        //目标的Handler，例如Enemyhandler
        public GameObject obj;
        public float halfHeight;

        public LockTarget(GameObject _obj, float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
        }
    }


}
