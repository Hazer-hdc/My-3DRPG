using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个可以在刚刚添加这个脚本时，就自动添加相应的组件
[RequireComponent(typeof(CapsuleCollider))]
public class BettleManager : Manager
{
    public float halfAttackAngle = 90;
    public float halfCounterBackAngle = 30;
    public float halfFaceToFaceAngle = 30;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Weapon")
        {
            WeaponController targetWc = other.GetComponentInParent<WeaponController>();
            GameObject attacker = targetWc.wm.am.gameObject;
            GameObject receiver = this.am.gameObject;

            //敌人->玩家的方向
            Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
            //玩家->敌人的方向
            Vector3 coutnerDir = attacker.transform.position - receiver.transform.position;

            //敌人的正前方与敌人->玩家的方向
            float attackingAngle = Vector3.Angle(attacker.transform.forward, attackingDir);

            //敌人的前方和玩家的前方的夹角,面对面的夹角
            float counterAngle1 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);
            //敌人是否在玩家的盾反范围内
            float counterAngle2 = Vector3.Angle(coutnerDir, receiver.transform.forward);

            //角色与敌人此时是否面对面，并且是否处于玩家的盾反范围内，决定了能否盾反。
            bool counterBackValid = (counterAngle2 < halfFaceToFaceAngle && Mathf.Abs(counterAngle1 - 180) < halfCounterBackAngle);
            
            //处于有效攻击角度才会TryDoDamage
            if (attackingAngle <= halfAttackAngle)
            {
                am.TryDoDemage(targetWc, counterBackValid);
            }

        }
    }

    


}
