using UnityEngine;
using System.Collections.Generic;
using Prime31;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController2D))]
public class CharacterInput : MonoBehaviour, ICharacterControllerInput2D
{
    [Header("External References:")]
    [SerializeField]
    Transform spriteRoot;
    [Header("Speed on ground:")]
    [SerializeField]
    float horizontalSpeed;
    [SerializeField]
    float horizontalAcceleration;
    [SerializeField]
    float verticalSpeed;
    [SerializeField]
    float verticalAcceleration;
    [SerializeField]
    float friction;
    [Header("Speed in air:")]
    [SerializeField]
    float airSpeed;
    [SerializeField]
    float airAcceleration;
    [SerializeField]
    float drag;
    [Header("Jumping:")]
    [SerializeField]
    float gravity;
    [SerializeField]
    float jumpSpeed;
    [SerializeField]
    float jumpCutSpeed;
    [SerializeField]
    int minJumpFrames;
    [SerializeField]
    int maxJumpExecutionDelay;
    [SerializeField]
    int transToFallWaitFrames;
    [Header("WallJumping:")]
    [SerializeField]
    Vector2 wallJumpForce;
    [SerializeField]
    int wallJumpNoControllFrames;
    [SerializeField]
    float wallSlidingSpeed;

    [SerializeField]
    public MovementRestrictions movementRestrictions;
    [Header("Update values:")]
    [SerializeField]
    bool shouldUpdateValues;

    //External reference
    private CharacterController2D charController;
    private Animator animator;

    //State vars
    private Vector2 inputVelocity;
    private Vector2 externalVelocity;
    private bool isJumpPressed;
    private bool isGrounded;
    private float horizontalAxis;
    private float verticalAxis;
    private bool isTurnedLeft;
    private bool isWallSliding;
    private bool shouldUseWallSlidingSpeed;
    private bool isJumping;
    private Coroutine wallJump;

    //Current handling vars
    [HideInInspector]
    public float _gravity;
    [HideInInspector]
    public float _horizontalSpeed;
    [HideInInspector]
    public float _horizontalAcceleration;
    [HideInInspector]
    public float _verticalSpeed;
    [HideInInspector]
    public float _verticalAcceleration;
    [HideInInspector]
    public float _friction;
    [HideInInspector]
    public float _airSpeed;
    [HideInInspector]
    public float _airAcceleration;
    [HideInInspector]
    public float _drag;
    [HideInInspector]
    public float _jumpSpeed;

    //Jumpqueque
    private int framesSinceJumpRequest;
    private bool isJumpConsumed;

    //Is Grounded
    private Coroutine lateIsGrounded;
    private bool oldIsGrounded;

    //External Forces
    public delegate Vector2 GetForce();
    private List<Vector2> externalConstantForces;
    private List<GetForce> externalForces;
    private List<GetForce> externalRelativeForces;
    private List<OutFadingForce> externalOutFadingForces;

    void Awake()
    {
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController2D>();
        charController.onControllerCollidedEvent += CharController_onControllerCollidedEvent;
        externalConstantForces = new List<Vector2>(2);
        externalForces = new List<GetForce>(2);
        externalRelativeForces = new List<GetForce>(2);
        externalOutFadingForces = new List<OutFadingForce>(2);
        ResetMovementVars();
        isTurnedLeft = false;
        if (spriteRoot.Equals(this))
            Debug.LogError("The spriteRoot an script should never be attached to the same GameObject");
    }

