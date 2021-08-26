using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private DataBase weaponDB;
    // Start is called before the first frame update
    void Awake()
    {
        CheckGameObject();
        CheckSingle();
    }

    private void Start()
    {
        InitWeaponDB();
    }

    //GameManager只能挂载在tag为GM的obj上
    private void CheckGameObject()
    {
        if (tag != "GM")
            Destroy(this);
    }

    //单例
    private void CheckSingle()
    {
        if (instance == null)
        {
            instance = this;
            //在切换场景时不要destroy这个obj
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(this);
    }

    private void InitWeaponDB()
    {
        weaponDB = new DataBase();
    }
}
