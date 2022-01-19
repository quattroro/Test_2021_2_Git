using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public bool walk;
    public bool walk_left;
    public bool walk_right;
    public bool jump;
    public bool down;
    public bool attack;
    public bool isground = true;
    public float jumppower;
    public float dash_power;
    public Texture2D cursorTexture;
    public ParticleSystem dashparticle;
    public ParticleSystemRenderer dash;
    public GameObject Weapon;
    //�ؽ�ó�� ����κ��� ���콺�� ��ǥ�� �� ������ �ؽ�ó��
    //���ο��� ����� �ʵ带 �����մϴ�.
    [SerializeField]
    private Vector2 hotSpot;
    [SerializeField]
    private Vector3 MousePosition; //���콺��ǥ 
    public LayerMask floorMask; //�����̾��ũ 
    public LayerMask BottomMask;
    private float jumpcount = 2f;
    private Rigidbody2D rigidbody=null;
    public BoxCollider2D boxcollider;
    //��������ö������ ������ 
    public float angle;
    public Vector2 perp;
    public bool isSlope;
    private float InputX;

    private enum PlayerState
    {
        e_idle,
        e_walk,
        e_jump
    }
    private PlayerState playerstate;
    // Start is called before the first frame update
    void Start()
    {
       
        hotSpot.x = cursorTexture.width / 2;
        hotSpot.y = cursorTexture.height / 2;
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        rigidbody = GetComponent<Rigidbody2D>();
        dashparticle.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        isGroundCheck();

        Attack();

        PlayerDash();
        
        CheckState();

        CheckAnimation();

        Move();

        HillRoad();
    }
    void isGroundCheck()
    {
        Vector2 Down = transform.position;             
        if (Physics2D.Raycast(Down, -transform.up, 0.5f, BottomMask))          
        {
            isground = true;          
        }
        else
        {
            isground = false;
        }
    }
    void HillRoad()
    {
        InputX = Input.GetAxis("Horizontal"); 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, floorMask);
        if (hit)
        {
            perp = Vector2.Perpendicular(hit.normal).normalized;//���Ͱ��� 90�������ִ°� 
            angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle != 0)
            {
                isSlope = true;
            }
            else
            {
                isSlope = false;
            }
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
        }
        else
        {
            angle = 0;
            isSlope = false;
        }
    }
    protected override void Attack()
    {
        Weapon.GetComponent<Weapon>().Attack();
    }

    protected override void Damaged()
    {
        throw new System.NotImplementedException();
    }

    protected override void Move()
    {
        Speed = 3f; //�ӵ�. 
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;
        Vector3 pos1;
        if (MousePosition.x>transform.position.x)
        {
            pos1 = new Vector3(0, 0, 0);
            scale.x = 4;
            dash.flip = pos1;
        }
        else
        {
            pos1 = new Vector3(1, 0, 0);
            scale.x = -4;           
            dash.flip = pos1;
        }
        if (walk)
        {
            //if (isSlope && isground) //�¿�. ����ƴҰ��
            //{
            //    rigidbody.velocity = perp * InputX*Speed * -1f;
            //}
            if(!isSlope && isground) //����ϰ��. 
            {               
                rigidbody.velocity = new Vector2(InputX * Speed, 0);
            }
            else
            {
                rigidbody.velocity = new Vector2(InputX * Speed, rigidbody.velocity.y);
            }
            if (playerstate != PlayerState.e_jump)
            {
                playerstate = PlayerState.e_walk;
            }
            if (playerstate==PlayerState.e_jump)
            {
                RayCastRigtLeft();
                Debug.Log("zz");
            }
            if (walk_left) //�������� �����̴°�
            {
                pos.x -= Speed * Time.deltaTime;
            }
            if (walk_right) //���������� �����̴°�.
            {
                pos.x += Speed * Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetButton("Vertical"))    //�ϴ����� 
        {                   
            f_Downjump();
            //CheckDownLays(pos);           
        }
        else if (jump && jumpcount>0) //���� 
        {
            f_Jump();
        }
        if(!walk&&playerstate!=PlayerState.e_jump)
        {
            playerstate = PlayerState.e_idle;
        }
        transform.position = pos;
        transform.localScale = scale;
    }
    void RayCastRigtLeft()
    {
        Vector2 CharRight = transform.position;
        Vector2 CharLeft = transform.position;
        CharRight.y += 0.5f;
        CharLeft.y += 0.5f;
        if (Physics2D.Raycast(CharRight, transform.right, 1f, floorMask) ||
             Physics2D.Raycast(CharLeft, -transform.right, 1f, floorMask))
        {
            f_Downjump();
        }
    }
    void f_Jump()
    {
        Vector2 Left = transform.position;
        Vector2 right = transform.position;
        Vector2 middle = transform.position;

        Left.x = transform.position.x - 0.4f;
        right.x = transform.position.x + 0.4f;
        Debug.DrawRay(right, Vector2.up * 2.5f, Color.blue, 0.3f);
        Debug.DrawRay(middle, Vector2.up * 2.5f, Color.blue, 0.3f);
        Debug.DrawRay(Left, Vector2.up * 2.5f, Color.blue, 0.3f);

        if(Physics2D.Raycast(Left, transform.up,2.5f,floorMask) ||
            Physics2D.Raycast(right, transform.up, 2.5f, floorMask) ||
            Physics2D.Raycast(middle, transform.up, 2.5f, floorMask) )         
        {
            f_Downjump();
        }
        playerstate = PlayerState.e_jump;
        isground = false;
        this.rigidbody.AddForce(transform.up * jumppower);
        rigidbody.velocity = Vector2.zero;
        jumpcount--;
    }
    void f_Downjump()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("wall"),true);
        StartCoroutine("IgnoreLaymask");
    }
    IEnumerator IgnoreLaymask()
    {
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("wall"), false);
    }
    void CheckState()
    {
        bool input_left = Input.GetKey(KeyCode.A);
        bool input_right = Input.GetKey(KeyCode.D);
        bool input_space = Input.GetKeyDown(KeyCode.Space);
        bool input_down = Input.GetKeyDown(KeyCode.S);
       
        walk = input_left || input_right;
        walk_left = input_left && !input_right;
        walk_right = !input_left && input_right;
        jump = input_space;
        down = input_down;
    }

    void CheckAnimation()
    {
        if(playerstate==PlayerState.e_walk)
        {
            GetComponent<Animator>().SetBool("Normal_Jump", false);
            GetComponent<Animator>().SetBool("Normal_Run", true);         
        }
        if (playerstate == PlayerState.e_jump)
        {
            GetComponent<Animator>().SetBool("Normal_Jump", true);
            GetComponent<Animator>().SetBool("Normal_Run", false);           
        }
        if(playerstate==PlayerState.e_idle)
        {
            GetComponent<Animator>().SetBool("Normal_Run", false);
            GetComponent<Animator>().SetBool("Normal_Jump", false);
        }
    }
    void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            f_Downjump();
            dashparticle.Stop();
            dashparticle.Play();
            playerstate = PlayerState.e_jump;
            StartCoroutine("f_dashcourtine");
            isground = false;
            rigidbody.velocity = Vector2.zero;
            if (transform.position.y>MousePosition.y-2)
            {
                MousePosition.y = transform.position.y + 2;
            }
            if (transform.position.x > MousePosition.x)
            {
                MousePosition.x = transform.position.x - 2;
            }
            else
            {
                MousePosition.x = transform.position.x + 2;
            }
            Vector2 direction = (MousePosition - transform.position).normalized; // direction�� �÷��̾� ��ǥ���� ���콺 ��ǥ���� ������ �������ͷ� �����
            rigidbody.AddForce(direction * dash_power * 1f); // direction�������� power��ŭ�� ���� ����, �뽬�� ������, Acceleration(���ӵ�)          
        }
    }
    IEnumerator f_dashcourtine()
    {      
        yield return new WaitForSeconds(0.3f);       
        dashparticle.Stop();
    }
 
    private void OnCollisionEnter2D(Collision2D collision)
    {    
        jumpcount = 2;
        playerstate = PlayerState.e_idle;
    }  
}
