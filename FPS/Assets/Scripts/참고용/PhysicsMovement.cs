using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��¥ : 2021-02-21 PM 8:23:22
// �ۼ��� : Rito

// ���� : ���� �� ���� �������� �̵��Ϸ��� �õ��� ��� ���͸� �߻�

namespace Rito.CharacterControl
{
    public class PhysicsMovement : MonoBehaviour, IMovement3D
    {
        /***********************************************************************
        *                               Definitions
        ***********************************************************************/
        #region .
        [Serializable]
        public class Components
        {
            [HideInInspector] public CapsuleCollider capsule;
            [HideInInspector] public Rigidbody rBody;
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

        #endregion
        /***********************************************************************
        *                               Variables
        ***********************************************************************/
        #region .

        [SerializeField] private Components _components = new Components();
        [SerializeField] private CheckOption _checkOptions = new CheckOption();
        [SerializeField] private MovementOption _moveOptions = new MovementOption();
        [SerializeField] private CurrentState _currentStates = new CurrentState();
        [SerializeField] private CurrentValue _currentValues = new CurrentValue();

        private Components Com => _components;
        private CheckOption COption => _checkOptions;
        private MovementOption MOption => _moveOptions;
        private CurrentState State => _currentStates;
        private CurrentValue Current => _currentValues;


        private float _capsuleRadiusDiff;//ĸ���ݶ��̴�
        private float _fixedDeltaTime;

        private float _castRadius; // Sphere, Capsule ����ĳ��Ʈ ������
        private Vector3 CapsuleTopCenterPoint
            => new Vector3(transform.position.x, transform.position.y + Com.capsule.height - Com.capsule.radius, transform.position.z);
        private Vector3 CapsuleBottomCenterPoint
            => new Vector3(transform.position.x, transform.position.y + Com.capsule.radius, transform.position.z);

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Start()
        {
            InitRigidbody();
            InitCapsuleCollider();
        }

        private void FixedUpdate()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;

            CheckGround();//�ٴڰ˻�
            CheckForward();//����˻�

            UpdatePhysics();//
            UpdateValues();

            CalculateMovements();
            ApplyMovementsToRigidbody();
        }

        #endregion
        /***********************************************************************
        *                               Init Methods
        ***********************************************************************/
        #region .

        private void InitRigidbody()
        {
            TryGetComponent(out Com.rBody);
            if (Com.rBody == null) Com.rBody = gameObject.AddComponent<Rigidbody>();

            // ȸ���� �ڽ� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
            Com.rBody.interpolation = RigidbodyInterpolation.Interpolate;
            Com.rBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Com.rBody.useGravity = false; // �߷� ���� ����
        }


        private void InitCapsuleCollider()
        {
            TryGetComponent(out Com.capsule);
            if (Com.capsule == null)
            {
                Com.capsule = gameObject.AddComponent<CapsuleCollider>();

                // �������� ��� Ž���Ͽ� ���� ����
                float maxHeight = -1f;

                // 1. SMR Ȯ��
                var smrArr = GetComponentsInChildren<SkinnedMeshRenderer>();
                if (smrArr.Length > 0)
                {
                    foreach (var smr in smrArr)
                    {
                        foreach (var vertex in smr.sharedMesh.vertices)
                        {
                            if (maxHeight < vertex.y)
                                maxHeight = vertex.y;
                        }
                    }
                }
                // 2. MR Ȯ��
                else
                {
                    var mfArr = GetComponentsInChildren<MeshFilter>();
                    if (mfArr.Length > 0)
                    {
                        foreach (var mf in mfArr)
                        {
                            foreach (var vertex in mf.mesh.vertices)
                            {
                                if (maxHeight < vertex.y)
                                    maxHeight = vertex.y;
                            }
                        }
                    }
                }

                // 3. ĸ�� �ݶ��̴� �� ����
                if (maxHeight <= 0)
                    maxHeight = 1f;

                float center = maxHeight * 0.5f;

                Com.capsule.height = maxHeight;
                Com.capsule.center = Vector3.up * center;
                Com.capsule.radius = 0.2f;
            }

            _castRadius = Com.capsule.radius * 0.9f;
            _capsuleRadiusDiff = Com.capsule.radius - _castRadius + 0.05f;
        }

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .

        bool IMovement3D.IsMoving() => State.isMoving;
        bool IMovement3D.IsGrounded() => State.isGrounded;
        float IMovement3D.GetDistanceFromGround() => Current.groundDistance;

