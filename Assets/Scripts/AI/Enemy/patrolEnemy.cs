using UnityEngine;
using System.Collections;
using Entities;
using Manager;


namespace CC2D
{

    public class patrolEnemy : MonoBehaviour
    {

        [SerializeField, HideInInspector, AssignEntityAutomaticly]
        ThightAIMovementEntity actor;

        [SerializeField, AssignEntityAutomaticly]
        HealthEntity health;

        [SerializeField, HideInInspector, AssignEntityAutomaticly]
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

        bool isSwinging;

        public enum State
        {
            Patrol,
            Aggro,
            Stare,
            Swing,
            FindLight,
            Cower,
            Dead
        }

        public State state = State.Patrol;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}