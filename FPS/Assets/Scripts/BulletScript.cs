using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    //public BaseGunWeapon Weapon;//�ڽ��� �߻��� ���� ��ũ��Ʈ



    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Bullet" && other.tag != "Player"&&other.tag != "EnemyRange") 
        {
            if(other.tag=="Enemy")
            {
                //other.GetComponent<EnemyMove>().EnemyHit(Weapon.Damage); 
            }
            Destroy(this.gameObject);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
