using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGunWeapon : MonoBehaviour
{
    public float BulletForce;
    public float CoolTime;
    protected float LastShootTime;
    public Transform barrel;
    public GameObject Bullet;
    public BaseGunWeapon weaponroot;
    public int Damage;
    public PlayerMove sc_Player;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
