using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Physics3DMove : MonoBehaviour
{
    public enum CameraType { FPCamera,TPCamera};//1인칭 3인칭 

    /***********************************************************************
    *                               Definitions
    ***********************************************************************/
    #region 변수 정의
    [System.Serializable]
    public class Components
    {
        public Camera tpCamera;
        public Camera fpCamera;

        [HideInInspector] public Transform tpRig;
        [HideInInspector] public Transform fpRoot;
        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject tpCamObject;
        [HideInInspector] public GameObject fpCamObject;

        [HideInInspector] public Rigidbody rBody;//움직임을 담당할 주체
        [HideInInspector] public Animator anim;//애니메이터

        [HideInInspector] public CapsuleCollider capsule;//충돌체
    }

    [Serializable]
    public class CheckOption
    {
        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;

        [Range(0.01f, 0.5f), Tooltip("전방 감지 거리")]
        public float forwardCheckDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
        public float groundCheckDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("지면 인식 허용 거리")]
        public float groundCheckThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("이동속도")]
        public float speed = 5f;

        [Range(1f, 3f), Tooltip("달리기 이동속도 증가 계수")]
        public float runningCoef = 1.5f;

        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 4.2f;

        [Range(0.0f, 2.0f), Tooltip("점프 쿨타임")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("점프 허용 횟수")]
        public int maxJumpCount = 1;

        [Range(1f, 75f), Tooltip("등반 가능한 경사각")]
        public float maxSlopeAngle = 50f;

        [Range(0f, 4f), Tooltip("경사로 이동속도 변화율(가속/감속)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;

        [Range(0f, 5.0f), Tooltip("이동 보간값")]
        public float acceleration = 0.1f;




    }

    [Serializable]
    public class CurrentState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isOnSteepSlope;   // 등반 불가능한 경사로에 올라와 있음
        public bool isJumpTriggered;
        public bool isJumping;
        public bool isForwardBlocked; // 전방에 장애물 존재
        public bool isOutOfControl;   // 제어 불가 상태

        public bool isCurrentFp;
        public bool isCursorActive;
    }

    [Serializable]
    public class CurrentValue
    {
        public Vector3 worldMoveDir;
        public Vector3 groundNormal;
        public Vector3 groundCross;
        public Vector3 horizontalVelocity;

        [Space]
        public float jumpCooldown;
        public int jumpCount;
        public float outOfControllDuration;

        [Space]
        public float groundDistance;
        public float groundSlopeAngle;         // 현재 바닥의 경사각
        public float groundVerticalSlopeAngle; // 수직으로 재측정한 경사각
        public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각
        public float slopeAccel;        // 경사로 인한 가속/감속 비율

        [Space]
        public float gravity;
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
    public class KeyOption
    {
        public KeyCode moveFoward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode run = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;
        public KeyCode switchCamera = KeyCode.Tab;
        public KeyCode showCursor = KeyCode.LeftAlt;

        //public KeyCode LClick = 1;
    }

    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveY = "Move Y";
        public string paramMoveZ = "Move Z";
        public string paramJump = "Jump";
        public string paramRun = "Running";
        


    }

    [SerializeField]
    public Components Com = new Components();
    [SerializeField]
    public KeyOption Key = new KeyOption();
    [SerializeField]
    public MovementOption moveOption = new MovementOption();
    [SerializeField]
    public CameraOption CamOption = new CameraOption();
    [SerializeField]
    public AnimatorOption aniOption = new AnimatorOption();
    [SerializeField]
    public CurrentState State = new CurrentState();
    [SerializeField]
    public CheckOption checkOption = new CheckOption();
    [SerializeField]
    public CurrentValue currentValue = new CurrentValue();

    //그 외 필요한 변수들
    public float _groundCheckRadius;
    private Vector3 _rotation;


    #endregion


    private void InitComponents()
    {
        LogNotInitializedComponentError(Com.tpCamera, "TP Camera");
        LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
        TryGetComponent(out Com.rBody);
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
        TryGetComponent(out Com.capsule);
        _groundCheckRadius = Com.capsule ? Com.capsule.height / 2 : 0.1f;
        //Debug.Log($"{cCol.height}");
    }

    //항상 움직이기 전에 바닥과 앞부분을 검사한다.
    private void CheckGround()
    {
        currentValue.groundDistance = float.MaxValue;
        currentValue.groundNormal = Vector3.up;//현재 출돌한 지면의 노멀벡터
        currentValue.groundSlopeAngle = 0f;
        currentValue.forwardSlopeAngle = 0f;

        bool cast = Physics.SphereCast(Com.capsule.transform.position, Com.capsule.radius, Vector3.down, out var hit, checkOption.groundCheckDistance, checkOption.groundLayerMask, QueryTriggerInteraction.Ignore/*트리거 이벤트를 발생시키지 않는다.*/);

        //상태를 바꿔주기 전에 초기화
        State.isGrounded = false;


        if (cast)
        {
            currentValue.groundNormal = hit.normal;

            currentValue.groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);//지면 충돌 지점의 각도
            currentValue.forwardSlopeAngle = Vector3.Angle(currentValue.groundNormal, currentValue.worldMoveDir) - 90f;

            State.isOnSteepSlope = currentValue.groundSlopeAngle >= moveOption.maxSlopeAngle;//이동할수 있는 지면인지 아닌지 검사

            currentValue.groundDistance = Mathf.Max(hit.distance - Com.capsule.height / 2, 0f);

            State.isGrounded = (currentValue.groundDistance <= 0.0001f) && !State.isOnSteepSlope;//지면과의 거리가 0.0001 이하이고 움직일수 있는 경사면일때 isgrounded가 true가 된다.


        }

        // 월드 이동벡터 회전축
        currentValue.groundCross = Vector3.Cross(currentValue.groundNormal, Vector3.up);//이동방향을 정해줄때 상하 회전을 위한 축->3차원공간이기 때문에 축이 필요


    }

    //점프를 하는 등의 동작을 했을때 벽에 붙어서 움직이면 공중에 떠있는 경우를 막기 위해
    private void CheckFoward()
    {
        Vector3 CapsuleTopCenterPoint = new Vector3(transform.position.x, transform.position.y + Com.capsule.height - Com.capsule.radius, transform.position.z);
        Vector3 CepsuleBottomCenterPoint = new Vector3(transform.position.x, transform.position.y - Com.capsule.height + Com.capsule.radius, transform.position.z);

        //내가 갈 방향의 대각선 아래로 캡슐캐스트를 보낸다.
        bool cast = Physics.CapsuleCast(CepsuleBottomCenterPoint, CapsuleTopCenterPoint, Com.capsule.radius, currentValue.worldMoveDir + Vector3.down * 0.1f, out var hit, checkOption.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

        State.isForwardBlocked = false;

        if(cast)
        {
            float forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isForwardBlocked = forwardObstacleAngle >= moveOption.maxSlopeAngle;
        }

    }


    ////떨어질때 중력가속도를 붙혀준다.
    //private void UpdatePhysics()
    //{
    //    //중력을 사용 안하고 내가 직접 제어 한다.
    //    if (State.isGrounded)
    //    {
    //        currentValue.gravity = 0f;

    //        currentValue.jumpCount = 0;
    //        State.isJumping = false;
    //    }
    //    else
    //    {
    //        currentValue.gravity += 0.02f * moveOption.gravity;
    //    }
    //}




    private void Jump()
    {
        //if (currentValue.jumpCooldown)
        //{
        //    return;
        //}
        if (Input.GetKeyDown(Key.jump))
        {
            State.isJumpTriggered = true;
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
        currentValue.worldMoveDir = Vector3.Lerp(currentValue.worldMoveDir, moveInput, moveOption.acceleration);
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = currentValue.worldMoveDir.sqrMagnitude > 0.01f;//조금이라고 값이 있으면 움직이는중
        State.isRunning = Input.GetKey(Key.run);

    }

    private void Rotate()
    {
        if (State.isCurrentFp)
        {
            if (!State.isCursorActive)
            {
                RotateFP();
            }
        }
        else
        {
            if (!State.isCursorActive)
            {
                RotateTP();
            }
            RotateFPRoot();
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

        Vector3 dir = Com.tpRig.TransformDirection(currentValue.worldMoveDir);
        //Vector3 dir = currentValue.worldMoveDir;
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
        //if (State.isMoving == false)
        //{
        //    //안움직이고 있을때는 y값 고정
        //    Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
        //    return;
        //}

        ////실제 이동 벡터 계산
        ////1인칭
        //if (State.isCurrentFp)
        //{
        //    _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        //}
        ////3인칭
        //else
        //{
        //    _worldMove = Com.tpRig.TransformDirection(_moveDir);
        //}

        //_worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);
        //y축 속도는 유지하면서 xz평면 이동
        //Com.rBody.velocity = new Vector3(currentValue.horizontalVelocity.x, currentValue.horizontalVelocity.y, currentValue.horizontalVelocity.z);

        if (State.isOutOfControl)
        {
            //Com.rBody.velocity = new Vector3(Com.rBody.velocity.x, currentValue.gravity, Com.rBody.velocity.z);
            return;
        }

        Com.rBody.velocity = currentValue.horizontalVelocity/* + Vector3.up*/;


    }

    //private void ApplyMovementsToRigidbody()
    //{
    //    if (State.isOutOfControl)
    //    {
    //        Com.rBody.velocity = new Vector3(Com.rBody.velocity.x, Current.gravity, Com.rBody.velocity.z);
    //        return;
    //    }

    //    Com.rBody.velocity = Current.horizontalVelocity + Vector3.up * (Current.gravity);
    //}


    //바닥에 닿으면 점프 관련 변수들을 초기화 해주는 함수가 필요

    private void CalculateMovements()
    {

        if (State.isOutOfControl)
        {
            currentValue.horizontalVelocity = Vector3.zero;
            return;
        }

        // 0. 가파른 경사면에 있는 경우 : 꼼짝말고 미끄럼틀 타기
        //if (State.isOnSteepSlope && Current.groundDistance < 0.1f)
        //{
        //    DebugMark(0);

        //    Current.horizontalVelocity =
        //        Quaternion.AngleAxis(90f - Current.groundSlopeAngle, Current.groundCross) * (Vector3.up * Current.gravity);

        //    Com.rBody.velocity = Current.horizontalVelocity;

        //    return;
        //}

        // 1. 점프
        if (State.isJumpTriggered && currentValue.jumpCooldown <= 0f)
        {
            //DebugMark(1);

            //Current.gravity = MOption.jumpForce;

            Com.rBody.AddForce(Vector3.up * moveOption.jumpForce);

            // 점프 쿨타임, 트리거 초기화
            currentValue.jumpCooldown = moveOption.jumpCooldown;
            State.isJumpTriggered = false;
            State.isJumping = true;

            currentValue.jumpCount++;
        }

        // 2. XZ 이동속도 계산
        // 공중에서 전방이 막힌 경우 제한 (지상에서는 벽에 붙어서 이동할 수 있도록 허용)
        if (State.isForwardBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
        {
            //DebugMark(2);

            currentValue.horizontalVelocity = Vector3.zero;
        }
        else // 이동 가능한 경우 : 지상 or 전방이 막히지 않음
        {
            //DebugMark(3);

            float speed = !State.isMoving ? 0f :
                          !State.isRunning ? moveOption.speed :
                                             moveOption.speed * moveOption.runningCoef;

            //1인칭
            if (State.isCurrentFp)
            {
                currentValue.worldMoveDir = Com.fpRoot.TransformDirection(currentValue.worldMoveDir);
            }
            //3인칭
            else
            {
                currentValue.worldMoveDir = Com.tpRig.TransformDirection(currentValue.worldMoveDir);
            }

            currentValue.horizontalVelocity = currentValue.worldMoveDir * speed;
        }

        // 3. XZ 벡터 회전
        // 지상이거나 지면에 가까운 높이
        if (State.isGrounded || currentValue.groundDistance < checkOption.groundCheckDistance && !State.isJumping)
        {
            if (State.isMoving && !State.isForwardBlocked)
            {
                //DebugMark(4);

                // 경사로 인한 가속/감속
                if (moveOption.slopeAccel > 0f)
                {
                    bool isPlus = currentValue.forwardSlopeAngle >= 0f;//속도가 증가하냐 감소하냐
                    float absFsAngle = isPlus ? currentValue.forwardSlopeAngle : -currentValue.forwardSlopeAngle;
                    float accel = moveOption.slopeAccel * absFsAngle * 0.01111f + 1f;
                    currentValue.slopeAccel = !isPlus ? accel : 1.0f / accel;

                    currentValue.horizontalVelocity *= currentValue.slopeAccel;
                }

                // 벡터 회전 (경사로)
                currentValue.horizontalVelocity = Quaternion.AngleAxis(-currentValue.groundSlopeAngle, currentValue.groundCross) * currentValue.horizontalVelocity;
            }
        }

        //GzUpdateValue(ref _gzRotatedWorldMoveDir, Current.horizontalVelocity * 0.2f);
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
        InitComponents();
        InitSettings();
    }

    // Update is called once per frame
    void Update()
    {

        SetValuesByKeyInput();
        ShowCursorToggle();
        CameraViewToggle();

        CheckGround();
        CheckFoward();
        //CheckDistanceFromGround();
        CalculateMovements();

        Jump();
        Rotate();
        Move();



    }
}
