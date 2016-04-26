using UnityEngine;
using System.Collections.Generic;
using Prime31;
using System;

public class CharacterInput : MonoBehaviour, ICharacterControllerInput2D
{
    [Header("Speed on ground:")]
    [SerializeField]
    float horizontalSpeed;
    [SerializeField]
    float horizontalAcceleration;
    [SerializeField]
    float verticalSpeed;
    [SerializeField]
    float verticalAcceleration;
    [Header("Speed in air:")]
    [SerializeField]
    float airSpeed;
    [SerializeField]
    float airAcceleration;
    [Header("Jumping:")]
    [SerializeField]
    float gravity;
    [SerializeField]
    float jumpSpeed;
    [SerializeField]
    int maxJumpExecutionDelay;
    [SerializeField]
    int transToFallWaitFrames;
    [SerializeField]
    public MovementRestrictions movementRestrictions;

    //External reference
    private CharacterController2D charController;

    //State vars
    private Vector2 velocity;
    private bool isJumpPressed;
    private bool isGrounded;

    //Current handling vars
    private float _gravity;
    private float _horizontalSpeed;
    private float _horizontalAcceleration;
    private float _verticalSpeed;
    private float _verticalAcceleration;
    private float _airSpeed;
    private float _airAcceleration;
    private float _jumpSpeed;

    //Jumpqueque
    private int framesSinceJumpRequest;
    private bool isJumpConsumed;

    //Is Grounded
    private int framesSinceLastGrounded;
    private bool oldIsGrounded;

    //External Forces
    public delegate Vector2 GetForce();
    private List<Vector2> externalConstantForces;
    private List<GetForce> externalForces;
    private List<GetForce> externalRelativeForces;

    void Awake()
    {
        charController = GetComponent<CharacterController2D>();
        charController.onControllerCollidedEvent += CharController_onControllerCollidedEvent;
        externalConstantForces = new List<Vector2>(2);
        externalForces = new List<GetForce>(2);
        externalRelativeForces = new List<GetForce>(2);
        ResetMovementVars();
    }

    private void CharController_onControllerCollidedEvent(RaycastHit2D obj)
    {
        obj.collider.SendMessage("OnFakeCollisionStay2D", this, SendMessageOptions.DontRequireReceiver);
    }

    void FixedUpdate()
    {
        velocity = charController.velocity;
        HandleInput();
        if (movementRestrictions.up || movementRestrictions.down)
        {
            MoveHorizontal(_horizontalAcceleration, _horizontalSpeed);
            MoveVertical(_verticalAcceleration, _verticalSpeed);
        }
        else
        {
            velocity.y += _gravity;
            if (isGrounded)
            {
                MoveHorizontal(_horizontalAcceleration, _horizontalSpeed);
                if (isJumpPressed)
                {
                    if (movementRestrictions.canJump)
                    {
                        velocity.y = _jumpSpeed;
                        isGrounded = false;
                    }
                    isJumpConsumed = true;
                }
            }
            else
                MoveHorizontal(_horizontalAcceleration, _horizontalSpeed);
        }
        ApplyExternalRelativeForces();
        ApplyExternalConstantForces();
        ApplyExternalForces();
        charController.move(velocity * Time.fixedDeltaTime);
    }

