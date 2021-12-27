using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class Test3dMove : MonoBehaviour
{
    #region 변수들
    public enum CameraType { FPCamera, TPCamera };

    [Serializable]
    public class Components
    {
        public Camera tpCamera;
        public Camera fpCamera;

        [HideInInspector] public Transform tpRig;
        [HideInInspector] public Transform fpRoot;
        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject tpCamObject;
        [HideInInspector] public GameObject fpCamObject;

        [HideInInspector] public Rigidbody rBody;
        [HideInInspector] public Animator anim;

        public Animator animator;


    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("이동속도")]
        public float speed = 3f;
        [Range(1f, 3f), Tooltip("달리기 이동속도 증가 계수")]
        public float runningCoef = 1.5f;
        [Range(1f, 10f), Tooltip("점프강도")]
        public float jumpForce = 5.5f;
        [Range(1f, 10f), Tooltip("가속 감속")]
        public float acceleration = 1.5f;
        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;
    }

    [Serializable]
    public class CameraOption
    {
        [Tooltip("게임 시작 시 카메라")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("이동속도")]
        public float RotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("이동속도")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("이동속도")]
        public float lookDownDegree = 75f;
    }

    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveY = "Move Y";
        public string ParamMoveZ = "Move Z";
    }

    [Serializable]
    public class CharacterState
    {
        public bool isCurrentFp;
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isCursorActive;
    }

    [Serializable]
    public class KeyOption
    {
        public KeyCode moveFoward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode run = KeyCode.LeftShift;
        public KeyCode junp = KeyCode.Space;
        public KeyCode switchCamera = KeyCode.Tab;
        public KeyCode showCursor = KeyCode.LeftAlt;


    }

    //[Serializable]
    //public class CurrentValue
    //{
    //    public Vector3 worldMoveDir;
    //    public Vector3 groundNormal;
    //    public Vector3 groundCross;
    //    public Vector3 horizontalVelocity;

    //    [Space]
    //    public float jumpCooldown;
    //    public int jumpCount;
    //    public float outOfControllDuration;

    //    [Space]
    //    public float groundDistance;
    //    public float groundSlopeAngle;         // 현재 바닥의 경사각
    //    public float groundVerticalSlopeAngle; // 수직으로 재측정한 경사각
    //    public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각
    //    public float slopeAccel;        // 경사로 인한 가속/감속 비율

    //    [Space]
    //    public float gravity;
    //}


    #endregion
    //[SerializeField] private CurrentValue _currentValues = new CurrentValue();


    public Components Com => _components;
    public KeyOption Key => _keyOption;
    public MovementOption MoveOption => _movementOption;

    public CameraOption CamOption => _cameraOption;
    public AnimatorOption AnimOption => _animatorOption;
    public CharacterState State => _state;

    //public CurrentValue Current => _currentValues;

    [SerializeField]
    private Components _components = new Components();
    [SerializeField]
    private KeyOption _keyOption = new KeyOption();
    [SerializeField]
    private MovementOption _movementOption = new MovementOption();
    [SerializeField]
    private CameraOption _cameraOption = new CameraOption();
    [SerializeField]
    private AnimatorOption _animatorOption = new AnimatorOption();
    [SerializeField]
    private CharacterState _state = new CharacterState();
    



    private Vector3 _moveDir;

    private Vector3 _worldMove;

    private Vector3 _rotation;


    //private float _distFromGround;

    public float _groundCheckRadius;

    public CapsuleCollider capsule;

    public LayerMask groundLayerMask;

    public bool isOnSteepSlope;

    public float maxSlopeAngle;

    private void Awake()
    {
        InitComponents();
        InitSettings();
    }


    private void InitComponents()
    {
        LogNotInitializedComponentError(Com.tpCamera, "TP Camera");
        LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
        TryGetComponent(out Com.rBody);
        TryGetComponent(out capsule);
        //Com.animator = GetComponent<Animator>();
        Com.anim = GetComponentInChildren<Animator>();

        Com.tpCamObject = Com.tpCamera.gameObject;
        Com.tpRig = Com.tpCamera.transform.parent;

        Com.fpCamObject = Com.fpCamera.gameObject;
        Com.fpRig = Com.fpCamera.transform.parent;
        Com.fpRoot = Com.fpRig.parent;
    }




    private void InitSettings()
    {
        if (Com.rBody)
        {
            //회전은 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        var allcams = FindObjectsOfType<Camera>();
        foreach (var cam in allcams)
        {
            cam.gameObject.SetActive(false);
        }
        //설정한 카메라 하나만 활성화 
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FPCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);
        Com.tpCamObject.SetActive(!State.isCurrentFp);


        //CapsuleCollider cCol = GetComponent<CapsuleCollider>();
        TryGetComponent(out CapsuleCollider cCol);
        _groundCheckRadius = cCol ? cCol.height / 2 : 0.1f;
        //Debug.Log($"{cCol.height}");
    }

    public float groundDistance;
    public Vector3 groundNormal;
    public float groundSlopeAngle;
    public float forwardObstacleAngle;
    public float forwardSlopeAngle;
    public Vector3 worldMoveDir;
    public Vector3 groundCross;

    public bool isForwardBlocked;

    //항상 움직이기 전에 바닥과 앞부분을 검사한다.
    private void CheckGround()
    {
        groundDistance = float.MaxValue;
        groundNormal = Vector3.up;//현재 출돌한 지면의 노멀벡터
        groundSlopeAngle = 0f;
        forwardSlopeAngle = 0f;
        worldMoveDir = _worldMove = Com.tpRig.TransformDirection(_moveDir);

        bool cast = Physics.SphereCast(capsule.transform.position, capsule.radius, Vector3.down, out var hit, /*checkOption.groundCheckDistance*/2f, groundLayerMask, QueryTriggerInteraction.Ignore/*트리거 이벤트를 발생시키지 않는다.*/);

        //상태를 바꿔주기 전에 초기화
        State.isGrounded = false;


        if (cast)
        {
            groundNormal = hit.normal;

            groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);//지면 충돌 지점의 각도
            forwardSlopeAngle = Vector3.Angle(groundNormal, worldMoveDir) - 90f;

            isOnSteepSlope = groundSlopeAngle >= maxSlopeAngle;//이동할수 있는 지면인지 아닌지 검사

            groundDistance = Mathf.Max(hit.distance - capsule.height / 2, 0f);

            State.isGrounded = (groundDistance <= 0.0001f) && !isOnSteepSlope;//지면과의 거리가 0.0001 이하이고 움직일수 있는 경사면일때 isgrounded가 true가 된다.


        }

        // 월드 이동벡터 회전축
        groundCross = Vector3.Cross(groundNormal, Vector3.up);//이동방향을 정해줄때 상하 회전을 위한 축->3차원공간이기 때문에 축이 필요


    }

    //점프를 하는 등의 동작을 했을때 벽에 붙어서 움직이면 공중에 떠있는 경우를 막기 위해
    private void CheckFoward()
    {
        Vector3 CapsuleTopCenterPoint = new Vector3(transform.position.x, transform.position.y + capsule.height - capsule.radius, transform.position.z);
        Vector3 CepsuleBottomCenterPoint = new Vector3(transform.position.x, transform.position.y - capsule.height + capsule.radius, transform.position.z);
        worldMoveDir= _worldMove = Com.tpRig.TransformDirection(_moveDir);



        //내가 갈 방향의 대각선 아래로 캡슐캐스트를 보낸다.
        bool cast = Physics.CapsuleCast(CepsuleBottomCenterPoint, CapsuleTopCenterPoint, capsule.radius, worldMoveDir + Vector3.down * 0.1f, out var hit, /*checkOption.forwardCheckDistance*/0.1f, -1, QueryTriggerInteraction.Ignore);

        isForwardBlocked = false;

        if (cast)
        {
            forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
            isForwardBlocked = forwardObstacleAngle >= maxSlopeAngle;
        }

    }




    //점프 하기 전에 땅에 붙어있는지 확인하고 isGrounded 변수를 초기화 해주는 함수
    //private void CheckDistanceFromGround()
    //{
    //    Vector3 ro = transform.position + Vector3.up;
    //    Vector3 rd = Vector3.down;
    //    Ray ray = new Ray(ro, rd);

    //    const float rayDist = 500f;
    //    const float threshold = 0.01f;

    //    bool cast = Physics.SphereCast(ray, _groundCheckRadius, out var hit, rayDist, MoveOption.groundLayerMask);

    //    _distFromGround = cast ? (hit.distance - 1f + _groundCheckRadius) : float.MaxValue;
    //    State.isGrounded = _distFromGround <= _groundCheckRadius + threshold;
    //}

    private void Jump()
    {
        if (!State.isGrounded)
        {
            return;
        }
        if (Input.GetKeyDown(Key.junp))
        {
            Com.rBody.AddForce(Vector3.up * MoveOption.jumpForce, ForceMode.VelocityChange);
        }
    }


    private void LogNotInitializedComponentError<T>(T component, string componentName) where T : Component
    {
        if (component == null)
            Debug.LogError($"{componentName} 컴포넌트를 인스펙터에 넣어주세요");
    }

    private void SetValuesByKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveFoward)) v += 1.0f;
        if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
        if (Input.GetKey(Key.moveRight)) h += 1.0f;


        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.acceleration);
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;//조금이라고 값이 있으면 움직이는중
        Com.anim.SetBool("AniRun", State.isMoving);
        State.isRunning = Input.GetKey(Key.run);



    }

    private void Rotate()
    {
        if (State.isCurrentFp)//1인칭
        {
            if (!State.isCursorActive)
            {
                RotateFP();
            }
        }
        else//3인칭
        {
            if (!State.isCursorActive)
            {
                RotateTP();
                RotateFP();
            }
            //RotateFPRoot();
        }
    }

    private void RotateFP()
    {
        float deltaCoef = Time.deltaTime * 50f;
        //상하회전 FP Rig
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y * CamOption.RotationSpeed * deltaCoef;

        if (xRotNext > 180f)
        {
            xRotNext -= 360f;
        }
        //좌우회전 FP Root
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext = yRotPrev + _rotation.x * CamOption.RotationSpeed * deltaCoef;

        //상하회전 가능 여부
        bool xRotatable = CamOption.lookUpDegree < xRotNext && CamOption.lookDownDegree > xRotNext;

        //상하 회전 적용
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        //좌우 회전 적용
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    private void RotateTP()
    {
        float deltaCoef = Time.deltaTime * 50f;

        //상하 TP Rig 회전
        float xRotPrev = Com.tpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y * CamOption.RotationSpeed * deltaCoef;

        if (xRotNext > 180f)
        {
            xRotNext -= 360f;
        }

        //좌우 TP Rig 회전
        float yRotPrev = Com.tpRig.localEulerAngles.y;
        float yRotNext = yRotPrev + _rotation.x * CamOption.RotationSpeed * deltaCoef;

        //상하 회전 가능 여부
        bool xRotatable = CamOption.lookUpDegree < xRotNext && CamOption.lookDownDegree > xRotNext;

        Vector3 nextRot = new Vector3(xRotatable ? xRotNext : xRotPrev, yRotNext, 0f);

        //TP Rig 회전 적용
        Com.tpRig.localEulerAngles = nextRot;


    }

    private void RotateFPRoot()
    {
        if (State.isMoving == false)
        {
            return;
        }

        Vector3 dir = Com.tpRig.TransformDirection(_moveDir);
        float currentY = Com.fpRoot.localEulerAngles.y;
        float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        if (nextY - currentY > 180f)
        {
            nextY -= 360f;
        }
        else if (currentY - nextY > 180f)
        {
            nextY += 360f;
        }

        Com.fpRoot.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);

    }



    private void Move()
    {
        if (isForwardBlocked) 
        {
            ////안움직이고 있을때는 y값 고정
            //Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            //Com.rBody.velocity = new Vector3(0f, 0f, 0f);
            return;
        }
        if (State.isMoving == false)
        {
            //안움직이고 있을때는 y값 고정
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        }

        //실제 이동 벡터 계산
        //1인칭
        if (State.isCurrentFp)
        {
            _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        }
        //3인칭
        else
        {
            _worldMove = Com.tpRig.TransformDirection(_moveDir);
        }

        _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);
        //y축 속도는 유지하면서 xz평면 이동
        Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);

    }

    private void ShowCursorToggle()
    {
        if (Input.GetKeyDown(Key.showCursor))
        {
            State.isCursorActive = !State.isCursorActive;
        }
        ShowCursor(State.isCursorActive);
    }

    private void ShowCursor(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void CameraViewToggle()
    {
        if (Input.GetKeyDown(Key.switchCamera))
        {
            State.isCurrentFp = !State.isCurrentFp;
            Com.fpCamObject.SetActive(State.isCurrentFp);
            Com.tpCamObject.SetActive(!State.isCurrentFp);
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetValuesByKeyInput();
        ShowCursorToggle();
        CameraViewToggle();
        //CheckDistanceFromGround();

        CheckGround();
        CheckFoward();

        Rotate();
        Move();
        Jump();




    }
}
