using UnityEngine;
using Entity;
using AI.Brain;
using System.Collections.Generic;

namespace AI.Sensor
{
    public class LineOfSight : MonoBehaviour, ISensor
    {
        [SerializeField, AssignEntityAutomaticly, HideInInspector]
        ActingEntity entity;

        [SerializeField]
        Transform rayOrigin; // eyes
        [SerializeField]
        float sightRange = 40;
        

        EntityDatabase actorDatabase;

        void Start()
        {
            actorDatabase = EntityDatabase.GetInstance();
        }

        public void UpdateBlackboard(Blackboard board)
        {
            board.visibleHostileEntities = GetVisibleActors(RelationshipType.Hostile);
        }

        public Blackboard.OtherEntity[] GetVisibleActors(RelationshipType requiredRel)
        {
            List<Blackboard.OtherEntity> inSight = new List<Blackboard.OtherEntity>(1); 
            ActingEntity[] allActors = actorDatabase.Find<ActingEntity>();
            float sightRangeSqared = sightRange * sightRange;

            foreach (var actor in allActors)
            {
                if (entity.RelationshipMarker.GetRelationship(actor) != RelationshipType.Hostile)
                    continue;
                if ((actor.transform.position - rayOrigin.position).sqrMagnitude + 1 > sightRangeSqared)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, actor.transform.position - rayOrigin.position, sightRange);
                if (hit && hit.collider.gameObject == actor.gameObject)
                {
                    inSight.Add(new Blackboard.OtherEntity(actor, Vector2.Distance(rayOrigin.position, entity.transform.position)));
                }
            }

            return inSight.ToArray();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(rayOrigin.position - Vector3.up * sightRange, Vector3.up * sightRange * 2);
            Gizmos.DrawRay(rayOrigin.position - Vector3.right * sightRange, Vector3.right * sightRange * 2);
            DebugExtension.DrawCircle(rayOrigin.position, Vector3.forward, sightRange);
        }
    }
}
