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

        [Range(1.0f, 1000.0f), Tooltip("�߷¼���")]
        public float Gravity;

        public LayerMask GroundMask;
        public LayerMask UsableMask;
        public LayerMask MoveableMask;
        public LayerMask LadderMask;

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

        public float gravity;//���� �������� y�� ��

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
        Vector2 size = new Vector2(Com.capsule.size.x - 0.01f, Com.capsule.size.y);
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
        //���� �Ʒ��� �ش� ĳ������ ��ġ�� ��ٸ� ���϶��� ����
        //

        if (Input.GetKey(keyoption.UpMove))
        {
            if(IsOnLadder())
            {
                Debug.Log("��ٸ� ����");
            }
        }
        if (Input.GetKey(keyoption.DownMove))
        {
            if (IsOnLadder())
            {
                Debug.Log("��ٸ� ����");
            }
        }
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
        bool flag= Physics.Raycast(ray, 2, moveoption.LadderMask);
        Debug.Log($"{flag}");
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

        if (State.ForwardBlocked /*&& !State.NowJumping*/)//���� ���� ������ �������� �ƴҶ�
        {
            return;
        }

        if (!State.IsMove && State.IsGrounded)//�������� ������ ���ο� ������� �̲������°��� ���� ����
        {
            Com.rBody.velocity = new Vector2(0f, Com.rBody.velocity.y);
            //Com.rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //�¿��̵�
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
        if (State.IsGrounded || State.OnLadder)
        {
            current.gravity = 0;

        }
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
