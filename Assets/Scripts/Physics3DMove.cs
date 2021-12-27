using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Physics3DMove : MonoBehaviour
{
    public enum CameraType { FPCamera,TPCamera};//1��Ī 3��Ī 

    /***********************************************************************
    *                               Definitions
    ***********************************************************************/
    #region ���� ����
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

        [HideInInspector] public Rigidbody rBody;//�������� ����� ��ü
        [HideInInspector] public Animator anim;//�ִϸ�����

        [HideInInspector] public CapsuleCollider capsule;//�浹ü
    }

    [Serializable]
    public class CheckOption
    {
        [Tooltip("�������� üũ�� ���̾� ����")]
        public LayerMask groundLayerMask = -1;

        [Range(0.01f, 0.5f), Tooltip("���� ���� �Ÿ�")]
        public float forwardCheckDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
        public float groundCheckDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("���� �ν� ��� �Ÿ�")]
        public float groundCheckThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float speed = 5f;

        [Range(1f, 3f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runningCoef = 1.5f;

        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpForce = 4.2f;

        [Range(0.0f, 2.0f), Tooltip("���� ��Ÿ��")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("���� ��� Ƚ��")]
        public int maxJumpCount = 1;

        [Range(1f, 75f), Tooltip("��� ������ ��簢")]
        public float maxSlopeAngle = 50f;

        [Range(0f, 4f), Tooltip("���� �̵��ӵ� ��ȭ��(����/����)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;

        [Range(0f, 5.0f), Tooltip("�̵� ������")]
        public float acceleration = 0.1f;




    }

    [Serializable]
    public class CurrentState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isOnSteepSlope;   // ��� �Ұ����� ���ο� �ö�� ����
        public bool isJumpTriggered;
        public bool isJumping;
        public bool isForwardBlocked; // ���濡 ��ֹ� ����
        public bool isOutOfControl;   // ���� �Ұ� ����

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
        public float groundSlopeAngle;         // ���� �ٴ��� ��簢
        public float groundVerticalSlopeAngle; // �������� �������� ��簢
        public float forwardSlopeAngle; // ĳ���Ͱ� �ٶ󺸴� ������ ��簢
        public float slopeAccel;        // ���� ���� ����/���� ����

        [Space]
        public float gravity;
    }

    [Serializable]
    public class CameraOption
    {
        [Tooltip("���� ���� �� ī�޶�")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float RotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("�̵��ӵ�")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("�̵��ӵ�")]
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

    //�� �� �ʿ��� ������
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
            //ȸ���� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        var allcams = FindObjectsOfType<Camera>();
        foreach (var cam in allcams)
        {
            cam.gameObject.SetActive(false);
        }
        //������ ī�޶� �ϳ��� Ȱ��ȭ 
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FPCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);
        Com.tpCamObject.SetActive(!State.isCurrentFp);


        //CapsuleCollider cCol = GetComponent<CapsuleCollider>();
        TryGetComponent(out Com.capsule);
        _groundCheckRadius = Com.capsule ? Com.capsule.height / 2 : 0.1f;
        //Debug.Log($"{cCol.height}");
    }

    //�׻� �����̱� ���� �ٴڰ� �պκ��� �˻��Ѵ�.
    private void CheckGround()
    {
        currentValue.groundDistance = float.MaxValue;
        currentValue.groundNormal = Vector3.up;//���� �⵹�� ������ ��ֺ���
        currentValue.groundSlopeAngle = 0f;
        currentValue.forwardSlopeAngle = 0f;

        bool cast = Physics.SphereCast(Com.capsule.transform.position, Com.capsule.radius, Vector3.down, out var hit, checkOption.groundCheckDistance, checkOption.groundLayerMask, QueryTriggerInteraction.Ignore/*Ʈ���� �̺�Ʈ�� �߻���Ű�� �ʴ´�.*/);

        //���¸� �ٲ��ֱ� ���� �ʱ�ȭ
        State.isGrounded = false;


        if (cast)
        {
            currentValue.groundNormal = hit.normal;

            currentValue.groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);//���� �浹 ������ ����
            currentValue.forwardSlopeAngle = Vector3.Angle(currentValue.groundNormal, currentValue.worldMoveDir) - 90f;

            State.isOnSteepSlope = currentValue.groundSlopeAngle >= moveOption.maxSlopeAngle;//�̵��Ҽ� �ִ� �������� �ƴ��� �˻�

            currentValue.groundDistance = Mathf.Max(hit.distance - Com.capsule.height / 2, 0f);

            State.isGrounded = (currentValue.groundDistance <= 0.0001f) && !State.isOnSteepSlope;//������� �Ÿ��� 0.0001 �����̰� �����ϼ� �ִ� �����϶� isgrounded�� true�� �ȴ�.


        }

        // ���� �̵����� ȸ����
        currentValue.groundCross = Vector3.Cross(currentValue.groundNormal, Vector3.up);//�̵������� �����ٶ� ���� ȸ���� ���� ��->3���������̱� ������ ���� �ʿ�


    }

    //������ �ϴ� ���� ������ ������ ���� �پ �����̸� ���߿� ���ִ� ��츦 ���� ����
    private void CheckFoward()
    {
        Vector3 CapsuleTopCenterPoint = new Vector3(transform.position.x, transform.position.y + Com.capsule.height - Com.capsule.radius, transform.position.z);
        Vector3 CepsuleBottomCenterPoint = new Vector3(transform.position.x, transform.position.y - Com.capsule.height + Com.capsule.radius, transform.position.z);

        //���� �� ������ �밢�� �Ʒ��� ĸ��ĳ��Ʈ�� ������.
        bool cast = Physics.CapsuleCast(CepsuleBottomCenterPoint, CapsuleTopCenterPoint, Com.capsule.radius, currentValue.worldMoveDir + Vector3.down * 0.1f, out var hit, checkOption.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

        State.isForwardBlocked = false;

        if(cast)
        {
            float forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isForwardBlocked = forwardObstacleAngle >= moveOption.maxSlopeAngle;
        }

    }


    ////�������� �߷°��ӵ��� �����ش�.
    //private void UpdatePhysics()
    //{
    //    //�߷��� ��� ���ϰ� ���� ���� ���� �Ѵ�.
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
            Debug.LogError($"{componentName} ������Ʈ�� �ν����Ϳ� �־��ּ���");
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

        State.isMoving = currentValue.worldMoveDir.sqrMagnitude > 0.01f;//�����̶�� ���� ������ �����̴���
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
        //����ȸ�� FP Rig
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y * CamOption.RotationSpeed * deltaCoef;

        if (xRotNext > 180f)
        {
            xRotNext -= 360f;
        }
        //�¿�ȸ�� FP Root
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext = yRotPrev + _rotation.x * CamOption.RotationSpeed * deltaCoef;

        //����ȸ�� ���� ����
        bool xRotatable = CamOption.lookUpDegree < xRotNext && CamOption.lookDownDegree > xRotNext;

        //���� ȸ�� ����
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        //�¿� ȸ�� ����
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    private void RotateTP()
    {
        float deltaCoef = Time.deltaTime * 50f;

        //���� TP Rig ȸ��
        float xRotPrev = Com.tpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y * CamOption.RotationSpeed * deltaCoef;

        if (xRotNext > 180f)
        {
            xRotNext -= 360f;
        }

        //�¿� TP Rig ȸ��
        float yRotPrev = Com.tpRig.localEulerAngles.y;
        float yRotNext = yRotPrev + _rotation.x * CamOption.RotationSpeed * deltaCoef;

        //���� ȸ�� ���� ����
        bool xRotatable = CamOption.lookUpDegree < xRotNext && CamOption.lookDownDegree > xRotNext;

        Vector3 nextRot = new Vector3(xRotatable ? xRotNext : xRotPrev, yRotNext, 0f);

        //TP Rig ȸ�� ����
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
        //    //�ȿ����̰� �������� y�� ����
        //    Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
        //    return;
        //}

        ////���� �̵� ���� ���
        ////1��Ī
        //if (State.isCurrentFp)
        //{
        //    _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        //}
        ////3��Ī
        //else
        //{
        //    _worldMove = Com.tpRig.TransformDirection(_moveDir);
        //}

        //_worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);
        //y�� �ӵ��� �����ϸ鼭 xz��� �̵�
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


    //�ٴڿ� ������ ���� ���� �������� �ʱ�ȭ ���ִ� �Լ��� �ʿ�

    private void CalculateMovements()
    {

        if (State.isOutOfControl)
        {
            currentValue.horizontalVelocity = Vector3.zero;
            return;
        }

        // 0. ���ĸ� ���鿡 �ִ� ��� : ��¦���� �̲���Ʋ Ÿ��
        //if (State.isOnSteepSlope && Current.groundDistance < 0.1f)
        //{
        //    DebugMark(0);

        //    Current.horizontalVelocity =
        //        Quaternion.AngleAxis(90f - Current.groundSlopeAngle, Current.groundCross) * (Vector3.up * Current.gravity);

        //    Com.rBody.velocity = Current.horizontalVelocity;

        //    return;
        //}

        // 1. ����
        if (State.isJumpTriggered && currentValue.jumpCooldown <= 0f)
        {
            //DebugMark(1);

            //Current.gravity = MOption.jumpForce;

            Com.rBody.AddForce(Vector3.up * moveOption.jumpForce);

            // ���� ��Ÿ��, Ʈ���� �ʱ�ȭ
            currentValue.jumpCooldown = moveOption.jumpCooldown;
            State.isJumpTriggered = false;
            State.isJumping = true;

            currentValue.jumpCount++;
        }

        // 2. XZ �̵��ӵ� ���
        // ���߿��� ������ ���� ��� ���� (���󿡼��� ���� �پ �̵��� �� �ֵ��� ���)
        if (State.isForwardBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
        {
            //DebugMark(2);

            currentValue.horizontalVelocity = Vector3.zero;
        }
        else // �̵� ������ ��� : ���� or ������ ������ ����
        {
            //DebugMark(3);

            float speed = !State.isMoving ? 0f :
                          !State.isRunning ? moveOption.speed :
                                             moveOption.speed * moveOption.runningCoef;

            //1��Ī
            if (State.isCurrentFp)
            {
                currentValue.worldMoveDir = Com.fpRoot.TransformDirection(currentValue.worldMoveDir);
            }
            //3��Ī
            else
            {
                currentValue.worldMoveDir = Com.tpRig.TransformDirection(currentValue.worldMoveDir);
            }

            currentValue.horizontalVelocity = currentValue.worldMoveDir * speed;
        }

        // 3. XZ ���� ȸ��
        // �����̰ų� ���鿡 ����� ����
        if (State.isGrounded || currentValue.groundDistance < checkOption.groundCheckDistance && !State.isJumping)
        {
            if (State.isMoving && !State.isForwardBlocked)
            {
                //DebugMark(4);

                // ���� ���� ����/����
                if (moveOption.slopeAccel > 0f)
                {
                    bool isPlus = currentValue.forwardSlopeAngle >= 0f;//�ӵ��� �����ϳ� �����ϳ�
                    float absFsAngle = isPlus ? currentValue.forwardSlopeAngle : -currentValue.forwardSlopeAngle;
                    float accel = moveOption.slopeAccel * absFsAngle * 0.01111f + 1f;
                    currentValue.slopeAccel = !isPlus ? accel : 1.0f / accel;

                    currentValue.horizontalVelocity *= currentValue.slopeAccel;
                }

                // ���� ȸ�� (����)
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