        void IMovement3D.SetMovement(in Vector3 worldMoveDir, bool isRunning)//������ ����� ũ���ϰ� ���� �޸��� �ִ����� �޾ƿͼ� �������� �ʱ�ȭ
        {
            Current.worldMoveDir = worldMoveDir;
            State.isMoving = worldMoveDir.sqrMagnitude > 0.01f;//�������� 0.1���� ũ�� �����̴� ����
            State.isRunning = isRunning;
        }


        //������ ������������ Ȯ���ϰ� ������ �����ش�.
        bool IMovement3D.SetJump()
        {
            // ù ������ ���� �������� ����
            if (!State.isGrounded && Current.jumpCount == 0) return false;

            // ���� ��Ÿ��, Ƚ�� Ȯ��
            if (Current.jumpCooldown > 0f) return false;
            if (Current.jumpCount >= MOption.maxJumpCount) return false;

            // ���� �Ұ��� ���ο��� ���� �Ұ���
            if (State.isOnSteepSlope) return false;

            State.isJumpTriggered = true;
            return true;
        }

        //�������� �����ִ� �Լ�
        void IMovement3D.StopMoving()
        {
            Current.worldMoveDir = Vector3.zero;
            State.isMoving = false;
            State.isRunning = false;
        }

        //
        void IMovement3D.KnockBack(in Vector3 force, float time)
        {
            SetOutOfControl(time);//������ �ð� ���� ��Ʈ�� �Ұ�
            Com.rBody.AddForce(force, ForceMode.Impulse);//�־��� ���� �ݴ� �������� ���� �����ش�.
        }

        //������ �ð� ���� ��Ʈ�� �Ұ�
        public void SetOutOfControl(float time)
        {
            Current.outOfControllDuration = time;
            ResetJump();
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .

        //������ ���õ� ������ �ʱ�ȭ
        private void ResetJump()
        {
            Current.jumpCooldown = 0f;
            Current.jumpCount = 0;
            State.isJumping = false;
            State.isJumpTriggered = false;
        }


        /// <summary> �ϴ� ���� �˻� </summary>
        private void CheckGround()
        {
            Current.groundDistance = float.MaxValue;
            Current.groundNormal = Vector3.up;
            Current.groundSlopeAngle = 0f;
            Current.forwardSlopeAngle = 0f;

            //
            bool cast =
                Physics.SphereCast(CapsuleBottomCenterPoint, _castRadius, Vector3.down, out var hit, COption.groundCheckDistance, COption.groundLayerMask, QueryTriggerInteraction.Ignore/*Ʈ���� �̺�Ʈ�� �߻���Ű�� �ʴ´�.*/);

            //���¸� �ٲ��ֱ� ���� �ʱ�ȭ
            State.isGrounded = false;


            //�浹ü�� ������ 
            if (cast)
            {
                // ���� ��ֺ��� �ʱ�ȭ
                Current.groundNormal = hit.normal;//�浹��ġ���� 

                // ���� ��ġ�� ������ ��簢 ���ϱ�(ĳ���� �̵����� ���)
                Current.groundSlopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);//������ ���� ����
                Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, Current.worldMoveDir) - 90f;

                State.isOnSteepSlope = Current.groundSlopeAngle >= MOption.maxSlopeAngle;

                // ��簢 ���߰��� (���� ����ĳ��Ʈ) : �����ϰų� ���� �κ� üũ
                //if (State.isOnSteepSlope)
                //{
                //    Vector3 ro = hit.point + Vector3.up * 0.1f;
                //    Vector3 rd = Vector3.down;
                //    bool rayD = 
                //        Physics.SphereCast(ro, 0.09f, rd, out var hitRayD, 0.2f, COption.groundLayerMask, QueryTriggerInteraction.Ignore);

                //    Current.groundVerticalSlopeAngle = rayD ? Vector3.Angle(hitRayD.normal, Vector3.up) : Current.groundSlopeAngle;

                //    State.isOnSteepSlope = Current.groundVerticalSlopeAngle >= MOption.maxSlopeAngle;
                //}


                Current.groundDistance = Mathf.Max(hit.distance - _capsuleRadiusDiff - COption.groundCheckThreshold, 0f);

                State.isGrounded =
                    (Current.groundDistance <= 0.0001f) && !State.isOnSteepSlope;

                GzUpdateValue(ref _gzGroundTouch, hit.point);
            }

            // ���� �̵����� ȸ����
            Current.groundCross = Vector3.Cross(Current.groundNormal, Vector3.up);
        }

