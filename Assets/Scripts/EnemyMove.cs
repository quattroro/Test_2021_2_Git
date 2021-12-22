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



                yield return new WaitForSeconds(1f);
            }
            else
            {






                yield return new WaitForSeconds(2f);
            }
            
        }

        
    }




    //�÷��̾ �����ϸ� �ش� �������� ray�� ���� �÷��̾ �� �ڿ� �ִ°� �ƴ��� Ȯ���Ѵ�. . �ش� ray������ Ž���� �Ǹ� �׶� �����δ�.
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
        Debug.Log($"monster{name}������ {damage}����");

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
