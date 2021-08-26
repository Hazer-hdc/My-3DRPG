using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventController : MonoBehaviour
{
    public WeaponManager wm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WeaponEnable()
    {
        wm.WeaponEnable();
    }

    public void WeaponDisable()
    {
        wm.WeaponDisable();
    }

    public void CounterBackEnable()
    {
        wm.CounterBackEnable();
    }

    public void CounterBackDiable()
    {
        wm.CounterBackDiable();
    }
}
