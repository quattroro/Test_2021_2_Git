using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : BaseGunWeapon
{
    public Transform Barrel;

    [Tooltip("스크립터블오브젝트 연결 필요")]
    public GunData gundata;

    //public float BulletForce;
    //public float CoolTime;
    
    private float LastShootTime;
    public Transform barrel;
    public GameObject Bullet;
    public BaseGunWeapon weaponroot;
    public int Damage;
    public PlayerMove sc_Player;


    public void InitSetting()
    {

    }
    public void ShotBullet()
    {
        if(Input.GetMouseButton(0))
        {
            if (Time.time - LastShootTime >= gundata.CoolDown)
            {
                LastShootTime = Time.time;
                GameObject obj = GameObject.Instantiate(Bullet);
                //obj.GetComponent<BulletScript>().Weapon = this;
                obj.SetActive(true);
                obj.transform.position = barrel.position;
                //obj.GetComponent<Rigidbody>().AddForce(barrel.forward * BulletForce);
            }
        }
    }

    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
