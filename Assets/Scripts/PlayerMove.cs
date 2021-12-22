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
    public Vector3 LastMousepos;//가장 최근의 마우스 위치

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
        //이전 프레임에서 있었던 마우스 위치에서 현재 마우스 위치로의 벡터를 구한다.
        Vector3 temp = Input.mousePosition - LastMousepos;
        if(transform.rotation.x+(temp.y-1)>=40|| transform.rotation.x + (temp.y - 1) <= -40)
        {
            //temp.y = transform.rotation.x * Mathf.Deg2Rad;
            //temp.y = 0;
        }
        //그 벡터의 좌우방향, 상하방향을 각도값으로 변환
        transform.rotation = Quaternion.Euler(temp.y*-1, temp.x, 0);


        //mouse
    }


    //충돌했으면 true 를 리턴함
    public bool IsCollision(Vector3 pos, Vector3 direction)
    {
        RaycastHit[] hit = Physics.SphereCastAll(pos+(direction * MoveSpeed * Time.deltaTime), circlecollider.radius, new Vector3(1, 0, 1), 0f);
        foreach(RaycastHit h in hit)
        {
            if (h.transform.tag != "Player"&& h.transform.tag != "EnemyRange")
            {
                Debug.Log($"{h.transform.tag}와 충돌!");
                return true;
            }
        }



        ////지정된 위치에서 지정된 방향으로 ray를 보내서 충돌체가 있는지 검사
        //RaycastHit[] hit = Physics.RaycastAll(pos, direction, Searchlen);
        //foreach(RaycastHit h in hit)
        //{
        //    //충돌한 충돌체 중에 player말고 다른게 있으면 충돌
        //    if(h.transform.tag!="Player")
        //    {
        //        Debug.Log($"{h.transform.tag}와 충돌!");
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
