using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpHeight;
    public float Searchlen;
    public Vector3 ScreenCenter;
    public SphereCollider circlecollider;

    public float ScreenMoveSpeed;
    public Vector3 LastMousepos;//���� �ֱ��� ���콺 ��ġ

    public float MoveDegree;

    public bool nowjumping;

    //public bool NowJumping
    //{
    //    get
    //    {
    //        return nowjumping;
    //    }
    //    set
    //    {
    //        nowjumping = value;
    //    }
    //}

    public GunScript gun;

    public void FrontMove()
    {
        Vector3 temp = transform.position;
        temp.x += transform.forward.x * MoveSpeed * Time.deltaTime;
        temp.z += transform.forward.z * MoveSpeed * Time.deltaTime;
        transform.position = temp;
    }

    public void BackMove()
    {
        Vector3 temp = transform.position;
        temp.x += transform.forward.x * MoveSpeed * Time.deltaTime * -1;
        temp.z += transform.forward.z * MoveSpeed * Time.deltaTime * -1;
        transform.position = temp;
    }

    public void RightMove()
    {
        Vector3 temp = transform.position;
        temp.x += transform.right.x * MoveSpeed * Time.deltaTime;
        temp.z += transform.right.z * MoveSpeed * Time.deltaTime;
        //temp.x += MoveSpeed * Time.deltaTime;
        transform.position = temp;
    }
        

    public void LeftMove()
    {
        Vector3 temp = transform.position;
        temp.x += transform.right.x * MoveSpeed * Time.deltaTime * -1;
        temp.z += transform.right.z * MoveSpeed * Time.deltaTime * -1;
        //temp.x -= MoveSpeed * Time.deltaTime;
        transform.position = temp;
    }

    public void Jump()
    {

        Vector3 temp = transform.position;
        temp.z += MoveSpeed * Time.deltaTime;
        transform.position = temp;
    }

    public void KeyInput()
    {
        if(Input.GetKey(KeyCode.W))
        {
            if (!IsCollision(transform.position, transform.forward))
            {
                FrontMove();        
            }

        }

        if(Input.GetKey(KeyCode.A))
        {
            if (!IsCollision(transform.position, transform.right * -1))
            {
                LeftMove();
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (!IsCollision(transform.position, transform.forward * -1))
            {
                BackMove();
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (!IsCollision(transform.position, transform.right))
            {
                RightMove();
            }
        }

        if(Input.GetMouseButton(0))
        {
            gun.ShotBullet();
        }

        //
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }


    public void MouseMove()
    {
        //���� �����ӿ��� �־��� ���콺 ��ġ���� ���� ���콺 ��ġ���� ���͸� ���Ѵ�.
        Vector3 temp = Input.mousePosition - LastMousepos;
        if(transform.rotation.x+(temp.y-1)>=40|| transform.rotation.x + (temp.y - 1) <= -40)
        {
            //temp.y = transform.rotation.x * Mathf.Deg2Rad;
            //temp.y = 0;
        }
        //�� ������ �¿����, ���Ϲ����� ���������� ��ȯ
        transform.rotation = Quaternion.Euler(temp.y*-1, temp.x, 0);


        //mouse
    }


    //�浹������ true �� ������
    public bool IsCollision(Vector3 pos, Vector3 direction)
    {
        RaycastHit[] hit = Physics.SphereCastAll(pos+(direction * MoveSpeed * Time.deltaTime), circlecollider.radius, new Vector3(1, 0, 1), 0f);
        foreach(RaycastHit h in hit)
        {
            if (h.transform.tag != "Player"&& h.transform.tag != "EnemyRange")
            {
                Debug.Log($"{h.transform.tag}�� �浹!");
                return true;
            }
        }



        ////������ ��ġ���� ������ �������� ray�� ������ �浹ü�� �ִ��� �˻�
        //RaycastHit[] hit = Physics.RaycastAll(pos, direction, Searchlen);
        //foreach(RaycastHit h in hit)
        //{
        //    //�浹�� �浹ü �߿� player���� �ٸ��� ������ �浹
        //    if(h.transform.tag!="Player")
        //    {
        //        Debug.Log($"{h.transform.tag}�� �浹!");
        //        return true;
        //    }
            
        //}
        return false;
    }

    



    // Start is called before the first frame update
    void Start()
    {
        
        ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0f);
        LastMousepos = Input.mousePosition;
        gun = FindObjectOfType<GunScript>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        MouseMove();
    }
}
