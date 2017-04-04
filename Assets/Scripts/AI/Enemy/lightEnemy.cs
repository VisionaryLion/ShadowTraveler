﻿using UnityEngine;
using System.Collections;
using Entities;


namespace CC2D
{
    public class lightEnemy : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        [AssignEntityAutomaticly]
        ThightAIMovementEntity actor;

        [SerializeField]
        int numLights = 0;

        [SerializeField]
        bool facingRight = true;
        [SerializeField]
        bool checkingLight = false;
        MovementInput bufferedInput;

        public enum State
        {
            Patrol,
            Aggro,
            FindLight,
            Cower
        }

        public State state = State.Patrol;


        void Start()
        {
            bufferedInput = actor.CC2DThightAIMotor.CurrentMovementInput;

            bufferedInput.horizontalRaw = 1;
            bufferedInput.horizontal = 1;
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

            switch (state)
            {
                case State.Patrol:

                    if (numLights == 0 && !checkingLight)
                    {                        
                        startFindLight();
                        break;                  
                    }
                    
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

                    break;

                case State.Aggro:

                    break;

                case State.FindLight:
                    
                    break;

                case State.Cower:

                    break;
            }
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

        void Flip()
        {
            Debug.Log("flip");
            facingRight = !facingRight;
            bufferedInput.horizontal *= -1;
            bufferedInput.horizontalRaw *= -1;
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