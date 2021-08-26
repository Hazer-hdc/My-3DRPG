using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//地面感应器，用来感应人物是否接触地面
public class OnGroundSensor : MonoBehaviour
{
    private GameObject handler;
    public CapsuleCollider capCol;
    private Vector3 point0;
    private Vector3 point1;
    private float radius;
    public float offset = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        handler = transform.parent.gameObject;

        radius = capCol.radius - 0.05f ;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        point0 = transform.position + transform.up * (radius - offset) ;
        point1 = transform.position + transform.up * (capCol.height - radius - offset);


        Collider[] outputCol = Physics.OverlapCapsule(point0, point1, radius, LayerMask.GetMask("Ground"));
        if (outputCol.Length != 0)
        {
            MessageCenter.Instance.SendMessage(MotionEvent.IsGround, handler);
        }
        else
        {
            MessageCenter.Instance.SendMessage(MotionEvent.IsNotGround, handler);
        }
    }
}
