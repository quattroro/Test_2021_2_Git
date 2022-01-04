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
        [Range(1.0f, 1000.0f), Tooltip("���� ����")]
        public float GravityAccelate;

        public LayerMask GroundMask;
        public LayerMask UsableMask;
        public LayerMask MoveableMask;

        public float DashForce;

        [Range(1, 5), Tooltip("���� Ƚ��")]
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


    //�׻� �ٴںκ��� ������ �˻��ؼ� ������ �� �ִ� ���������� �˻��Ѵ�. ������ ������ ���� �ö󰡸� �Ʒ��� �̲�������.
    public void CheckGround()
    {

        State.IsGrounded = false;
        State.IsOnTheSlope = false;
        //State.IsOnMaxSlope=false;
        //�ε巯�� �̵��� ���� �߽ɰ�, �̵������� �ణ�� �ι��� �˻縦 �Ѵ�.

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, Com.capsule.size.x / 2, Vector2.down, moveoption.GroundCheckDistanse, moveoption.GroundMask);
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
        State.ForwardBlocked = false;

       
        //RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, Com.capsule.size, CapsuleDirection2D.Horizontal, 90f, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, current.InputDirection, moveoption.FowardCheckDistanse, moveoption.GroundMask);
        Debug.DrawLine(transform.position, transform.position + (current.InputDirection * moveoption.FowardCheckDistanse), Color.blue);
        if (hit)
        {
            current.FowardDegree = Vector2.Angle(hit.normal, Vector2.up);
            if (current.FowardDegree >= moveoption.MaxDegree)
            {
                State.ForwardBlocked = true;
            }
            //State.IsGrounded = true;
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
        //���� �Ʒ��� �ش� ĳ������ ��ġ�� ��ٸ� ���϶��� ����
        //
        if (Input.GetKey(keyoption.UpMove))
        {

        }
        if (Input.GetKey(keyoption.DownMove))
        {

        }
        
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

        if (State.ForwardBlocked && !State.NowJumping)//���� ���� ������ �������� �ƴҶ�
        {
            return;
        }

        if (!State.IsMove && State.IsGrounded)//�������� ������ ���ο� ������� �̲������°��� ���� ����
        {
            Com.rBody.velocity = new Vector2(0f, Com.rBody.velocity.y);
            //Com.rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //����


        //�¿��̵�
        if (current.GroundDegree > 0 && State.IsGrounded)
        {
            Com.rBody.velocity = current.Perp * moveoption.Speed * current.InputDirection.x;
        }
        else
        {
            Com.rBody.velocity = new Vector2(current.InputDirection.x * moveoption.Speed, Com.rBody.velocity.y);
        }

    }

    //ù��° ������ ���� �پ��������� ����
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
        if (State.IsGrounded && State.IsOutOfControl)//outofcontrol���¿��� �ٴڿ� ������ �ش� ���¿��� �����.
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
