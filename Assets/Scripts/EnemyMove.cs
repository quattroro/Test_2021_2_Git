using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour
{
    NavMeshAgent navagent;
    public SphereCollider range;
    public Vector3 targetpos;
    public Vector3 RanMove;
    public bool PlayerDetected;
    public float MaxHP;
    public float CurHP;
    public Image HPBar;
    public PlayerMove sc_player;

    enum STATE { IDLE, WALK };

    public float HP
    {
        get
        {
            return CurHP;
        }
        set
        {
            CurHP = CurHP - value;
            HPBar.transform.localScale = new Vector3(MaxHP / CurHP, 1, 1);
        }
    }


    public void IsDetect(PlayerMove player)
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

    IEnumerator attack()
    {
        while(true)
        {
            if (IsDetect())
            {

            }
            else
            {
                yield break;
            }
        }
    }


    public void Attack()
    {

    }



    //플레이어가 감지되었는지 아닌지에 따라서 다르게 움직인다.
    //1초에 한번씩 목표지점을 갱신하면서 움직인다. 한번 플레이어가 감지되면 죽을때까지 따라간다.
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
    


    //플레이어가 감지되어 있는 동안엔 1초에 한번씩 플레이어 위치로 이동하고 아닌 동안에는 8방향 중에 랜덤으로 2초에 한번씩 이동하면서 플레이어를 찾는다.
    IEnumerator NavMove()
    {
        while(true)
        {
            if(IsDetect())
            {



                yield return new WaitForSeconds(1f);
            }
            else
            {






                yield return new WaitForSeconds(2f);
            }
            
        }

        
    }




    //플레이어를 감지하면 해당 방향으로 ray를 쏴서 플레이어가 벽 뒤에 있는건 아닌지 확인한다. . 해당 ray에까지 탐지가 되면 그때 움직인다.
    public void DetectPlayer()
    {
        if(sc_player==null)
        {
            RaycastHit[] hit = Physics.SphereCastAll(range.transform.position, range.radius, new Vector3(0, 1, 0), 0);
            foreach (RaycastHit h in hit)
            {
                if (h.transform.tag == "Player")
                {

                    PlayerMove temp = h.transform.GetComponent<PlayerMove>();


                    RaycastHit hit2;
                    Vector3 dir = temp.transform.position - this.transform.position;
                    Physics.Raycast(this.transform.position, dir, out hit2);

                    if(hit2.transform.tag=="Player")
                    {
                        
                        Debug.Log("detect player");
                        IsDetect(temp);
                        //NavMove(h.transform.position);
                        targetpos = h.transform.position;
                    }
                    
                }
            }
        }
        else
        {
            navagent.SetDestination(sc_player.transform.position);
        }

        //if(IsDetect==true)
        //{
        //    IsDetect = false;
        //}
    }

    
    public void EnemyHit(int damage)
    {
        Debug.Log($"monster{name}데미지 {damage}입음");

        HP = HP - damage;
    }


    // Start is called before the first frame update
    void Start()
    {
        navagent = GetComponent<NavMeshAgent>();
        HPBar.transform.localScale = new Vector3(CurHP / MaxHP, 1, 1);
        //range = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
    }
}
