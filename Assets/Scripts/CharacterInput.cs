using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FakePhysics
{
    [RequireComponent(typeof(CharacterController2D))]
    public class CharacterInput : MonoBehaviour, IManagedCharController2D
    {
        enum MovementState
        {
            WallSlide,
            WallJump,
            Jump,
            Glide,
            Fall,
            HMove,
            VMove
        }

        [Header("External References:")]
        [SerializeField]
        Transform spriteRoot;

        [Header("Special trigger:")]
        [SerializeField]
        string ladderTag;
        [SerializeField]
        string frictionTag;

        [Header("Controll easer:")]
        [SerializeField]
        int transToFallDelay;
        [SerializeField]
        int maxJumpExecutionDelay;

        [Header("Speed on ground:")]
        [SerializeField]
        float hMaxSpeed;
        [SerializeField]
        float hAcceleration;

        [Header("Jumping:")]
        [SerializeField]
        float jumpSpeed;
        [SerializeField]
        float jumpCutSpeed;
        [SerializeField]
        int minJumpFrames;

        [Header("Gravity:")]
        [SerializeField]
        float gravityAcceleration;
        [SerializeField]
        float gravityCap;
        [SerializeField]
        float slopeGravity;
        [SerializeField]
        float friction;
        [SerializeField]
        float drag;

        [Header("Glide:")]
        [SerializeField]
        float gMaxSpeed;
        [SerializeField]
        float gAcceleration;
        [SerializeField]
        float glideGravityCap;

        [Header("Wall interaction:")]
        [SerializeField]
        float wallSlidingSpeed;
        [SerializeField]
        Vector2 wallJumpForce;
        [SerializeField]
        int wallJumpNoControllFrames;
        [SerializeField]
        float wallLeaveAxisRatio;

        [Header("Vertical move:")]
        [SerializeField]
        float vMaxSpeed;
        [SerializeField]
        float vAcceleration;
        [SerializeField]
        float vhMaxSpeed;
        [SerializeField]
        float vhAcceleration;

        [Header("Friction move:")]
        [SerializeField]
        float fMaxSpeed;
        [SerializeField]
        float fAcceleration;

        //state defining vars
        MovementState _cState = MovementState.Fall;
        bool _delayedIsGrounded = true;
        CharacterController2D _motor;
        Animator animator;
        Vector2 _deltaMovement;
        Vector2 _deltaExternalForces;
        bool _stateAllowsNormalJumping = true;
        bool _stateAllowsNormalGravity = true;
        bool _isJumpButtonPressed = false;
        bool _isWallSliding;
        bool _jumpedSinceLastGrounded;
        bool _startNonAtomic;
        int _facingDir = 1;
        PhysicsMaterial2D _cPhysicalMaterial;

        //corroutines
        Coroutine _CGrounded;
        Coroutine _CJumpPressed;
        Coroutine _CAnalogJump;
        Coroutine _CWallJump;

        //injection
        bool _shouldInjectX;
        bool _shouldInjectY;
        float _injectionValueX;
        float _injectionValueY;

        //external forces
        List<Vector2> externalForce;
        List<GetVelocity> externalVelocity;
        List<Vector2> externalConstantVelocity;
        List<GetVelocity> externalPlattformVelocity;

        //Trigger managment
        int _ladderTriggerCount = 0;

        void Awake()
        {
            _motor = GetComponent<CharacterController2D>();
            _motor.onTriggerEnterEvent += _motor_onTriggerEnterEvent;
            _motor.onTriggerExitEvent += _motor_onTriggerExitEvent;
            animator = GetComponent<Animator>();
            externalForce = new List<Vector2>(2);
            externalVelocity = new List<GetVelocity>(2);
            externalConstantVelocity = new List<Vector2>(2);
            externalPlattformVelocity = new List<GetVelocity>(2);
        }

        private void _motor_onTriggerExitEvent(Collider2D obj)
        {
            if (obj.CompareTag(ladderTag))
            {
                _ladderTriggerCount--;
                if (_ladderTriggerCount == 0)
                {
                    if (_motor.isGrounded)
                        StartNMove();
                    else
                        StartFalling();
                }
            }
        }

        private void _motor_onTriggerEnterEvent(Collider2D obj)
        {
            if (obj.CompareTag(ladderTag))
            {
                if (_cState != MovementState.VMove)
                    StartVMove();
                _ladderTriggerCount++;
            }
        }

        void Update()
        {
            if (_motor.collisionState.wasGroundedLastFrame && !_motor.isGrounded)
                OnIsNotGrounded();
            else if (_motor.collisionState.becameGroundedThisFrame)
            {
                OnBecameGrounded();
                Debug.Log("Became Grounded this frame");
            }

            if (Input.GetButtonDown("Jump") && !_motor.collisionState.standOnToSteepSlope)
            {
                _isJumpButtonPressed = true;
                if (_CJumpPressed != null)
                    StopCoroutine(_CJumpPressed);
            }
            else if (!Input.GetButton("Jump") && _isJumpButtonPressed)
                _CJumpPressed = StartCoroutine(DelayForFrames(() => { _isJumpButtonPressed = false; }, transToFallDelay));
        }

        void FixedUpdate()
        {
            _deltaMovement = _motor.velocity;
            if (_shouldInjectX)
            {
                _shouldInjectX = false;
                _deltaMovement.x = _injectionValueX;
            }
            if (_shouldInjectY)
            {
                _shouldInjectY = false;
                _deltaMovement.y = _injectionValueY;
            }
            if (_stateAllowsNormalGravity)
            {
                _deltaMovement.y = Mathf.Max(_deltaMovement.y + gravityAcceleration, gravityCap);
            }
            if (_stateAllowsNormalJumping && _isJumpButtonPressed)
                StartJumping();

            switch (_cState)
            {
                case MovementState.HMove:
                    _motor.collisionState.belowHit.collider.SendMessage("OnFakeCollisionStay2D", (IManagedCharController2D)this, SendMessageOptions.DontRequireReceiver);
                    if (_motor.collisionState.belowHit.collider.CompareTag(frictionTag))
                    {
                        _cPhysicalMaterial = _motor.collisionState.belowHit.collider.sharedMaterial;
                        MoveHorizontalWithFriction(ref fAcceleration, ref fMaxSpeed);
                        if (_motor.collisionState.standOnToSteepSlope)
                            HandleSlope();
                    }
                    else
                    {
                        MoveHorizontalAtomic(ref hAcceleration, ref hMaxSpeed);
                        if (_motor.collisionState.standOnToSteepSlope)
                            HandleSlope();
                    }
                    break;
                case MovementState.Jump:
                    MoveHorizontalAtomic(ref hAcceleration, ref hMaxSpeed);
                    break;
                case MovementState.Fall:
                    if (_startNonAtomic)
                    {
                        MoveHorizontal(ref hAcceleration, ref hMaxSpeed);
                        if (Input.GetAxisRaw("Horizontal") != 0)
                            _startNonAtomic = false;
                    }
                    else
                        MoveHorizontalAtomic(ref hAcceleration, ref hMaxSpeed);
                    CheckForSideCollisions();
                    if (_isWallSliding)
                        StartWallSliding();
                    if (_isJumpButtonPressed && _jumpedSinceLastGrounded)
                        StartGliding();
                    break;
                case MovementState.WallSlide:
                    CheckForSideCollisions();

                    _deltaMovement.y = wallSlidingSpeed;

                    if (!_isWallSliding)
                        StartFalling();
                    if (_isJumpButtonPressed)
                        StartWallJumping();
                    if ((_motor.collisionState.left && Input.GetAxis("Horizontal") > wallLeaveAxisRatio) || (_motor.collisionState.right && Input.GetAxis("Horizontal") < -wallLeaveAxisRatio))
                        MoveHorizontalAtomic(ref hAcceleration, ref hMaxSpeed);
                    break;
                case MovementState.Glide:
                    _deltaMovement.y = Mathf.Max(_deltaMovement.y + gravityAcceleration, glideGravityCap);
                    if (_startNonAtomic)
                    {
                        MoveHorizontal(ref gAcceleration, ref gMaxSpeed);
                        if (Input.GetAxisRaw("Horizontal") != 0)
                            _startNonAtomic = false;
                    }
                    else
                        MoveHorizontalAtomic(ref gAcceleration, ref gMaxSpeed);
                    if (!Input.GetButton("Jump"))
                        StartFalling();
                    break;
                case MovementState.VMove:
                    MoveHorizontalAtomic(ref vhAcceleration, ref vhMaxSpeed);
                    MoveVerticalAtomic(ref vAcceleration, ref vMaxSpeed);
                    break;
            }
            ApplyExternalInput();
            AdjustFacingDir();
            UpdateAnimatorVars();
            _motor.moveSilent(_deltaExternalForces * Time.fixedDeltaTime, false);
            _motor.move((_deltaMovement + _deltaExternalForces) * Time.fixedDeltaTime, _cState == MovementState.Jump);
        }

        void OnBecameGrounded()
        {
            if (_CGrounded != null)
                StopCoroutine(_CGrounded);
            if (_CAnalogJump != null)
                StopCoroutine(_CAnalogJump);
            if (_CWallJump != null)
                StopCoroutine(_CWallJump);
            if (_cState != MovementState.VMove)
                StartNMove();
        }

        void OnIsNotGrounded()
        {
            _CGrounded = StartCoroutine(DelayForFrames(() => { _delayedIsGrounded = false; }, transToFallDelay));
            if (_cState == MovementState.HMove)
                StartFalling();
        }

        void MoveHorizontalAtomic(ref float acceleration, ref float cap)
        {
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            if (horizontalAxis == 0)
            {
                _deltaMovement.x = 0;
                return;
            }
            if (horizontalAxis > 0 && _deltaMovement.x < 0)
                _deltaMovement.x = 0;
            else if (horizontalAxis < 0 && _deltaMovement.x > 0)
                _deltaMovement.x = 0;
            _deltaMovement.x = Mathf.Clamp(_deltaMovement.x + acceleration * horizontalAxis, -cap, cap);
        }

        void MoveHorizontal(ref float acceleration, ref float cap)
        {
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            if (horizontalAxis > 0 && _deltaMovement.x < 0)
                _deltaMovement.x = 0;
            else if (horizontalAxis < 0 && _deltaMovement.x > 0)
                _deltaMovement.x = 0;
            _deltaMovement.x = Mathf.Clamp(_deltaMovement.x + acceleration * horizontalAxis, -cap, cap);
        }

        void MoveHorizontalWithFriction(ref float acceleration, ref float cap)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            AddVelocity(() => { return Vector2.left * _cPhysicalMaterial.friction * _deltaMovement.x; });
            _deltaMovement.x = Mathf.Clamp(_deltaMovement.x + acceleration * horizontalAxis, -cap, cap);
        }

        void MoveVerticalAtomic(ref float acceleration, ref float cap)
        {
            float verticalAxis = Input.GetAxisRaw("Vertical");
            if (verticalAxis == 0)
            {
                _deltaMovement.y = 0;
                return;
            }
            if (verticalAxis > 0 && _deltaMovement.y < 0)
                _deltaMovement.y = 0;
            else if (verticalAxis < 0 && _deltaMovement.y > 0)
                _deltaMovement.y = 0;
            _deltaMovement.y = Mathf.Clamp(_deltaMovement.y + acceleration * verticalAxis, -cap, cap);
        }

        void UpdateAnimatorVars()
        {
            animator.SetFloat("VelocityX", _deltaMovement.x);
            animator.SetFloat("VelocityY", _deltaMovement.y);
            animator.SetBool("IsGrounded", _motor.isGrounded);
            animator.SetFloat("AbsX", Mathf.Abs(_deltaMovement.x));
            animator.SetFloat("AbsY", Mathf.Abs(_deltaMovement.y));
            animator.SetBool("HasMoveInput", _deltaMovement != Vector2.zero);
        }

        void HandleSlope()
        {
            if (_motor.collisionState.belowHit.normal.x < 0)
                _deltaMovement += new Vector2(_motor.collisionState.belowHit.normal.y, -_motor.collisionState.belowHit.normal.x) * Vector2.Dot(Vector2.up * slopeGravity, _motor.collisionState.belowHit.normal);
            else
                _deltaMovement -= new Vector2(_motor.collisionState.belowHit.normal.y, -_motor.collisionState.belowHit.normal.x) * Vector2.Dot(Vector2.up * slopeGravity, _motor.collisionState.belowHit.normal);
        }

        void StartJumping()
        {
            _isJumpButtonPressed = false;
            _stateAllowsNormalGravity = true;
            _stateAllowsNormalJumping = false;
            _jumpedSinceLastGrounded = true;
            _startNonAtomic = false;
            _cState = MovementState.Jump;
            _CAnalogJump = StartCoroutine(AnalogJump());
        }

        void StartFalling()
        {
            _stateAllowsNormalGravity = true;
            _stateAllowsNormalJumping = false;
            _cState = MovementState.Fall;
        }

        void StartWallSliding()
        {
            _stateAllowsNormalGravity = false;
            _stateAllowsNormalJumping = false;
            _startNonAtomic = false;
            _cState = MovementState.WallSlide;
        }

        void StartGliding()
        {
            _isJumpButtonPressed = false;
            _stateAllowsNormalGravity = false;
            _stateAllowsNormalJumping = false;
            _cState = MovementState.Glide;
        }

        void StartWallJumping()
        {
            _isJumpButtonPressed = false;
            _stateAllowsNormalGravity = true;
            _stateAllowsNormalJumping = false;
            _jumpedSinceLastGrounded = true;
            _startNonAtomic = true;
            _cState = MovementState.WallJump;
            Vector2 wallJumpVelocity = wallJumpForce;
            if (_motor.collisionState.right)
            {
                wallJumpVelocity.x = -wallJumpVelocity.x;
            }
            _deltaMovement += wallJumpVelocity;
            _CWallJump = StartCoroutine(DelayForFrames(() => { StartFalling(); }, wallJumpNoControllFrames));
        }

        void StartVMove()
        {
            _stateAllowsNormalGravity = false;
            _stateAllowsNormalJumping = true;
            _jumpedSinceLastGrounded = false;
            _startNonAtomic = false;
            if (_CGrounded != null)
                StopCoroutine(_CGrounded);
            if (_CAnalogJump != null)
                StopCoroutine(_CAnalogJump);
            if (_CWallJump != null)
                StopCoroutine(_CWallJump);
            _cState = MovementState.VMove;
        }

        void StartNMove()
        {
            _cState = MovementState.HMove;
            _stateAllowsNormalJumping = true;
            _stateAllowsNormalGravity = true;
            _delayedIsGrounded = true;
            _jumpedSinceLastGrounded = false;
            _startNonAtomic = false;
        }

        void FlipFacingDir()
        {
            spriteRoot.localScale = new Vector3(-spriteRoot.localScale.x, spriteRoot.localScale.y, spriteRoot.localScale.z);
            _facingDir *= -1;
        }

        void AdjustFacingDir()
        {
            if (_deltaMovement.x * _facingDir < 0)
                FlipFacingDir();
        }

        private const float wallJumpCollisionDistance = 0.01f;
        void CheckForSideCollisions()
        {
            if (_motor.velocity.x == 0)
            {
                _motor.manuallyCheckForCollisions(wallJumpCollisionDistance);
                _motor.manuallyCheckForCollisions(-wallJumpCollisionDistance);
            }
            else if (_motor.velocity.x > 0)
                _motor.manuallyCheckForCollisions(-wallJumpCollisionDistance);
            else
                _motor.manuallyCheckForCollisions(wallJumpCollisionDistance);
            if (_motor.collisionState.left || _motor.collisionState.right)
                _isWallSliding = true;
            else
                _isWallSliding = false;
        }

        private delegate void DelayedAction();
        IEnumerator DelayForFrames(DelayedAction action, int delay)
        {
            int frameCounter = 0;
            while (frameCounter < delay)
            {
                yield return new WaitForFixedUpdate();
                frameCounter++;
            }
            action();
        }

        IEnumerator AnalogJump()
        {
            float jumpedFrames = 0;
            bool cutJump = false;
            InjectSpeedY(jumpSpeed);
            while (jumpedFrames < minJumpFrames)
            {
                if (!Input.GetButton("Jump"))
                    cutJump = true;
                yield return new WaitForFixedUpdate();
                jumpedFrames++;
            }
            while (_deltaMovement.y > jumpCutSpeed)
            {
                if (!Input.GetButton("Jump") || cutJump)
                {
                    InjectSpeedY(jumpCutSpeed);
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            StartFalling();
        }

        void InjectSpeedX(float newSpeed)
        {
            _shouldInjectX = true;
            _injectionValueX = newSpeed;
        }

        void InjectSpeedY(float newSpeed)
        {
            _shouldInjectY = true;
            _injectionValueY = newSpeed;
        }

        Vector2 DampVelocity(ref Vector2 velocity)
        {
            if (_motor.isGrounded)
                velocity = velocity * (1 - Time.fixedDeltaTime * friction);
            else
                velocity = velocity * (1 - Time.fixedDeltaTime * drag);
            return velocity;
            //velocity += -velocity.normalized * (Mathf.Sqrt(velocity.magnitude) * drag);
        }

        bool IsVelocityAproximatleyZero(ref Vector2 velocity)
        {
            return Mathf.Abs(velocity.x) < 0.01f && Mathf.Abs(velocity.y) < 0.01f;
        }

        void ApplyExternalInput()
        {
            _deltaExternalForces = Vector2.zero;
            Vector2 velocity;
            for (int i = 0; i < externalForce.Count; i++)
            {
                velocity = externalForce[i];
                _deltaExternalForces += velocity;
                externalForce[i] = DampVelocity(ref velocity);
                if (IsVelocityAproximatleyZero(ref velocity))
                {
                    externalForce.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < externalConstantVelocity.Count; i++)
            {
                _deltaExternalForces += externalConstantVelocity[i];
            }
            for (int i = 0; i < externalVelocity.Count; i++)
            {
                _deltaExternalForces += externalVelocity[i]();
            }
            externalVelocity.Clear();
            for (int i = 0; i < externalPlattformVelocity.Count; i++)
            {
                velocity = externalPlattformVelocity[i]();
                if (_deltaMovement.x != 0)
                    velocity.x = 0;
                _deltaExternalForces += velocity;
            }
            externalPlattformVelocity.Clear();
        }

        //public

        public void AddForce(Vector2 force)
        {
            externalForce.Add(force);
        }

        public void AddConstantVelocity(Vector2 velocity)
        {
            externalConstantVelocity.Add(velocity);
        }

        public delegate Vector2 GetVelocity();
        public void AddVelocity(GetVelocity velocity)
        {
            externalVelocity.Add(velocity);
        }

        public void AddPlattformVelocity(GetVelocity velocity)
        {
            externalPlattformVelocity.Add(velocity);
        }

        public void RemoveConstantVelocity(Vector2 velocity)
        {
            externalConstantVelocity.Remove(velocity);
        }

        public Vector2 Velocity {
            get { return _motor.velocity; }
        }

        //debug

        void OnGUI()
        {
            GUILayout.Label("cState = " + _cState);
            GUILayout.Label("input velocity = " + _deltaMovement);
            GUILayout.Label("external velocity = " + _deltaExternalForces);
            GUILayout.Label("isGrounded = " + _delayedIsGrounded + "( raw = " + _motor.isGrounded + ")");
            GUILayout.Label("shouldJump = " + _isJumpButtonPressed + "( raw = " + Input.GetButton("Jump") + ")");
        }
    }
}