    private void CharController_onControllerCollidedEvent(RaycastHit2D obj)
    {
        obj.collider.SendMessage("OnFakeCollisionStay2D", this, SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        HandleInput();
    }

    private float wallJumpFrames;
    void FixedUpdate()
    {
        if (shouldUpdateValues)
            ResetMovementVars();

        inputVelocity = Vector2.zero;
        externalVelocity = Vector2.zero;
        if (movementRestrictions.CanMoveVertical)
        {
            MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
            MoveVertical(ref _verticalAcceleration, ref _verticalSpeed);
        }
        else
        {
            //Apply gravity
            if (!charController.isGrounded)
                inputVelocity.y += _gravity;
            else
                inputVelocity.y = _gravity;

            if (isGrounded)
            {
                MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
                if (isJumpPressed)
                {
                    //Jump
                    if (movementRestrictions.CanJump)
                        StartCoroutine(AnalogJump());
                    isJumpConsumed = true;
                }
            }
            else
            {
                CheckForSideCollisions();
                if (isWallSliding)
                {
                    if (isJumpPressed)
                        StartCoroutine(WallJump());
                    else if (shouldUseWallSlidingSpeed && !isJumping)
                        inputVelocity.y = wallSlidingSpeed;
                }
                MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
            }
        }
        ApplyExternalForces();
        charController.move((inputVelocity + externalVelocity) * Time.fixedDeltaTime, false);
        UpdateAnimatorVars();
        SetFace();
    }

    void UpdateAnimatorVars()
    {
        animator.SetFloat("VelocityX", inputVelocity.x);
        animator.SetFloat("VelocityY", inputVelocity.y);
        animator.SetBool("IsGrounded", charController.isGrounded);
        animator.SetFloat("AbsX", Mathf.Abs(inputVelocity.x));
        animator.SetFloat("AbsY", Mathf.Abs(inputVelocity.y));
        animator.SetBool("HasMoveInput", inputVelocity != Vector2.zero);
    }

    void SetFace()
    {
        if ((inputVelocity.x < 0 && !isTurnedLeft) || (inputVelocity.x > 0 && isTurnedLeft))
        {
            spriteRoot.localScale = new Vector3(-spriteRoot.localScale.x, spriteRoot.localScale.y, spriteRoot.localScale.z);
            isTurnedLeft = !isTurnedLeft;
        }
    }

    private const float wallJumpCollisionDistance = 0.01f;

    void CheckForSideCollisions()
    {
        if (charController.velocity.x == 0)
        {
            charController.manuallyCheckForCollisions(wallJumpCollisionDistance);
            charController.manuallyCheckForCollisions(-wallJumpCollisionDistance);
        }
        else if (charController.velocity.x > 0)
            charController.manuallyCheckForCollisions(-wallJumpCollisionDistance);
        else
            charController.manuallyCheckForCollisions(wallJumpCollisionDistance);
        if (charController.collisionState.left || charController.collisionState.right)
            isWallSliding = true;
        else
            isWallSliding = false;
    }

    void HandleInput()
    {
        if (charController.isGrounded)
        {
            if (!isGrounded)
            {
                if (wallJump != null)
                {
                    StopCoroutine(wallJump);
                    movementRestrictions.CanMoveHorizontal = true;
                }
                if (lateIsGrounded != null)
                {
                    StopCoroutine(lateIsGrounded);
                    isJumping = false;
                    wallJumpFrames = wallJumpNoControllFrames;
                }
            }
            isGrounded = true;
            oldIsGrounded = true;
        }
        else if (oldIsGrounded)
        {
            oldIsGrounded = false;
            lateIsGrounded = StartCoroutine(LateIsGrounded());
        }
        if (Input.GetButtonDown("Jump"))
        {
            isJumpConsumed = false;
            framesSinceJumpRequest = 0;
        }
        else if (Input.GetButton("Jump"))
            framesSinceJumpRequest = 0;
        else if (!isJumpConsumed)
            framesSinceJumpRequest++;
        isJumpPressed = !isJumpConsumed && framesSinceJumpRequest <= maxJumpExecutionDelay;
    }

    void MoveVertical(ref float acceleration, ref float maxSpeed)
    {
        if (!movementRestrictions.CanMoveVertical)
            return;
        verticalAxis = Input.GetAxisRaw("Vertical");
        if (verticalAxis == 0)
        {
            inputVelocity.y = 0;
            return;
        }
        if (verticalAxis > 0)
        {
            if (!movementRestrictions.CanMoveUp)
            {
                inputVelocity.y = 0;
                return;
            }
            if (inputVelocity.y < 0)
                inputVelocity.y = 0;
        }
        else
        {
            if (!movementRestrictions.CanMoveDown)
            {
                inputVelocity.y = 0;
                return;
            }
            if (inputVelocity.y > 0)
                inputVelocity.y = 0;
        }

        inputVelocity.y = Mathf.Clamp(inputVelocity.y + acceleration * verticalAxis, -maxSpeed, maxSpeed);
    }

    void MoveHorizontal(ref float acceleration, ref float maxSpeed)
    {
        if (!movementRestrictions.CanMoveHorizontal)
            return;

        horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (horizontalAxis == 0)
        {
            inputVelocity.x = 0;
            return;
        }
        if (horizontalAxis > 0)
        {
            if (!movementRestrictions.CanMoveRight)
            {
                inputVelocity.x = 0;
                return;
            }
            if (inputVelocity.x < 0)
                inputVelocity.x = 0;
        }
        else
        {
            if (!movementRestrictions.CanMoveLeft)
            {
                inputVelocity.x = 0;
                return;
            }
            if (inputVelocity.x > 0)
                inputVelocity.x = 0;
        }

        inputVelocity.x = Mathf.Clamp(inputVelocity.x + acceleration * horizontalAxis, -maxSpeed, maxSpeed);
    }

    void ApplyExternalForces()
    {
        //normal forces
        for (int i = 0; i < externalForces.Count; i++)
        {
            externalVelocity += externalForces[i]();
        }
        externalForces.Clear();

        //relativ forces
        for (int i = 0; i < externalRelativeForces.Count; i++)
        {
            Vector2 force = externalRelativeForces[i]();
            if (inputVelocity.x != 0)
                force.x = 0;
            if (movementRestrictions.CanMoveVertical)
            {
                if (inputVelocity.y != 0)
                    force.y = 0;
            }
            externalVelocity += force;
        }
        externalRelativeForces.Clear();

        //constant forces
        for (int i = 0; i < externalConstantForces.Count; i++)
        {
            externalVelocity += externalConstantForces[i];
        }

        //outfading forces
        for (int i = 0; i < externalOutFadingForces.Count; i++)
        {
            externalVelocity += externalOutFadingForces[i].force;
            externalOutFadingForces[i].DampForce(charController.isGrounded);
            if (externalOutFadingForces[i].IsForceNull())
            {
                externalOutFadingForces.RemoveAt(i);
                i--;
            }
        }
    }

    private float jumpedFrames;
    IEnumerator AnalogJump()
    {
        jumpedFrames = 1;
        isJumping = true;
        isGrounded = false;
        AddOutFadingForce(new OutFadingForce(Vector2.up * _jumpSpeed, drag, float.MaxValue));
        yield return new WaitForFixedUpdate();
        while (!charController.isGrounded && jumpedFrames < minJumpFrames)
        {
            yield return new WaitForFixedUpdate();
            jumpedFrames++;
        }
        while (!charController.isGrounded && inputVelocity.y > jumpCutSpeed)
        {
            if (!Input.GetButton("Jump"))
            {
                charController.velocity.y = jumpCutSpeed;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        isJumping = false;
    }

    private int framesSinceLastGrounded;
    IEnumerator LateIsGrounded()
    {
        framesSinceLastGrounded = 0;
        while (framesSinceLastGrounded < transToFallWaitFrames)
        {
            yield return new WaitForFixedUpdate();
            framesSinceLastGrounded++;
        }
        isGrounded = false;
    }

    IEnumerator WallJump()
    {
        isJumpConsumed = true;
        wallJumpFrames = 0;
        shouldUseWallSlidingSpeed = false;
        Vector2 wallJumpVelocity = wallJumpForce;
        if (charController.collisionState.right)
        {
            wallJumpVelocity.x = -wallJumpVelocity.x;
        }
        AddOutFadingForce(new OutFadingForce(wallJumpVelocity, drag, float.MaxValue));
        movementRestrictions.CanMoveHorizontal = false;
        while (wallJumpFrames < wallJumpNoControllFrames)
        {
            yield return new WaitForFixedUpdate();
            wallJumpFrames++;
        }
        movementRestrictions.CanMoveHorizontal = true;
        shouldUseWallSlidingSpeed = true;
    }

    //public

    public void ResetMovementVars()
    {
        _gravity = gravity;
        _horizontalSpeed = horizontalSpeed;
        _horizontalAcceleration = horizontalAcceleration;
        _verticalSpeed = verticalSpeed;
        _verticalAcceleration = verticalAcceleration;
        _friction = friction;
        _airSpeed = airSpeed;
        _airAcceleration = airAcceleration;
        _drag = drag;
        _jumpSpeed = jumpSpeed;
    }

    public void AddConstantForce(Vector2 force)
    {
        externalConstantForces.Add(force);
    }

    public void AddForce(GetForce force)
    {
        externalForces.Add(force);
    }

    public void AddRelativeForce(GetForce force)
    {
        externalRelativeForces.Add(force);
    }

    public void AddOutFadingForce(OutFadingForce force)
    {
        externalOutFadingForces.Add(force);
    }

    public void RemoveConstantForce(Vector2 force)
    {
        externalConstantForces.Remove(force);
    }

    //debug

    void OnGUI()
    {
        GUILayout.Label("input velocity = " + inputVelocity);
        GUILayout.Label("external velocity = " + externalVelocity);
        GUILayout.Label("isGrounded = " + isGrounded + "( raw = " + charController.isGrounded + ")");
        GUILayout.Label("shouldJump = " + isJumpPressed + "( consumed = " + isJumpConsumed + ")");
        GUILayout.Label("isWallSliding = " + isWallSliding);
    }

    [System.Serializable]
    public class MovementRestrictions
    {
        public bool CanMoveRight
        {
            get { return _right; }
            set
            {
                if (value)
                    _horizontal = true;
                _right = value;
            }
        }
        public bool CanMoveLeft
        {
            get { return _left; }
            set
            {
                if (value)
                    _horizontal = true;
                _left = value;
            }
        }
        public bool CanMoveUp
        {
            get { return _up; }
            set
            {
                if (value)
                {
                    _vertical = true;
                    _jump = false;
                }
                _up = value;
            }
        }
        public bool CanMoveDown
        {
            get { return _down; }
            set
            {
                if (value)
                {
                    _vertical = true;
                    _jump = false;
                }
                _down = value;
            }
        }
        public bool CanJump
        {
            get { return _jump; }
            set
            {
                if (value)
                {
                    _up = false;
                    _down = false;
                    _vertical = false;
                }
                _jump = value;
            }
        }
        public bool CanMoveVertical
        {
            get { return _vertical; }
            set
            {
                _up = value;
                _down = value;
                _vertical = value;
            }
        }
        public bool CanMoveHorizontal
        {
            get { return _horizontal; }
            set
            {
                _right = value;
                _left = value;
                _horizontal = value;
            }
        }

        [SerializeField]
        private bool _right;
        [SerializeField]
        private bool _left;
        [SerializeField]
        private bool _up;
        [SerializeField]
        private bool _down;
        [SerializeField]
        private bool _jump;
        [SerializeField]
        private bool _horizontal;
        [SerializeField]
        private bool _vertical;

        public void Reset()
        {
            _right = true;
            _left = true;
            _up = false;
            _down = false;
            _jump = true;
            _horizontal = true;
            _vertical = false;
        }
    }

    public class OutFadingForce
    {
        public float drag;
        public float friction;
        public Vector2 force;

        public OutFadingForce(Vector2 force, float drag, float friction)
        {
            this.drag = drag;
            this.force = force;
            this.friction = friction;
        }

        public void DampForce(bool isGrounded)
        {
            if(isGrounded)
                force = force * (1 - Time.fixedDeltaTime * friction);
            else
            force = force * (1 - Time.fixedDeltaTime * drag);
            //velocity += -velocity.normalized * (Mathf.Sqrt(velocity.magnitude) * drag);
        }

        public bool IsForceNull()
        {
            return force.x < 0.01f && force.y < 0.01f;
        }
    }
}