        /// <summary> ���� ��ֹ� �˻� : ���̾� ���� ���� trigger�� �ƴ� ��� ��ֹ� �˻� </summary>
        private void CheckForward()
        {
            bool cast =
                Physics.CapsuleCast(CapsuleBottomCenterPoint, CapsuleTopCenterPoint, _castRadius, Current.worldMoveDir + Vector3.down * 0.1f,
                    out var hit, COption.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

            State.isForwardBlocked = false;
            if (cast)
            {
                float forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
                State.isForwardBlocked = forwardObstacleAngle >= MOption.maxSlopeAngle;

                GzUpdateValue(ref _gzForwardTouch, hit.point);
            }
        }

        private void UpdatePhysics()//�������� �߷°��ӵ��� �����ش�.
        {
            // Custom Gravity, Jumping State
            if (State.isGrounded)
            {
                Current.gravity = 0f;

                Current.jumpCount = 0;
                State.isJumping = false;
            }
            else
            {
                Current.gravity += _fixedDeltaTime * MOption.gravity;
            }
        }

        //���� ��Ÿ��, ��Ʈ�� �Ұ� �����϶� ���� �ٿ��ش�.
        private void UpdateValues()
        {
            // Calculate Jump Cooldown
            if (Current.jumpCooldown > 0f)
                Current.jumpCooldown -= _fixedDeltaTime;


            // Out Of Control
            State.isOutOfControl = Current.outOfControllDuration > 0f;

            if (State.isOutOfControl)
            {
                Current.outOfControllDuration -= _fixedDeltaTime;
                Current.worldMoveDir = Vector3.zero;
            }
        }

        private void CalculateMovements()
        {

            if (State.isOutOfControl)
            {
                Current.horizontalVelocity = Vector3.zero;
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
            if (State.isJumpTriggered && Current.jumpCooldown <= 0f)
            {
                DebugMark(1);

                Current.gravity = MOption.jumpForce;

                // ���� ��Ÿ��, Ʈ���� �ʱ�ȭ
                Current.jumpCooldown = MOption.jumpCooldown;
                State.isJumpTriggered = false;
                State.isJumping = true;

                Current.jumpCount++;
            }

            // 2. XZ �̵��ӵ� ���
            // ���߿��� ������ ���� ��� ���� (���󿡼��� ���� �پ �̵��� �� �ֵ��� ���)
            if (State.isForwardBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
            {
                DebugMark(2);

                Current.horizontalVelocity = Vector3.zero;
            }
            else // �̵� ������ ��� : ���� or ������ ������ ����
            {
                DebugMark(3);

                float speed = !State.isMoving ? 0f :
                              !State.isRunning ? MOption.speed :
                                                 MOption.speed * MOption.runningCoef;

                Current.horizontalVelocity = Current.worldMoveDir * speed;
            }

            // 3. XZ ���� ȸ��
            // �����̰ų� ���鿡 ����� ����
            if (State.isGrounded || Current.groundDistance < COption.groundCheckDistance && !State.isJumping)
            {
                if (State.isMoving && !State.isForwardBlocked)
                {
                    DebugMark(4);

                    // ���� ���� ����/����
                    if (MOption.slopeAccel > 0f)
                    {
                        bool isPlus = Current.forwardSlopeAngle >= 0f;//�ӵ��� �����ϳ� �����ϳ�
                        float absFsAngle = isPlus ? Current.forwardSlopeAngle : -Current.forwardSlopeAngle;
                        float accel = MOption.slopeAccel * absFsAngle * 0.01111f + 1f;
                        Current.slopeAccel = !isPlus ? accel : 1.0f / accel;

                        Current.horizontalVelocity *= Current.slopeAccel;
                    }

                    // ���� ȸ�� (����)
                    Current.horizontalVelocity =
                        Quaternion.AngleAxis(-Current.groundSlopeAngle, Current.groundCross) * Current.horizontalVelocity;
                }
            }

            GzUpdateValue(ref _gzRotatedWorldMoveDir, Current.horizontalVelocity * 0.2f);
        }

        /// <summary> ������ٵ� ���� �ӵ� ���� </summary>
        private void ApplyMovementsToRigidbody()
        {
            if (State.isOutOfControl)
            {
                Com.rBody.velocity = new Vector3(Com.rBody.velocity.x, Current.gravity, Com.rBody.velocity.z);
                return;
            }

            Com.rBody.velocity = Current.horizontalVelocity + Vector3.up * (Current.gravity);
        }

        #endregion
        /***********************************************************************
        *                               Debugs
        ***********************************************************************/
        #region .

        public bool _debugOn;
        public int _debugIndex;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DebugMark(int index)
        {
            if (!_debugOn) return;
            Debug.Log("MARK - " + index);
            _debugIndex = index;
        }

        #endregion
        /***********************************************************************
        *                               Gizmos, GUI
        ***********************************************************************/
        #region .

        private Vector3 _gzGroundTouch;
        private Vector3 _gzForwardTouch;
        private Vector3 _gzRotatedWorldMoveDir;

        [Header("Gizmos Option")]
        public bool _showGizmos = true;

        [SerializeField, Range(0.01f, 2f)]
        private float _gizmoRadius = 0.05f;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false) return;
            if (!_showGizmos) return;
            if (!enabled) return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_gzGroundTouch, _gizmoRadius);

            if (State.isForwardBlocked)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_gzForwardTouch, _gizmoRadius);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_gzGroundTouch - Current.groundCross, _gzGroundTouch + Current.groundCross);

            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + _gzRotatedWorldMoveDir);

            Gizmos.color = new Color(0.5f, 1.0f, 0.8f, 0.8f);
            Gizmos.DrawWireSphere(CapsuleTopCenterPoint, _castRadius);
            Gizmos.DrawWireSphere(CapsuleBottomCenterPoint, _castRadius);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void GzUpdateValue<T>(ref T variable, in T value)
        {
            variable = value;
        }



        [SerializeField, Space]
        private bool _showGUI = true;
        [SerializeField]
        private int _guiTextSize = 28;

        private float _prevForwardSlopeAngle;

        private void OnGUI()
        {
            if (Application.isPlaying == false) return;
            if (!_showGUI) return;
            if (!enabled) return;

            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.normal.textColor = Color.yellow;
            labelStyle.fontSize = Math.Max(_guiTextSize, 20);

            _prevForwardSlopeAngle = Current.forwardSlopeAngle == -90f ? _prevForwardSlopeAngle : Current.forwardSlopeAngle;

            var oldColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.Box(new Rect(40, 40, 420, 260), "");
            GUI.color = oldColor;

            GUILayout.BeginArea(new Rect(50, 50, 1000, 500));
            GUILayout.Label($"Ground Height : {Mathf.Min(Current.groundDistance, 99.99f): 00.00}", labelStyle);
            GUILayout.Label($"Slope Angle(Ground)  : {Current.groundSlopeAngle: 00.00}", labelStyle);
            GUILayout.Label($"Slope Angle(Forward) : {_prevForwardSlopeAngle: 00.00}", labelStyle);
            GUILayout.Label($"Allowed Slope Angle : {MOption.maxSlopeAngle: 00.00}", labelStyle);
            GUILayout.Label($"Current Slope Accel : {Current.slopeAccel: 00.00}", labelStyle);
            GUILayout.Label($"Current Speed Mag  : {Current.horizontalVelocity.magnitude: 00.00}", labelStyle);
            GUILayout.EndArea();

            float sWidth = Screen.width;
            float sHeight = Screen.height;

            GUIStyle RTLabelStyle = GUI.skin.label;
            RTLabelStyle.fontSize = 20;
            RTLabelStyle.normal.textColor = Color.green;

            oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUI.Box(new Rect(sWidth - 355f, 5f, 340f, 100f), "");
            GUI.color = oldColor;

            float yPos = 10f;
            GUI.Label(new Rect(sWidth - 350f, yPos, 150f, 30f), $"Speed : {MOption.speed: 00.00}", RTLabelStyle);
            MOption.speed = GUI.HorizontalSlider(new Rect(sWidth - 200f, yPos + 10f, 180f, 20f), MOption.speed, 1f, 10f);

            yPos += 20f;
            GUI.Label(new Rect(sWidth - 350f, yPos, 150f, 30f), $"Jump : {MOption.jumpForce: 00.00}", RTLabelStyle);
            MOption.jumpForce = GUI.HorizontalSlider(new Rect(sWidth - 200f, yPos + 10f, 180f, 20f), MOption.jumpForce, 1f, 10f);

            yPos += 20f;
            GUI.Label(new Rect(sWidth - 350f, yPos, 150f, 30f), $"Jump Count : {MOption.maxJumpCount: 0}", RTLabelStyle);
            MOption.maxJumpCount = (int)GUI.HorizontalSlider(new Rect(sWidth - 200f, yPos + 10f, 180f, 20f), MOption.maxJumpCount, 1f, 3f);

            yPos += 20f;
            GUI.Label(new Rect(sWidth - 350f, yPos, 150f, 30f), $"Max Slope : {MOption.maxSlopeAngle: 00}", RTLabelStyle);
            MOption.maxSlopeAngle = (int)GUI.HorizontalSlider(new Rect(sWidth - 200f, yPos + 10f, 180f, 20f), MOption.maxSlopeAngle, 1f, 75f);

            labelStyle.fontSize = Math.Max(_guiTextSize, 20);
        }

        #endregion
    }
}