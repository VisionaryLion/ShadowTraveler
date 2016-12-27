using UnityEngine;
using Entity;

namespace AI.Brain
{
    public enum RelationshipType { Friendly, Neutral, Hostile }
    public enum ActorType { Player, TestLight, TestDark}
    public class RelationshipMarker : MonoBehaviour
    {
        public ActorType actorType;
        [SerializeField]
        Relationship[] relationships;
        [SerializeField]
        StandardRelationship[] standardRelationship;
        [SerializeField]
        RelationshipType fallbackRelationship;

        public RelationshipType GetRelationship(ActingEntity otherEntity)
        {
            foreach (Relationship rel in relationships)
            {
                if (rel.otherEntity == otherEntity)
                {
                    return rel.relationshipType;
                }
            }
            foreach (StandardRelationship rel in standardRelationship)
            {
                if (rel.otherEntity == otherEntity.RelationshipMarker.actorType)
                {
                    return rel.relationshipType;
                }
            }
            return fallbackRelationship;
        }

        [System.Serializable]
        public class Relationship
        {
            public ActingEntity otherEntity;
            public RelationshipType relationshipType;

            public Relationship(ActingEntity otherEntity, RelationshipType relType = RelationshipType.Friendly)
            {
                this.otherEntity = otherEntity;
                this.relationshipType = relType;
            }
        }

        [System.Serializable]
        public class StandardRelationship
        {
            public ActorType otherEntity;
            public RelationshipType relationshipType;

            public StandardRelationship(ActorType otherEntity, RelationshipType relType = RelationshipType.Friendly)
            {
                this.otherEntity = otherEntity;
                this.relationshipType = relType;
            }
        }
    }
}
