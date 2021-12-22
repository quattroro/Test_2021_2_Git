using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : BaseGunWeapon
{
    

    public void ShotBullet()
    {
        if (Time.time - LastShootTime >= CoolTime)
        {
            LastShootTime = Time.time;
            GameObject obj = GameObject.Instantiate(Bullet);
            obj.GetComponent<BulletScript>().Weapon = this;
            obj.SetActive(true);
            obj.transform.position = barrel.position;
            obj.GetComponent<Rigidbody>().AddForce(barrel.forward * BulletForce);
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
