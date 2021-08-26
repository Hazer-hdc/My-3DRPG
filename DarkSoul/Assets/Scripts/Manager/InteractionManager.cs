using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Manager
{

    private CapsuleCollider interCol;

    //保存碰撞到的EventCasteManager
    public List<EventCasterManager> overlapECsteMs = new List<EventCasterManager>();

    // Start is called before the first frame update
    void Start()
    {
        interCol = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("OnTriggerEnter");
        EventCasterManager[] eCastMs = other.GetComponents<EventCasterManager>();
        foreach(var eCastM in eCastMs)
        {
            if (!overlapECsteMs.Contains(eCastM)){
                overlapECsteMs.Add(eCastM);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EventCasterManager[] eCastMs = other.GetComponents<EventCasterManager>();
        foreach (var eCastM in eCastMs)
        {
            if (overlapECsteMs.Contains(eCastM))
            {
                overlapECsteMs.Remove(eCastM);
            }
        }
    }
}
