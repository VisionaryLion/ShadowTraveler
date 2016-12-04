#define DEBUG

using UnityEngine;
using Actors;
using System.Collections;
using System;

public class SimpleStateMaschineBehaviour : MonoBehaviour {

    [SerializeField, HideInInspector, AssignActorAutomaticly]
    StateMaschineAIActor actor;

    [SerializeField]
    PositionHolder2D patrolPoints;

    enum BrainState
    {
        Normal,
        Flee,
        Stunt,
        Chase,
        Wait,
        Search,
        Attack
    }

    [SerializeField, ReadOnly]
    BrainState brainState = BrainState.Normal;

    int currentPatrolPoint = 0;
    BasicEntityActor currentlyChased;

    void Start()
    {
        actor.NavAgent.SetDestination(patrolPoints.positions[currentPatrolPoint], actor.LightSensor.LightSkin, OnPathComputationFinished);
    }

	// Update is called once per frame
	void Update () {
        switch (brainState)
        {
            case BrainState.Normal:
                if (!actor.NavAgent.IsFollowingAPath && patrolPoints.positions.Count > 1)
                {
                    currentPatrolPoint++;
                    if (currentPatrolPoint >= patrolPoints.positions.Count)
                        currentPatrolPoint = 0;
                    actor.NavAgent.SetDestination(patrolPoints.positions[currentPatrolPoint], actor.LightSensor.LightSkin, OnPathComputationFinished);
                }
                if (actor.LineOfSight.CanSeeAnEnemy(out currentlyChased))
                {
                    brainState = BrainState.Chase;
                }
                break;
            case BrainState.Chase:
                BasicEntityActor tmpChased;
                if (!actor.LineOfSight.CanSeeAnEnemy(out tmpChased))
                {
                    brainState = BrainState.Search;
                    actor.NavAgent.UpdateDestination(currentlyChased.transform.position, actor.LightSensor.LightSkin, OnPathComputationFinished);
                    return;
                }
                currentlyChased = tmpChased;
                actor.NavAgent.UpdateDestination(currentlyChased.transform.position, actor.LightSensor.LightSkin, OnPathComputationFinished);
                if (Vector2.Distance(currentlyChased.transform.position, transform.position) < 1)
                {
                    brainState = BrainState.Attack;
                }
                break;
            case BrainState.Search:
                if (actor.LineOfSight.CanSeeAnEnemy(out currentlyChased))
                {
                    brainState = BrainState.Chase;
                }
                if (!actor.NavAgent.IsFollowingAPath)
                    brainState = BrainState.Normal;
                break;
            case BrainState.Attack:
                if (Vector2.Distance(currentlyChased.transform.position, transform.position) > 1)
                {
                    brainState = BrainState.Chase;
                }
                break;
        }
	}

    private void OnPathComputationFinished(bool foundPath)
    {

    }
}
