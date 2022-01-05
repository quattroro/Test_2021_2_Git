using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character2DMove : MonoBehaviour
{
    

    //구현할 기능
    //1. 스페이스바를 누르는 시간에 따라서 점프 높이가 달라지도록
    //2. 몬스터한테 피격당하면 1초동안 OutOfControl 상태가 되어서 조작 붕가능 하게 되도록 그리고 입력한 방향으로 날라가도록
    //3. 올라갈 수 있는 경사보다 가파른곳에 올라가면 미끄러지도록
    //4. 2단점프, 대쉬 기능 등은 사용하지는 않지만 구현 
    
    [Serializable]
    public class Components
    {
        public Rigidbody2D rBody;
        public CapsuleCollider2D capsule;
    }

    [Serializable]
    public class MoveOption
    {
        [Range(1.0f,500.0f),Tooltip("캐릭터 속도")]
        public float Speed;
        [Range(1.0f,90.0f),Tooltip("최대로 올라갈 수 있는 경사")]
        public float MaxDegree;
        [Range(1.0f, 1000.0f), Tooltip("점프 세기")]
        public float JumpForce;

        [Range(1.0f, 1000.0f), Tooltip("중력세기")]
        public float Gravity;

        public LayerMask GroundMask;
        public LayerMask UsableMask;
        public LayerMask MoveableMask;
        public LayerMask LadderMask;

        public float DashForce;

        [Range(1, 5), Tooltip("점프 횟수")]
        public int MaxJump;

        public float GroundCheckDistanse;
        public float FowardCheckDistanse;

    }


    [Serializable]
    public class KeyOption
    {
        public KeyCode RightMove = KeyCode.D;
        public KeyCode LeftMove = KeyCode.A;
        public KeyCode UpMove = KeyCode.W;
        public KeyCode DownMove = KeyCode.S;
        public KeyCode Jump = KeyCode.Space;




    }

    [Serializable]
    public class CurrentState
    {
        public bool IsOutOfControl;
        public bool IsGrounded;
        public bool ForwardBlocked;
        public bool IsOnTheSlope;
        public bool IsOnMaxSlope;
        public bool NowJumping;
        public bool IsLeft;
        public bool OnLadder;

        public bool IsMove;

    }

    [Serializable]
    public class CurrentValues
    {
        public float GroundDegree;
        public float FowardDegree;

        public float JumpCount;

        public Vector3 InputDirection;
        public Vector3 WorldDirection;

        public Vector2 Perp;

        public float gravity;//최종 움직임의 y축 값

    }

    [Serializable]
    public class DebugVal
    {
        Vector3 groundHit;
        Vector3 fowardHit;



    }

    [SerializeField]
    public Components Com = new Components();
    [SerializeField]
    public MoveOption moveoption = new MoveOption();
    [SerializeField]
    public KeyOption keyoption = new KeyOption();
    [SerializeField]
    public CurrentValues current = new CurrentValues();
    [SerializeField]
    public CurrentState State = new CurrentState();


    public void InitSetting()
    {
        TryGetComponent(out Com.rBody);
        TryGetComponent(out Com.capsule);
        Com.rBody.freezeRotation = true;
        moveoption.GroundCheckDistanse = Com.capsule.size.y / 2 + 0.1f;
        //Physics.gravity



    }


    //항상 바닥부분의 각도를 검사해서 움직일 수 있는 각도인지를 검사한다. 움직일 수없는 경사로 올라가면 아래로 미끄러진다.
    public void CheckGround()
    {

        State.IsGrounded = false;
        State.IsOnTheSlope = false;
        //State.IsOnMaxSlope=false;
        Vector2 size = new Vector2(Com.capsule.size.x - 0.01f, Com.capsule.size.y);
        //부드러운 이동을 위해 중심과, 이동방향의 약간앞 두번의 검사를 한다.
        RaycastHit2D hit = Physics2D.BoxCast( transform.position, size, 0 , Vector2.down, moveoption.GroundCheckDistanse, moveoption.GroundMask);
        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, Com.capsule.size.x / 2, Vector2.down, moveoption.GroundCheckDistanse, moveoption.GroundMask);
        Debug.DrawLine(transform.position, hit.point, Color.red);
        if(hit)
        {
            current.GroundDegree = Vector2.Angle(hit.normal, Vector2.up);
            //State.IsOnTheSlope = current.GroundDegree > 0;
            current.Perp = Vector2.Perpendicular(hit.normal) * -1;
            if (current.GroundDegree>=moveoption.MaxDegree)
            {
                State.IsOnTheSlope = true;
            }

            State.IsGrounded = true;
        }

    }

    
    public void CheckFoward()
    {
        RaycastHit2D[] hit = new RaycastHit2D[3];
        State.ForwardBlocked = false;

        Vector3 pos1 = new Vector3(transform.position.x, (transform.position.y + Com.capsule.size.y / 2) - 0.1f, transform.position.z);
        Vector3 pos2 = new Vector3(transform.position.x, (transform.position.y - Com.capsule.size.y / 2) +  0.1f, transform.position.z);

        // hit = Physics2D.CapsuleCast(transform.position, Com.capsule.size, CapsuleDirection2D.Horizontal, 0f, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        hit[0] = Physics2D.Raycast(transform.position, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        hit[1] = Physics2D.Raycast(pos1, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        hit[2] = Physics2D.Raycast(pos2, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        Debug.DrawLine(transform.position, transform.position + (current.InputDirection * moveoption.FowardCheckDistanse), Color.blue);
        Debug.DrawLine(pos1, pos1 + (current.InputDirection * moveoption.FowardCheckDistanse), Color.blue);
        Debug.DrawLine(pos2, pos2 + (current.InputDirection * moveoption.FowardCheckDistanse), Color.blue);
        foreach(var a in hit)
        {
            if (a)
            {
                current.FowardDegree = Vector2.Angle(a.normal, Vector2.up);
                if (current.FowardDegree >= moveoption.MaxDegree)
                {
                    State.ForwardBlocked = true;
                }
                //State.IsGrounded = true;
            }
        }
        


    }

    public void KeyInput()
    {
        float v = 0f;
        float h = 0f;

        if (Input.GetKey(keyoption.RightMove))
        {
            h = 1.0f;
            State.IsLeft = false;
        }

        if (Input.GetKey(keyoption.LeftMove))
        {
            h = -1.0f;
            State.IsLeft = true;
        }

        //State.IsLeft = h < 0;
        float rotate = State.IsLeft ? 0 : 180;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, rotate, 0));

        Vector3 input = new Vector3(h, v, 0);
        current.InputDirection = Vector3.Lerp(current.InputDirection, input, 0.1f);
        State.IsMove = current.InputDirection.magnitude >= 0.01f;


        //if (Input.GetKey(keyoption.Jump))
        //위와 아래는 해당 캐릭터의 위치가 사다리 위일때만 적용
        //

        if (Input.GetKey(keyoption.UpMove))
        {
            if(IsOnLadder())
            {
                Debug.Log("사다리 감지");
            }
        }
        if (Input.GetKey(keyoption.DownMove))
        {
            if (IsOnLadder())
            {
                Debug.Log("사다리 감지");
            }
        }
    }

    //메인 카메라에서 캐릭터의 위치로 레이를 발사 
    public bool IsOnLadder()
    {
        //Vector3 direction= 
        Ray ray;
        Vector2 campos = Camera.main.WorldToScreenPoint(this.transform.position);
        ray = Camera.main.ScreenPointToRay(campos);

        //Ray ray = new Ray(this.transform.position, new Vector3(0, 0, 1));
        Debug.DrawRay(ray.origin, ray.direction);
        bool flag= Physics.Raycast(ray, 2, moveoption.LadderMask);
        Debug.Log($"{flag}");
        return flag;

    }

    //outofcontrol 상태인지(해당 상태일때는 한번 땅에 떨어지기 전까지는 계속 유지), 떨어지고 있는 상태인지(해당상태에서는 움직이는게 가능, 점프는 불가능),
    //진행방향에 벽이 있는지 내가 서있는 곳이 올라가는게 불가능한 경사로인지 등에 따라서 움직인다.
    //사다리를 오르는 중인지
    public void Move()
    {
        if (State.IsOutOfControl || State.IsOnTheSlope)
        {
            return;
        }

        if (State.ForwardBlocked /*&& !State.NowJumping*/)//앞이 막혀 있지만 점프중이 아닐때
        {
            return;
        }

        if (!State.IsMove && State.IsGrounded)//움직이지 않을때 경사로에 있을경우 미끄러지는것을 막기 위해
        {
            Com.rBody.velocity = new Vector2(0f, Com.rBody.velocity.y);
            //Com.rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //좌우이동
        if (current.GroundDegree > 0 && State.IsGrounded)
        {
            Com.rBody.velocity = current.Perp * moveoption.Speed * current.InputDirection.x;
        }
        else
        {

            Com.rBody.velocity = new Vector2(current.InputDirection.x * moveoption.Speed, Com.rBody.velocity.y);
            //Com.rBody.velocity = new Vector2(current.InputDirection.x * moveoption.Speed, current.gravity);
        }

    }

    //첫번째 점프는 땅에 붙어있을때만 가능
    public void Jump()
    {
        if(State.IsOutOfControl)
        {
            return;
        }
        
        if(Input.GetKeyDown(keyoption.Jump))
        {
            if(State.IsGrounded)
            {
                Com.rBody.AddForce(Vector2.up * moveoption.JumpForce);

                State.NowJumping = true;
                current.JumpCount++;
                return;
            }
            else
            {
                if(current.JumpCount<moveoption.MaxJump)
                {
                    Com.rBody.AddForce(Vector2.up * moveoption.JumpForce);

                    State.NowJumping = true;
                    current.JumpCount++;
                    return;
                }
            }
            
        }
        //Falling();
    }

    public void Falling()
    {
        if (State.IsGrounded || State.OnLadder)
        {
            current.gravity = 0;

        }
        if (State.IsGrounded && State.IsOutOfControl)//outofcontrol상태에서 바닥에 닿으면 해당 상태에서 벋어난다.
        {
            State.IsOutOfControl = false;
        }
        if(State.IsGrounded&&State.NowJumping)
        {
            if(Com.rBody.velocity.y<0)
            {
                State.NowJumping = false;
                current.JumpCount = 0;
            }
        }
        if (!State.IsGrounded && !State.OnLadder)
        {

            //Com.rBody.velocity = new Vector2(Com.rBody.velocity.x, Com.rBody.velocity.y-=)
            
        }

    }
    

    




    // Start is called before the first frame update
    void Start()
    {
        InitSetting();

    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();

        CheckGround();
        CheckFoward();

        Falling();
        Jump();
        
        Move();
        
        
        //Debug.DrawLine(transform.position, transform.position+current.InputDirection, Color.yellow);
    }
}
