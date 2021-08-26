using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase 
{
    private string weaponDataBaseFileName = "weaponData";

    public JSONObject WeaponDataBase { get; private set; }

    public DataBase()
    {
        TextAsset weaponContent = Resources.Load(weaponDataBaseFileName) as TextAsset;
        WeaponDataBase = new JSONObject(weaponContent.text);
    }
}
