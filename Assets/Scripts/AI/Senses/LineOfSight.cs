﻿using UnityEngine;
using System.Collections;
using Actors;

public class LineOfSight : MonoBehaviour {
    [SerializeField]
    Transform rayOrigin; // eyes
    [SerializeField]
    float sightRange = 40;

    public bool CanSeeAnEnemy(out BasicEntityActor enemy)
    {
        Actor[] actors = ActorDatabase.GetInstance().Find(typeof(PlayerActor));

        foreach (var a in actors)
        {
            if (Vector2.Distance(a.transform.position, rayOrigin.position) <= sightRange)
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, a.transform.position - rayOrigin.position, sightRange);
                if (hit && hit.collider.gameObject == a.gameObject)
                {
                    enemy = (BasicEntityActor)a;
                    return true;
                }
            }
        }
        enemy = null;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(rayOrigin.position - Vector3.up * sightRange, Vector3.up * sightRange * 2);
        Gizmos.DrawRay(rayOrigin.position - Vector3.right * sightRange, Vector3.right * sightRange * 2);
        DebugExtension.DrawCircle(rayOrigin.position, Vector3.forward, sightRange);
    }
}