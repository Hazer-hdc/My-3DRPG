using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Manager
{
    
    public Collider leftWeaponCol;
    public Collider rightWeaponCol;

    private WeaponController leftWc;
    private WeaponController rightWc;

    public GameObject leftWeaponHandle;
    public GameObject rightWeaponHandle;

    private void Start()
    {
        leftWeaponHandle = FindWeaponHandle("Left", transform.parent.name);
        rightWeaponHandle = FindWeaponHandle("Right", transform.parent.name);

        if(rightWeaponHandle != null)
        {
            rightWeaponCol = rightWeaponHandle.GetComponentInChildren<Collider>();
            rightWeaponCol.enabled = false;
            rightWc = BindWeaponController(rightWeaponHandle);
        }
        if(leftWeaponHandle != null)
        {
            leftWeaponCol = leftWeaponHandle.GetComponentInChildren<Collider>();
            //leftWeaponCol.enabled = false;
            leftWc = BindWeaponController(leftWeaponHandle);
        }


        MessageCenter.Instance.AddListener(BattleEvent.WeaponDisable, WeaponDisable);
    }

    public WeaponController BindWeaponController(GameObject targetObj)
    {
        WeaponController tempWc;
        tempWc = targetObj.GetComponent<WeaponController>();
        if(tempWc == null)
        {
            tempWc = targetObj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }
    public GameObject FindWeaponHandle(string targetType, string parentName)
    {
        if (parentName == "PlayerHandler")
        {
            if (targetType == "Left")
            {
                return GameObject.FindWithTag("LeftWeaponHandler");
            }
            else
            {
                return GameObject.FindWithTag("RightWeaponHandler");
            }
        }
        else if (parentName == "EnemyHandler")
        {
            if (targetType == "Left")
            {
                return GameObject.FindWithTag("EnemyLeftWeaponHandler");
            }
            else
            {
                return GameObject.FindWithTag("EnemyRightWeaponHandler");
            }
        }

        return null;
    }

    public void WeaponEnable(Message message = null)
    {
        if (am.ac.checkStateTag("attackR"))
        {
            rightWeaponCol.enabled = true;
        }
        else if (am.ac.checkStateTag("attackL"))
        {
            leftWeaponCol.enabled = true;
        } 
    }

    public void WeaponDisable(Message message = null)
    {
        //如果不是通过消息中心触发的调用的，直接false。否则就要判断一下了
        if (message == null)
        {
            rightWeaponCol.enabled = false;
            leftWeaponCol.enabled = false;
        }
        else if(this.transform.parent.gameObject == (GameObject)message.Body)
        {
            rightWeaponCol.enabled = false;
            leftWeaponCol.enabled = false;
        }
    }

    //AnimEventController脚本会调用这个函数
    public void CounterBackEnable()
    {
        am.sm.setIsCounterBack(true);
    }

    //AnimEventController脚本会调用这个函数
    public void CounterBackDiable()
    {
        am.sm.setIsCounterBack(false);
    }

    
}
