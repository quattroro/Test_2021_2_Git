using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CharacterMainController : MonoBehaviour
{
    #region ������
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



    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float speed = 3f;
        [Range(1f, 3f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runningCoef = 1.5f;
        [Range(1f, 10f), Tooltip("��������")]
        public float jumpForce = 5.5f;
        [Range(1f, 10f), Tooltip("���� ����")]
        public float acceleration = 1.5f;
        [Tooltip("�������� üũ�� ���̾� ����")]
        public LayerMask groundLayerMask = -1;
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
    #endregion

    public Components Com => _components;
    public KeyOption Key => _keyOption;
    public MovementOption MoveOption => _movementOption;

    public CameraOption CamOption => _cameraOption;
    public AnimatorOption AnimOption => _animatorOption;
    public CharacterState State => _state;

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


    private float _distFromGround;

    public float _groundCheckRadius;

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
        TryGetComponent(out CapsuleCollider cCol);
        _groundCheckRadius = cCol ? cCol.height / 2 : 0.1f;
        //Debug.Log($"{cCol.height}");
    }

    //���� �ϱ� ���� ���� �پ��ִ��� Ȯ���ϰ� isGrounded ������ �ʱ�ȭ ���ִ� �Լ�
    private void CheckDistanceFromGround()
    {
        Vector3 ro = transform.position + Vector3.up;
        Vector3 rd = Vector3.down;
        Ray ray = new Ray(ro, rd);

        const float rayDist = 500f;
        const float threshold = 0.01f;

        bool cast = Physics.SphereCast(ray, _groundCheckRadius, out var hit, rayDist, MoveOption.groundLayerMask);

        _distFromGround = cast ? (hit.distance - 1f + _groundCheckRadius) : float.MaxValue;
        State.isGrounded = _distFromGround <= _groundCheckRadius + threshold;
    }

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
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.acceleration);
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;//�����̶�� ���� ������ �����̴���
        State.isRunning = Input.GetKey(Key.run);



    }

    private void Rotate()
    {
        if (State.isCurrentFp)//1��Ī
        {
            if (!State.isCursorActive)
            {
                RotateFP();
            }
        }
        else//3��Ī
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
        if (State.isMoving == false)
        {
            //�ȿ����̰� �������� y�� ����
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        }

        //���� �̵� ���� ���
        //1��Ī
        if (State.isCurrentFp)
        {
            _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        }
        //3��Ī
        else
        {
            _worldMove = Com.tpRig.TransformDirection(_moveDir);
        }

        _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);
        //y�� �ӵ��� �����ϸ鼭 xz��� �̵�
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
        CheckDistanceFromGround();



        Rotate();
        Move();
        Jump();




    }
}