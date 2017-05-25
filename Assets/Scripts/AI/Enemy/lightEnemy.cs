﻿using UnityEngine;
using System.Collections;
using Entities;
using Manager;


namespace CC2D
{
    public class lightEnemy : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        [AssignEntityAutomaticly]
        ThightAIMovementEntity actor;
        [SerializeField]
        
        [AssignEntityAutomaticly]
        HealthEntity health;

        [SerializeField]
        [HideInInspector]
        [AssignEntityAutomaticly]
        ActingEntity actingEntity;
        
        [SerializeField]
        Transform player;

        [SerializeField]
        int numLights = 0;

        [SerializeField]
        bool facingRight = true;
        [SerializeField]
        bool checkingLight = false;
        MovementInput bufferedInput;

        [SerializeField]
        bool isAggro = false;
        [SerializeField]
        float radius;
        [SerializeField]
        float range;
        [SerializeField]
        float aggroSpeed;
        [SerializeField]
        float normalSpeed;

        public enum State
        {
            Patrol,
            Aggro,
            Swing,
            FindLight,
            Cower,
            Dead
        }

        public State state = State.Patrol;


        void Start()
        {
            bufferedInput = actor.CC2DThightAIMotor.CurrentMovementInput;

            bufferedInput.horizontalRaw = 1;
            bufferedInput.horizontal = 1;
            health = GetComponentInChildren<HealthEntity>();
        }
        // Update is called once per frame
        void Update()
        {
            #region oldCode
            /* // health 
            if (actor.IHealth.IsDeath)
            {
                if (!resetOnce)
                {
                    actor.CC2DMotor.ResetPlayerMovementInput();
                    resetOnce = true;
                }
                return;
            }
            */

            /* random jump/move

            if (runRight)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0 || actor.CharacterController2D.collisionState.right)
                {
                    timeLeft = timeRunLeft + Random.Range(-deviation, deviation);
                    runRight = false;
                }
                bufferedInput.horizontalRaw = 1;
                bufferedInput.horizontal = 1;
                if (actor.CharacterController2D.collisionState.right || Random.Range(0, randomJumpChance) == 0)
                {
                    bufferedInput.AddEvent(new JumpEvent(maxJumpExecutionDelay));
                }
            }
            else
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0 || actor.CharacterController2D.collisionState.left)
                {
                    timeLeft = timeRunRight + Random.Range(-deviation, deviation);
                    runRight = true;
                }
                bufferedInput.horizontalRaw = -1;
                bufferedInput.horizontal = -1;
                if (actor.CharacterController2D.collisionState.left || Random.Range(0, randomJumpChance) == 0)
                {
                    bufferedInput.AddEvent(new JumpEvent(maxJumpExecutionDelay));
                }
            }

            */

            #endregion

            isAggro = false;

            Collider2D[] overlap = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D coll in overlap)
            {
                if (coll.gameObject.CompareTag("Player"))
                {
                    isAggro = true;
                    player = coll.gameObject.transform;
                }
            }

            if (health.IHealth.IsDeath)
                state = State.Dead;

            switch (state)
            {
                case State.Patrol:

                    actor.CC2DThightAIMotor.MaxWalkSpeed = normalSpeed;

                    checkFlip();
                    checkLights();

                    if (isAggro)
                    {
                        StartAggro();
                        break;
                    }

                    break;

                case State.Aggro:                                                            

                    checkFlip();
                    checkLights();

                    if (!isAggro)
                    {
                        state = State.Patrol;
                    }
                    
                    actor.CC2DThightAIMotor.MaxWalkSpeed = aggroSpeed;

                    if (Mathf.Abs(calcDist()) <= range)
                    {
                        StartSwing();
                    }
                    else if (calcDist() > 0)
                    {
                        if(!facingRight)
                        {
                            Flip();
                        }
                    }
                    else if (calcDist() < 0)
                    {
                        if (facingRight)
                        {
                            Flip();
                        }
                    }

                    break;

                case State.Swing:

                    //bufferedInput.

                    break;


                case State.FindLight:

                    break;

                case State.Cower:
                    actor.CC2DThightAIMotor.MaxWalkSpeed = ((actor.CC2DThightAIMotor.MaxWalkSpeed) * 0f);
                    break;

                case State.Dead:

                    bufferedInput.horizontalRaw = 0;
                    bufferedInput.horizontal = 0;
                    break;
            }
        }

        void checkFlip()
        {

            if (facingRight)
            {
                if (actor.CharacterController2D.collisionState.right)
                {
                    Flip();
                }
            }
            else {
                if (actor.CharacterController2D.collisionState.left)
                {
                    Flip();
                }
            }
        }

        void checkLights()
        {
            if (numLights == 0 && !checkingLight)
            {
                startFindLight();
            }
        }

        float calcDist()
        {
            return (player.transform.position.x - this.transform.position.x);
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Light")
            {
                numLights++;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Light")
            {
                numLights--;
            }
        }

        void startFindLight()
        {
            checkingLight = true;
            StartCoroutine(tryFlip()); // flips enemy, checks if in light, else goto findlight
        }

        void StartAggro()
        {
            Debug.Log("startAggro");
            state = State.Aggro;
            actor.CC2DThightAIMotor.MaxWalkSpeed = ((actor.CC2DThightAIMotor.MaxWalkSpeed) * 1.2f);
        }

        void StopAggro()
        {
            state = State.Patrol;
            actor.CC2DThightAIMotor.MaxWalkSpeed = ((actor.CC2DThightAIMotor.MaxWalkSpeed) / 1.2f);
        }

        void StartSwing()
        {
            state = State.Swing;

            //bufferedInput.()
        }


        public void Flip()
        {
            facingRight = !facingRight;

            bufferedInput.horizontalRaw *= -1;
            bufferedInput.horizontal *= -1;

            Vector3 scale = this.transform.localScale;
            scale.x *= -1;

            this.transform.localScale = scale;
        }

        IEnumerator tryFlip()
        {
            Flip();
            yield return new WaitForSeconds(0.5f);
            checkingLight = false;
            if (numLights == 0)
            {
                state = State.Cower;
            }
            else
            {
                state = State.Patrol;
            }
       }
    }
}