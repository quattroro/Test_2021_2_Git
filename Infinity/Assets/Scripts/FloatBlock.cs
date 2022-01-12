using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBlock : MonoBehaviour
{
    public enum FloatDirection { Horizental, Vertical};
    public float MoveSpeed;
    public Transform startpoint;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public Transform endpoint;
    public FloatDirection type;

    public LayerMask FloatBlockMask;

    public int size;

    public Vector3 target;
    public float StopSecond;

    public bool PlayerOn;
    public LayerMask PlayerLayer;
    public BoxCollider2D boxcol;
    public float checkdistance;
    public Transform player;

    public void CheckPlayerOn()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y) + boxcol.offset;
        RaycastHit2D hit = Physics2D.BoxCast(pos, boxcol.size, 0f, Vector2.up, checkdistance, PlayerLayer);
        if(hit)
        {
            PlayerOn = true;
            player = hit.transform;
        }
        else
        {
            PlayerOn = false;
            player = null;
        }
    }



    //������ �Ǹ� ��������� ����������, ���������� �������� �˻縦 �ؼ� �ش� ���⿡ ���� �����̴� ���� ������ �ش� ���� �ڽ����� 
    //���ʰ� �������� �˻��Ѵ�.
    //���ʿ� ���� ���� �����ϸ� �˻����� ����(������ �������˻縦 �Ҳ��� ������)
    //�����ؼ� �Բ� ������ �� �ֵ��� �Ѵ�.
    void InitSetting()
    {

        float posx;
        float posy;
        RaycastHit2D hit;
        Vector2 temp = Vector2.zero;
        startpoint = this.transform.Find("StartPos");
        endpoint = this.transform.Find("EndPos");
        StartPos = startpoint.position;
        EndPos = endpoint.position;
        boxcol = GetComponent<BoxCollider2D>();


        target = EndPos;
        //direction = EndPos - StartPos;

        if (type == FloatDirection.Horizental)
        {
            //temp = new Vector2(transform.position.x + 1, transform.position.y);
           // posx = transform.position.x - 1;
            //posy = transform.position.y;
            temp = new Vector2(transform.position.x - 1, transform.position.y);
        }
        else
        {
            //temp = new Vector2(transform.position.x, transform.position.y - 1);
            //posx = transform.position.x;
            //posy = transform.position.y - 1;
            temp = new Vector2(transform.position.x, transform.position.y - 1);
        }

        Vector2 pos = Camera.main.WorldToScreenPoint(temp);
        Ray ray = Camera.main.ScreenPointToRay(pos);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 2, FloatBlockMask);
        if(hit)
        {
            //Debug.Log($"{name} �˻����");
            this.enabled = false;
            return;
        }


        posx = transform.position.x;
        posy = transform.position.y;

        while (true)
        {
            if (type == FloatDirection.Horizental) posx += 1;
            else if (type == FloatDirection.Vertical) posy += 1;

            temp = new Vector2(posx, posy);
            pos = Camera.main.WorldToScreenPoint(temp);
            ray = Camera.main.ScreenPointToRay(pos);
            hit = Physics2D.Raycast(ray.origin, ray.direction, 2, FloatBlockMask);
            if (hit)
            {
                //Debug.Log($"{hit.transform.name} ������");
                //size++;
                hit.transform.parent = this.transform;
                hit.transform.GetComponent<FloatBlock>().enabled = false;
            }
            else
            {
                return;
            }
        }

        

        //this.direction.Normalize();

        //if (type == FloatDirection.Horizental)
        //{
        //    temp = new Vector2(transform.position.x + 1, transform.position.y);
        //}
        //else
        //{
        //    temp = new Vector2(transform.position.x, transform.position.y + 1);
        //}

        //Vector2 pos = Camera.main.WorldToScreenPoint(temp);
        //Ray ray = Camera.main.ScreenPointToRay(pos);
        //hit = Physics2D.Raycast(ray.origin, ray.direction, 2, FloatBlockMask);
        //hit.transform.parent = this.transform;

        
    }


    public bool Move()
    {
        //this.transform.position = Vector3.Lerp(this.transform.position, target, 0.02f*0.02f);

        Vector3 temp = target - this.transform.position;

        this.transform.position += temp * MoveSpeed * Time.deltaTime;
        if(PlayerOn)
        {
            player.position += temp * MoveSpeed * Time.deltaTime;
        }

        Vector3 direction = target - transform.position;
        if (direction.x >= 0)
        {
            //temp = temp + new Vector3(size, 0, 0);
            if (temp.sqrMagnitude <= 0.1)
            {
                return true;
            }
        }
        else
        {
            if (temp.sqrMagnitude <= 0.1)
            {
                return true;
            }
        }

        
        return false;
    }

    public bool HorizentalMove()
    {


        return false;
    }

    

    IEnumerator CoroutineMove()
    {
        while (true)
        {
            if(Move())
            {
                yield return new WaitForSeconds(StopSecond);
                target = (target == EndPos) ? StartPos : EndPos;
            }
            yield return Time.deltaTime;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        InitSetting();
        StartCoroutine(CoroutineMove());
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerOn();
    }
}
