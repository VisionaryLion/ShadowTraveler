using UnityEngine;
using Entities;
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

        List<Entity> queueryResultBuffer;
        EntityDatabase actorDatabase;

        void Awake()
        {
            queueryResultBuffer = new List<Entity>(4);
        }

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
            queueryResultBuffer.Clear();
            actorDatabase.Find<ActingEntity>(ref queueryResultBuffer);
            float sightRangeSqared = sightRange * sightRange;
            ActingEntity ae;
            foreach (var e in queueryResultBuffer)
            {
                ae = (ActingEntity)e;
                if (this.entity.RelationshipMarker.GetRelationship(ae) != RelationshipType.Hostile)
                    continue;
                if ((e.transform.position - rayOrigin.position).sqrMagnitude + 1 > sightRangeSqared)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, e.transform.position - rayOrigin.position, sightRange);
                if (hit && hit.collider.gameObject == e.gameObject)
                {
                    inSight.Add(new Blackboard.OtherEntity(ae, Vector2.Distance(rayOrigin.position, this.entity.transform.position)));
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
