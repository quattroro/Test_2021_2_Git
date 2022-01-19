using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public Animator animator;
    public bool NowAttack = false;
    public Transform Barrel;

    public float ShootInterval;
    public GameObject BulletPrefab;
    public float BulletForce;

    public void InitSetting()
    {
        animator = GetComponentInChildren<Animator>();
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            if (!NowAttack)
            {
                GameObject obj = GameObject.Instantiate(BulletPrefab);
                obj.transform.position = Barrel.transform.position;
                obj.GetComponent<Rigidbody2D>().AddForce(Barrel.right * BulletForce);
                animator.SetBool("MonsterAttack",true);
                GameObject.Destroy(obj, 1);
                yield return new WaitForSeconds(0.5f);
                animator.SetBool("MonsterAttack", false);
            }

            yield return new WaitForSeconds(ShootInterval);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSetting();
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
