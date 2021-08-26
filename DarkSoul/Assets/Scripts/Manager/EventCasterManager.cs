using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCasterManager : Manager
{
    public string eventName;
    public bool isActive;

    private void Start()
    {
        am = transform.parent.GetComponent<ActorManager>();
    }
}
