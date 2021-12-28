using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public float ExplosiveTime;

    public SphereCollider ExplosiveRange;
    public float ExplosiveRadius;

    public int Damage = 100;

    public ParticleSystem particle;

    //private Test3dMove pl
    IEnumerator Grenade()
    {
        yield return new WaitForSeconds(ExplosiveTime);

        if(!particle.gameObject.activeSelf)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
        Explosion();
    }


    public void Explosion()
    {
        RaycastHit[] hit = Physics.SphereCastAll(this.transform.position, ExplosiveRadius, new Vector3(0, 1, 0), 0);
        foreach(var h in hit)
        {
            if(h.transform.tag=="Player"|| h.transform.tag == "Enemy")
            {
                if (h.transform.tag == "Player")
                {
                    h.transform.GetComponent<Test3dMove>().PlayerHit(Damage);
                }
                else if (h.transform.tag == "Enemy")
                {
                    h.transform.GetComponent<EnemyMove>().EnemyHit(Damage);
                }

            }
        }
        Destroy(this.gameObject, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        //particle.GetComponentInChildren<ParticleSystem>();
        StartCoroutine("Grenade");
        ExplosiveRadius = ExplosiveRange.radius;
        ExplosiveRange.gameObject.SetActive(false);
        particle.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