    void HandleInput()
    {
        if (charController.isGrounded)
        {
            isGrounded = true;
            oldIsGrounded = true;
        }
        else if (!isGrounded)
        {
            if (oldIsGrounded)
            {
                framesSinceLastGrounded = 0;
                oldIsGrounded = false;
            }
            else if (framesSinceLastGrounded >= transToFallWaitFrames)
                isGrounded = false;
            else
                framesSinceLastGrounded++;
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

    void MoveVertical(float acceleration, float maxSpeed)
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        if (verticalAxis == 0)
        {
            velocity.y = 0;
            return;
        }
        if (verticalAxis > 0)
        {
            if (!movementRestrictions.up)
            {
                velocity.y = 0;
                return;
            }
            if (velocity.y < 0)
                velocity.y = 0;
        }
        else
        {
            if (!movementRestrictions.down)
            {
                velocity.y = 0;
                return;
            }
            if (velocity.y > 0)
                velocity.y = 0;
        }
        velocity.y = Mathf.Clamp(velocity.y + acceleration * verticalAxis, -maxSpeed, maxSpeed);
    }

    void MoveHorizontal(float acceleration, float maxSpeed)
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (horizontalAxis == 0)
        {
            velocity.x = 0;
            return;
        }
        if (horizontalAxis > 0)
        {
            if (!movementRestrictions.right)
            {
                velocity.x = 0;
                return;
            }
            if (velocity.x < 0)
                velocity.x = 0;
        }
        else
        {
            if (!movementRestrictions.left)
            {
                velocity.x = 0;
                return;
            }
            if (velocity.x > 0)
                velocity.x = 0;
        }
        velocity.x = Mathf.Clamp(velocity.x + acceleration * horizontalAxis, -maxSpeed, maxSpeed);
    }

    void ApplyExternalConstantForces()
    {
        for (int i = 0; i < externalConstantForces.Count; i++)
        {
            velocity += externalConstantForces[i];
        }
    }

    void ApplyExternalForces()
    {
        for (int i = 0; i < externalForces.Count; i++)
        {
            velocity += externalForces[i]();
        }
        externalForces.Clear();
    }

    void ApplyExternalRelativeForces()
    {
        for (int i = 0; i < externalRelativeForces.Count; i++)
        {
            Vector2 force = externalRelativeForces[i]();
            if ((velocity.x != 0 ))//&& force.x < 0) || (velocity.x < 0 && force.x > 0))
                force.x = 0;
            if (movementRestrictions.up || movementRestrictions.down)
            {
                if ((velocity.y != 0))// && force.y < 0) || (velocity.y < 0 && force.y > 0))
                    force.y = 0;
            }
            velocity += force;
        }
        externalRelativeForces.Clear();
    }

    public void SetNormalMovementVars(float gravity, float horizontalSpeed, float horizontalAcceleration, float verticalSpeed, float verticalAcceleration, float airSpeed, float airAcceleration, float jumpSpeed)
    {
        _gravity = gravity;
        _horizontalSpeed = horizontalSpeed;
        _horizontalAcceleration = horizontalAcceleration;
        _verticalSpeed = verticalSpeed;
        _verticalAcceleration = verticalAcceleration;
        _airSpeed = airSpeed;
        _airAcceleration = airAcceleration;
        _jumpSpeed = jumpSpeed;
    }

    public void ResetMovementVars()
    {
        _gravity = gravity;
        _horizontalSpeed = horizontalSpeed;
        _horizontalAcceleration = horizontalAcceleration;
        _verticalSpeed = verticalSpeed;
        _verticalAcceleration = verticalAcceleration;
        _airSpeed = airSpeed;
        _airAcceleration = airAcceleration;
        _jumpSpeed = jumpSpeed;
    }

    public void ResetRestrictions()
    {
        movementRestrictions.up = false;
        movementRestrictions.down = false;
        movementRestrictions.left = true;
        movementRestrictions.right = true;
        movementRestrictions.canJump = true;
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

    public void RemoveConstantForce(Vector2 force)
    {
        externalConstantForces.Remove(force);
    }

    void OnGUI()
    {
        GUILayout.Label("velocity = " + velocity);
        GUILayout.Label("isGrounded = " + isGrounded + "(" + charController.isGrounded + ")");
        GUILayout.Label("shouldJump = " + isJumpPressed + "(" + isJumpConsumed + ")");
    }

    [System.Serializable]
    public class MovementRestrictions
    {
        [SerializeField]
        public bool right;
        [SerializeField]
        public bool left;
        [SerializeField]
        public bool up;
        [SerializeField]
        public bool down;
        [SerializeField]
        public bool canJump;
    }
}
