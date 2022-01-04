using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour
{
    NavMeshAgent navagent;
    public SphereCollider detectrange;
    public float detectRadius;
    public Vector3 targetpos;
    public Vector3 RanMove;
    public bool PlayerDetected;
    public float MaxHP;
    public float CurHP;
    public Image HPBar;
    public Test3dMove sc_player;
    public bool NowAttacking;

    public Vector3 Movedirection;

    



    public Vector3[] direction = new Vector3[8] {
        new Vector3(1,0,0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(-1, 0, -1), };


    public GunScript enemygun;
    enum STATE { IDLE, WALK };

    public float HP
    {
        get
        {
            return CurHP;
        }
        set
        {
            CurHP = value;
            HPBar.transform.localScale = new Vector3(CurHP / MaxHP, 1, 1);
        }
    }


    public void IsDetect(Test3dMove player)
    {
        if(player==null)
        {
            PlayerDetected = false;
            sc_player = null;
        }
        else
        {

            PlayerDetected = true;
            //NavMove(h.transform.position);
            this.sc_player = player;
        }
        
    }
    public bool IsDetect()
    {
        return PlayerDetected;
    }

    //public bool IsDetect
    //{
    //    get
    //    {
    //        return PlayerDetected;
    //    }
    //    set
    //    {
    //        if(value)
    //        {
    //            PlayerDetected = value;
    //            this.transform.Find("Cube").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/Red");   

    //        }
    //        else
    //        {
    //            PlayerDetected = value;
    //            this.transform.Find("Cube").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/White");
    //        }
    //    }
    //}

    public void EnemyAttack(PlayerMove player)
    {
        //StartCoroutine
    }


    //�Ѿ� �ѹ߾� �߻�
    IEnumerator attack()
    {
        while(true)
        {
            if (IsDetect())
            {
                if(!enemygun.NowFiring)
                {
                    enemygun.NowFiring = true;
                    yield return new WaitForSeconds(0.5f);
                }
                enemygun.NowFiring = false;
                yield return new WaitForSeconds(3.0f);
            }
            else
            {
                yield break;
            }
        }
    }



    //�÷��̾ �����Ǿ����� �ƴ����� ���� �ٸ��� �����δ�.
    //1�ʿ� �ѹ��� ��ǥ������ �����ϸ鼭 �����δ�. �ѹ� �÷��̾ �����Ǹ� ���������� ���󰣴�.
    //public void NavMove(Vector3 pos)
    //{
    //    if(sc_player==null)
    //    {

    //    }
    //    else
    //    {
    //        navagent.SetDestination(pos);
    //    }

    //}
    


    //�÷��̾ �����Ǿ� �ִ� ���ȿ� 1�ʿ� �ѹ��� �÷��̾� ��ġ�� �̵��ϰ� �ƴ� ���ȿ��� 8���� �߿� �������� 2�ʿ� �ѹ��� �̵��ϸ鼭 �÷��̾ ã�´�.
    IEnumerator NavMove()
    {
        while(true)
        {
            if(IsDetect())
            {
                if(!NowAttacking)
                {
                    navagent.SetDestination(sc_player.transform.position);
                    if (!navagent.pathPending)
                    {
                        if (navagent.remainingDistance <= navagent.stoppingDistance)
                        {
                            if (!navagent.hasPath || navagent.velocity.sqrMagnitude == 0)
                            {
                                NowAttacking = true;
                                StartCoroutine("attack");
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                //������ ����
                int rnd = Random.Range(0, 8);
                Movedirection = direction[rnd];
                //������ �Ÿ�
                int Distance = Random.Range(5, 20);

                Movedirection *= Distance;

                navagent.SetDestination(this.transform.position + Movedirection);

                yield return new WaitForSeconds(2f);
            }
            
        }

        
    }




    //�÷��̾ �����ϸ� �ش� �������� ray�� ���� �÷��̾ �� �ڿ� �ִ°� �ƴ��� Ȯ���Ѵ�. . �ش� ray������ Ž���� �Ǹ� �׶� �����δ�.
    public void DetectPlayer()
    {
        if(sc_player==null)
        {
            RaycastHit[] hit = Physics.SphereCastAll(this.transform.position, detectRadius, new Vector3(0, 1, 0), 0);
            foreach (RaycastHit h in hit)
            {
                if (h.transform.tag == "Player")
                {

                    Test3dMove temp = h.transform.GetComponent<Test3dMove>();

                    RaycastHit hit2;
                    Vector3 dir = temp.transform.position - this.transform.position;
                    Physics.Raycast(this.transform.position, dir, out hit2);

                    if(hit2.transform.tag=="Player")
                    {
                        
                        Debug.Log("detect player");
                        IsDetect(temp);
                        //NavMove(h.transform.position);
                        //targetpos = h.transform.position;
                    }
                    
                }
            }
        }
        //else
        //{
        //    navagent.SetDestination(sc_player.transform.position);
        //}

        //if(IsDetect==true)
        //{
        //    IsDetect = false;
        //}
    }

    
    public void EnemyHit(Test3dMove player, int damage)
    {
        Debug.Log($"monster{name}������ {damage}����");
        IsDetect(player);
        HP = HP - damage;
        if (HP <= 0)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item"));
            obj.transform.position = this.transform.position;
            int rnd = Random.Range(0, (int)GunData.GunType.Grenade + 1);
            obj.GetComponent<ItemScript>().ItemSet((GunData.GunType)rnd);
            
            Destroy(this.gameObject);
        }

    }
    public void EnemyHit(int damage)
    {
        Debug.Log($"monster{name}������ {damage}����");
        //IsDetect(player);
        HP = HP - damage;
        if (HP <= 0)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item"));
            obj.transform.position = this.transform.position;
            int rnd = Random.Range(0, (int)GunData.GunType.Grenade);
            obj.GetComponent<ItemScript>().ItemSet((GunData.GunType)rnd);

            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        detectrange = GetComponentInChildren<SphereCollider>();
        
        detectRadius = detectrange.radius;
        detectrange.gameObject.SetActive(false);//�浹�˻翡 ���صǱ� ������ �ʿ��� ������ ��� ��Ȱ��ȭ ��Ų��.


        navagent = GetComponent<NavMeshAgent>();
        
        HPBar.transform.localScale = new Vector3(CurHP / MaxHP, 1, 1);
        StartCoroutine("NavMove");

        enemygun = GetComponentInChildren<GunScript>();
        enemygun.IsPlayer = false;
        enemygun.LoadGunData(GunData.GunType.Rifle);
        
        //range = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
    }
}
