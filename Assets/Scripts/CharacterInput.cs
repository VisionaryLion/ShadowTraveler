﻿using UnityEngine;
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
    [SerializeField]
    public MovementRestrictions movementRestrictions;
    [Header("Update values:")]
    [SerializeField]
    bool shouldUpdateValues;

    //External reference
    private CharacterController2D charController;

    //State vars
    private Vector2 inputVelocity;
    private Vector2 externalVelocity;
    private bool isJumpPressed;
    private bool isGrounded;
    private float horizontalAxis;
    private float verticalAxis;

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
    private bool isJumping;
    private float jumpedFrames;

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
        if (shouldUpdateValues)
            ResetMovementVars();
        inputVelocity = charController.velocity;
        HandleInput();
        if (movementRestrictions.up || movementRestrictions.down)
        {
            MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
            MoveVertical(ref _verticalAcceleration, ref _verticalSpeed);
        }
        else
        {
            if (isGrounded)
            {
                isJumping = false;
                MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
                if (isJumpPressed)
                {
                    if (movementRestrictions.canJump)
                    {
                        inputVelocity.y = _jumpSpeed;
                        isJumping = true;
                        jumpedFrames = 0;
                        isGrounded = false;
                    }
                    isJumpConsumed = true;
                }
                else
                    inputVelocity.y = _gravity;
            }
            else
            {
                MoveHorizontal(ref _horizontalAcceleration, ref _horizontalSpeed);
                if (isJumping)
                {
                    jumpedFrames++;
                    if (!Input.GetButton("Jump") && jumpedFrames >= minJumpFrames)
                    {
                        inputVelocity.y = jumpCutSpeed;
                        isJumping = false;
                    }
                }
                if (inputVelocity.y < 0)
                    isJumping = false;
                inputVelocity.y += _gravity;
            }
        }
        PostProccessVelocitys();
        if (externalVelocity.y != 0)
            isJumping = false;
        charController.move((inputVelocity + externalVelocity) * Time.fixedDeltaTime);
    }

    void HandleInput()
    {
        if (charController.isGrounded)
        {
            isGrounded = true;
            oldIsGrounded = true;
        }
        else
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

    void MoveVertical(ref float acceleration, ref float maxSpeed)
    {
        verticalAxis = Input.GetAxisRaw("Vertical");
        if (verticalAxis == 0)
        {
            inputVelocity.y = 0;
            return;
        }
        if (verticalAxis > 0)
        {
            if (!movementRestrictions.up)
            {
                inputVelocity.y = 0;
                return;
            }
            if (inputVelocity.y < 0)
                inputVelocity.y = 0;
        }
        else
        {
            if (!movementRestrictions.down)
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
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        
        if (horizontalAxis == 0)
        {
            inputVelocity.x = 0;
            return;
        }
        if (horizontalAxis > 0)
        {
            if (!movementRestrictions.right)
            {
                inputVelocity.x = 0;
                return;
            }
            if (inputVelocity.x < 0)
                inputVelocity.x = 0;
        }
        else
        {
            if (!movementRestrictions.left)
            {
                inputVelocity.x = 0;
                return;
            }
            if (inputVelocity.x > 0)
                inputVelocity.x = 0;
        }

        inputVelocity.x = Mathf.Clamp(inputVelocity.x + acceleration * horizontalAxis, -maxSpeed, maxSpeed);
    }

    void PostProccessVelocitys ()
    {
        ApplyExternalConstantForces();
        ApplyExternalForces();
        ApplyExternalRelativeForces();
        if (charController.isGrounded)
        {
            ApplyDamping(ref externalVelocity, ref _friction);
        }
        else
        {
            ApplyDamping(ref externalVelocity, ref _drag);
        }

    }

    void ApplyExternalConstantForces()
    {
        for (int i = 0; i < externalConstantForces.Count; i++)
        {
            externalVelocity += externalConstantForces[i];
        }
    }

    void ApplyExternalForces()
    {
        for (int i = 0; i < externalForces.Count; i++)
        {
            externalVelocity += externalForces[i]();
        }
        externalForces.Clear();
    }

    void ApplyExternalRelativeForces()
    {
        for (int i = 0; i < externalRelativeForces.Count; i++)
        {
            Vector2 force = externalRelativeForces[i]();
            if (inputVelocity.x != 0 )//&& force.x < 0) || (velocity.x < 0 && force.x > 0))
                force.x = 0;
            if (movementRestrictions.up || movementRestrictions.down)
            {
                if ((inputVelocity.y != 0))// && force.y < 0) || (velocity.y < 0 && force.y > 0))
                    force.y = 0;
            }
            inputVelocity += force;
        }
        externalRelativeForces.Clear();
    }

    void ApplyDamping(ref Vector2 velocity, ref float damp)
    {
        velocity = velocity * (1 - Time.fixedDeltaTime * damp);
        //velocity += -velocity.normalized * (Mathf.Sqrt(velocity.magnitude) * drag);
    }

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
        GUILayout.Label("input velocity = " + inputVelocity);
        GUILayout.Label("external velocity = " + externalVelocity);
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
