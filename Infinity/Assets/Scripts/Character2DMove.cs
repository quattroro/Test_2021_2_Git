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
        public Animator animator;
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

        [Range(0.0f,2.0f),Tooltip("점프 최대 차징 (최대 차징할수 있게 할 시간 )")]
        public float MaxJumpChargeSecond;
        [Range(0.0f, 90.0f), Tooltip("점프로 이동할 최소 거리")]
        public float MinJumpDegree;
        [Range(0.0f, 90.0f), Tooltip("점프로 이동할 최대 거리")]
        public float MaxJumpDegree;
        [Range(0.0f, 500.0f), Tooltip("점프로 이동할 최소 거리")]
        public float MinJumpForce;
        [Range(0.0f, 500.0f), Tooltip("점프로 이동할 최대 거리")]
        public float MaxJumpForce;

        //public float Gravity;


        //[Range(0.0f, 100.0f), Tooltip("점프 최대 차징")]
        //public float ChargingSpeed;



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

        public bool IsCharging;

        public bool JumpTrigger;
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

        public float ChargingStartTime;
        public float ChargingEndTime;
        public float CurJumpCharging;

        public float CurJumpDegree;

        public float KnockbackTime;
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

    bool flag = false;
    public void InitSetting()
    {
        TryGetComponent(out Com.rBody);
        TryGetComponent(out Com.capsule);
        Com.animator = GetComponentInChildren<Animator>();

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
        Vector2 size = new Vector2(Com.capsule.size.x - 0.02f, Com.capsule.size.y - 0.42f);
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

    //사다리를 올라가는 중이 아닐때 
    public void LadderCheck()
    {
        if(!State.OnLadder)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, moveoption.GroundCheckDistanse, moveoption.LadderMask);

            if (hit)
            {
                State.IsGrounded = true;
            }
        }
    }

    public void KnockBack(Vector2 force, float time)
    {
        //Debug.Log("넉백들어옴");
        current.KnockbackTime = Time.time + time;
        State.IsOutOfControl = true;

        Com.animator.SetBool("AniOutOfControl", true);
        Com.rBody.AddForce(force);

    }

    public void CheckFoward()
    {
        RaycastHit2D[] hit = new RaycastHit2D[3];
        State.ForwardBlocked = false;

        Vector3 pos1 = new Vector3(transform.position.x, (transform.position.y + Com.capsule.size.y / 2) - 0.1f, transform.position.z);
        Vector3 pos2 = new Vector3(transform.position.x, (transform.position.y - Com.capsule.size.y / 2) +  0.1f, transform.position.z);

        // hit = Physics2D.CapsuleCast(transform.position, Com.capsule.size, CapsuleDirection2D.Horizontal, 0f, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        Vector2 direction = (State.IsLeft) ? new Vector2(-1, 0) : new Vector2(1, 0);

        //hit[0] = Physics2D.Raycast(transform.position, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        //hit[1] = Physics2D.Raycast(pos1, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        //hit[2] = Physics2D.Raycast(pos2, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);

        hit[0] = Physics2D.Raycast(transform.position, direction, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        hit[1] = Physics2D.Raycast(pos1, direction, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        hit[2] = Physics2D.Raycast(pos2, direction, moveoption.FowardCheckDistanse, moveoption.GroundMask);

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
        if(!IsOnLadder())
        {
            State.OnLadder = false;
            Com.animator.SetBool("AniOnLadder", false);
        }

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

        if (Input.GetKey(keyoption.UpMove))
        {
            if (IsOnLadder())
            {
                State.OnLadder = true;
                Com.animator.SetBool("AniOnLadder", true);
                Debug.Log("사다리 감지");
                v = 1.0f;
                //current.InputDirection = Vector3.Lerp(current.InputDirection, input, 0.1f);
            }

        }
        if (Input.GetKey(keyoption.DownMove))
        {
            if (IsOnLadder())
            {
                State.OnLadder = true;
                Com.animator.SetBool("AniOnLadder", true);
                Debug.Log("사다리 감지");
                v = -1.0f;
            }
        }

        //State.IsLeft = h < 0;
        float rotate = State.IsLeft ? 0 : 180;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, rotate, 0));

        Vector3 input = new Vector3(h, v, 0);
        current.InputDirection = Vector3.Lerp(current.InputDirection, input, 0.1f);
        State.IsMove = current.InputDirection.magnitude >= 0.01f;
        bool b = State.IsMove ? true : false;
        Com.animator.SetBool("AniRun", b);

        //if (Input.GetKey(keyoption.Jump))
        //위와 아래는 해당 캐릭터의 위치가 사다리 위일때만 적용
        //


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
        bool flag = Physics2D.Raycast(ray.origin, ray.direction, 2, moveoption.LadderMask);
        //Debug.Log($"{flag}");
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

        if (State.NowJumping && State.ForwardBlocked)
        {
            Debug.Log("점프튕김");
            if(!flag)
            {
                Com.rBody.velocity = new Vector2(Com.rBody.velocity.x * -1, Com.rBody.velocity.y);
                flag = true;
            }
            
        }

        if (State.NowJumping)
        {
            return;
        }

        if (State.ForwardBlocked /*&& !State.NowJumping*/)//앞이 막혀 있지만 점프중이 아닐때
        {
            return;
        }

        if(!State.IsGrounded&&!State.OnLadder)
        {
            return;
        }

        if (!State.IsMove && State.IsGrounded)//움직이지 않을때 경사로에 있을경우 미끄러지는것을 막기 위해
        {
            Com.rBody.velocity = new Vector2(0f, Com.rBody.velocity.y);
            return;
            //Com.rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        
        if (!State.IsMove && State.OnLadder)
        {
            //Com.rBody.velocity = new Vector2(0f, 0f);
            Com.rBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            Com.rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (!State.IsMove && !State.OnLadder)
        {
            return;
        }


        //좌우이동
        if (current.GroundDegree > 0 && State.IsGrounded)
        {
            //Com.rBody.velocity = current.Perp * moveoption.Speed * current.InputDirection.x;
            current.WorldDirection = current.Perp * moveoption.Speed * current.InputDirection.x;
            Com.rBody.velocity = current.WorldDirection;
        }
        else
        {

            //Com.rBody.velocity = new Vector2(current.InputDirection.x * moveoption.Speed, Com.rBody.velocity.y);
            //current.WorldDirection = new Vector2(current.InputDirection.x * moveoption.Speed, Com.rBody.velocity.y);
            if (State.OnLadder)
            {
                current.WorldDirection = new Vector2(current.InputDirection.x * moveoption.Speed, current.InputDirection.y * moveoption.Speed);
                Com.rBody.velocity = current.WorldDirection;
            }
            else
            {
                current.WorldDirection = new Vector2(current.InputDirection.x * moveoption.Speed, Com.rBody.velocity.y);
                Com.rBody.velocity = current.WorldDirection;
            }
            


            //Com.rBody.velocity = new Vector2(current.InputDirection.x * moveoption.Speed, current.gravity);
        }


        //점프
        //현재 점프 차징값을 이용해서 점프 각도와 세기를 알아내서 점프를 시킨다.
        //if (State.JumpTrigger)
        //{
        //    Debug.Log("점프 시작");
        //    State.JumpTrigger = false;
        //    State.NowJumping = true;
        //    float jumpdegree = moveoption.MinJumpDegree + ((moveoption.MaxJumpDegree - moveoption.MinJumpDegree) * current.CurJumpCharging);
        //    float jumpforce = moveoption.MinJumpForce + ((moveoption.MaxJumpForce - moveoption.MinJumpForce) * current.CurJumpCharging);

        //    current.CurJumpDegree = jumpdegree;
        //    float val = (State.IsLeft) ? -1 : 1;
        //    Vector2 jumpdierction = new Vector2(Mathf.Cos(jumpdegree*Mathf.Deg2Rad ) * val, Mathf.Sin(jumpdegree * Mathf.Deg2Rad));

        //    Com.rBody.AddForce(jumpdierction * jumpforce);

        //    //current.WorldDirection



        //}

    }

    //첫번째 점프는 땅에 붙어있을때만 가능
    public void Jump()
    {
        if(State.IsOutOfControl||State.NowJumping||State.OnLadder)
        {
            return;
        }
        
        if(Input.GetKeyDown(keyoption.Jump))
        {
            if(!State.IsCharging)
            {

                State.IsCharging = true;
                current.ChargingStartTime = Time.time;
            }
            //if(State.IsCharging)
            //{
                


            //}

            //if(State.IsGrounded)
            //{
            //    Com.rBody.AddForce(Vector2.up * moveoption.JumpForce);

            //    State.NowJumping = true;
            //    current.JumpCount++;
            //    return;
            //}
            //else
            //{
            //    if(current.JumpCount<moveoption.MaxJump)
            //    {
            //        Com.rBody.AddForce(Vector2.up * moveoption.JumpForce);

            //        State.NowJumping = true;
            //        current.JumpCount++;
            //        return;
            //    }
            //}
            
        }
        if(Input.GetKeyUp(keyoption.Jump))
        {
            if(State.IsCharging)
            {
                State.IsCharging = false;
                current.ChargingEndTime = Time.time;
                float time = current.ChargingEndTime - current.ChargingStartTime;
                //current.CurJumpCharging = Mathf.Floor(time * 100) * 0.01f;
                time = Mathf.Floor(time * 100) * 0.01f;
                current.CurJumpCharging = time / moveoption.MaxJumpChargeSecond;
                if(current.CurJumpCharging >= 1.0f)
                {
                    current.CurJumpCharging = 1.0f;
                }

                State.JumpTrigger = true;

                Debug.Log($"{time}");
                Debug.Log($"{current.CurJumpCharging}");
            }
        }


        if (State.JumpTrigger)
        {
            Debug.Log("점프 시작");
            State.JumpTrigger = false;
            State.NowJumping = true;
            Com.animator.SetBool("AniJump", true);
            float jumpdegree = moveoption.MinJumpDegree + ((moveoption.MaxJumpDegree - moveoption.MinJumpDegree) * current.CurJumpCharging);
            float jumpforce = moveoption.MinJumpForce + ((moveoption.MaxJumpForce - moveoption.MinJumpForce) * current.CurJumpCharging);

            current.CurJumpDegree = jumpdegree;
            float val = (State.IsLeft) ? -1 : 1;
            Vector2 jumpdierction = new Vector2(Mathf.Cos(jumpdegree * Mathf.Deg2Rad) * val, Mathf.Sin(jumpdegree * Mathf.Deg2Rad));

            Com.rBody.AddForce(jumpdierction * jumpforce);

            //current.WorldDirection



        }
        //Falling();
    }

    public void Falling()
    {
        if (State.IsGrounded || State.OnLadder)
        {
            current.gravity = 0;

        }
        if (State.IsGrounded && State.IsOutOfControl&& Time.time>=current.KnockbackTime)//outofcontrol상태에서 바닥에 닿으면 해당 상태에서 벋어난다.
        {
            State.IsOutOfControl = false;
            Com.animator.SetBool("AniOutOfControl", false);
        }
        if(State.IsGrounded&&State.NowJumping)
        {
            if(Com.rBody.velocity.y<0)
            {
                State.NowJumping = false;
                Com.animator.SetBool("AniJump", false);
                flag = false;
                current.JumpCount = 0;
            }
        }
        if (!State.IsGrounded && !State.OnLadder)
        {

            //Com.rBody.velocity = new Vector2(Com.rBody.velocity.x, Com.rBody.velocity.y-=)
            //Physics.gravity.y += 0.02f * moveoption.Gravity;


            
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
        LadderCheck();
        Falling();
        Jump();
        
        Move();
        
        
        //Debug.DrawLine(transform.position, transform.position+current.InputDirection, Color.yellow);
    }
}
