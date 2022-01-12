using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character2DMove : MonoBehaviour
{
    

    //������ ���
    //1. �����̽��ٸ� ������ �ð��� ���� ���� ���̰� �޶�������
    //2. �������� �ǰݴ��ϸ� 1�ʵ��� OutOfControl ���°� �Ǿ ���� �ذ��� �ϰ� �ǵ��� �׸��� �Է��� �������� ���󰡵���
    //3. �ö� �� �ִ� ��纸�� ���ĸ����� �ö󰡸� �̲���������
    //4. 2������, �뽬 ��� ���� ��������� ������ ���� 
    
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
        [Range(1.0f,500.0f),Tooltip("ĳ���� �ӵ�")]
        public float Speed;
        [Range(1.0f,90.0f),Tooltip("�ִ�� �ö� �� �ִ� ���")]
        public float MaxDegree;
        [Range(1.0f, 1000.0f), Tooltip("���� ����")]
        public float JumpForce;

        [Range(1.0f, 1000.0f), Tooltip("�߷¼���")]
        public float Gravity;

        public LayerMask GroundMask;
        public LayerMask UsableMask;
        public LayerMask MoveableMask;
        public LayerMask LadderMask;

        public float DashForce;

        [Range(1, 5), Tooltip("���� Ƚ��")]
        public int MaxJump;

        [Range(0.0f,2.0f),Tooltip("���� �ִ� ��¡ (�ִ� ��¡�Ҽ� �ְ� �� �ð� )")]
        public float MaxJumpChargeSecond;
        [Range(0.0f, 90.0f), Tooltip("������ �̵��� �ּ� �Ÿ�")]
        public float MinJumpDegree;
        [Range(0.0f, 90.0f), Tooltip("������ �̵��� �ִ� �Ÿ�")]
        public float MaxJumpDegree;
        [Range(0.0f, 500.0f), Tooltip("������ �̵��� �ּ� �Ÿ�")]
        public float MinJumpForce;
        [Range(0.0f, 500.0f), Tooltip("������ �̵��� �ִ� �Ÿ�")]
        public float MaxJumpForce;

        //public float Gravity;


        //[Range(0.0f, 100.0f), Tooltip("���� �ִ� ��¡")]
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

        public float gravity;//���� �������� y�� ��

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


    //�׻� �ٴںκ��� ������ �˻��ؼ� ������ �� �ִ� ���������� �˻��Ѵ�. ������ ������ ���� �ö󰡸� �Ʒ��� �̲�������.
    public void CheckGround()
    {

        State.IsGrounded = false;
        State.IsOnTheSlope = false;
        //State.IsOnMaxSlope=false;
        Vector2 size = new Vector2(Com.capsule.size.x - 0.02f, Com.capsule.size.y - 0.42f);
        //�ε巯�� �̵��� ���� �߽ɰ�, �̵������� �ణ�� �ι��� �˻縦 �Ѵ�.
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

    //��ٸ��� �ö󰡴� ���� �ƴҶ� 
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
        //Debug.Log("�˹����");
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
                Debug.Log("��ٸ� ����");
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
                Debug.Log("��ٸ� ����");
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
        //���� �Ʒ��� �ش� ĳ������ ��ġ�� ��ٸ� ���϶��� ����
        //


    }

    //���� ī�޶󿡼� ĳ������ ��ġ�� ���̸� �߻� 
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

    //outofcontrol ��������(�ش� �����϶��� �ѹ� ���� �������� �������� ��� ����), �������� �ִ� ��������(�ش���¿����� �����̴°� ����, ������ �Ұ���),
    //������⿡ ���� �ִ��� ���� ���ִ� ���� �ö󰡴°� �Ұ����� �������� � ���� �����δ�.
    //��ٸ��� ������ ������
    public void Move()
    {
        if (State.IsOutOfControl || State.IsOnTheSlope)
        {
            return;
        }

        if (State.NowJumping && State.ForwardBlocked)
        {
            Debug.Log("����ƨ��");
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

        if (State.ForwardBlocked /*&& !State.NowJumping*/)//���� ���� ������ �������� �ƴҶ�
        {
            return;
        }

        if(!State.IsGrounded&&!State.OnLadder)
        {
            return;
        }

        if (!State.IsMove && State.IsGrounded)//�������� ������ ���ο� ������� �̲������°��� ���� ����
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


        //�¿��̵�
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


        //����
        //���� ���� ��¡���� �̿��ؼ� ���� ������ ���⸦ �˾Ƴ��� ������ ��Ų��.
        //if (State.JumpTrigger)
        //{
        //    Debug.Log("���� ����");
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

    //ù��° ������ ���� �پ��������� ����
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
            Debug.Log("���� ����");
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
        if (State.IsGrounded && State.IsOutOfControl&& Time.time>=current.KnockbackTime)//outofcontrol���¿��� �ٴڿ� ������ �ش� ���¿��� �����.
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
