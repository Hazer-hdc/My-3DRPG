using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponManager wm;
    public WeaponDate wd;

    private void Start()
    {
        wd = GetComponent<WeaponDate>();
    }

    public float GetATK()
    {
        return wd.ATK;
    }
}
