using UnityEngine;
using System.Collections.Generic;
using System;

namespace Entities
{
    /// <summary>
    /// A faster replacement for Unitys GameObject.Find methods.
    /// In order to find an GameObject it has to be added to the database first.
    /// Added entities are grouped by a group string.
    /// </summary>
    public class EntityDatabase
    {

        private static EntityDatabase instance;

        public static EntityDatabase GetInstance()
        {
            if (instance == null)
                instance = new EntityDatabase();
            return instance;
        }

        DatabaseNode rootNode;
        List<Entity> tmpGarbageList;

        EntityDatabase()
        {
            rootNode = new DatabaseNode(null, typeof(Entity));
            tmpGarbageList = new List<Entity>(20);
        }

        /// <summary>
        /// Adds the entity to the database. It can now be found by search requests.
        /// Note that duplicate entries aren't removed automatically.
        /// </summary>
        /// <param name="group">The group in which the entity can be found.</param>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity)
        {
            rootNode.AddEntity(entity);
        }

        /// <summary>
        /// Removes the given entity from the given group.
        /// Will throw an exception, if the group isn't defined.
        /// </summary>
        public void RemoveEntity(Entity entity)
        {
            rootNode.RemoveEntity(entity);

        }

        public void Find<T>(ref List<Entity> inoutResult) where T : Entity
        {
            rootNode.FindSubEntities(typeof(T), ref inoutResult);
        }

        public T FindFirst<T>() where T : Entity
        {
            List<Entity> list = rootNode.FindExactType(typeof(T));
            if (list == null || list.Count == 0)
                throw new Exception("Couldn't find Type in EntityDatabase");
            return (T)list[0];

        }

        public Entity FindByGameObject(Type requiredSuperType, GameObject gameObject)
        {
            tmpGarbageList.Clear();
            rootNode.FindSubEntities(requiredSuperType, ref tmpGarbageList);
            foreach (var entity in tmpGarbageList)
            {
                if (entity.gameObject == gameObject)
                    return entity;
            }
            return null;
        }

        class DatabaseNode
        {
            Type entityType;
            DatabaseNode parent;
            List<DatabaseNode> children;
            List<Entity> entities;

            public DatabaseNode(DatabaseNode parent, Type entityType)
            {
                this.entityType = entityType;
                this.parent = parent;
            }

            public DatabaseNode(DatabaseNode parent, Entity other)
            {
                this.entityType = other.GetType();
                this.parent = parent;
                entities = new List<Entity>(5);
                entities.Add(other);
            }

            public void AddEntity(Entity other)
            {
                AddEntityIntern(other);
            }

            public void RemoveEntity(Entity other)
            {
                DatabaseNode targetNode;

                FindNodeOfType(other.GetType(), out targetNode);
                if (targetNode == null)
                    throw new System.Exception("No \"" + other.GetType() + "\" in the entity database could be found that matches \"" + other + "\"");

                targetNode.entities.Remove(other);

                //Clean up branches - neccessary??
                while ((targetNode.entities == null || targetNode.entities.Count == 0) && targetNode.parent != null && (targetNode.children == null || targetNode.children.Count == 0))
                {
                    targetNode.parent.children.Remove(targetNode);
                    targetNode = targetNode.parent;
                }
            }

            public List<Entity> FindExactType(Type entityType)
            {
                DatabaseNode targetNode;
                FindNodeOfType(entityType, out targetNode);
                if (targetNode == null)
                    return null;
                return targetNode.entities;
            }

            public void FindSubEntities(Type entityType, ref List<Entity> inoutResult)
            {
                DatabaseNode targetNode;
                FindNodeOfType(entityType, out targetNode);
                if (targetNode == null)
                    return;
                targetNode.AddContentToList(ref inoutResult);
            }

            bool AddEntityIntern(Entity other)
            {
                Type otherType = other.GetType();

                if (otherType.Equals(entityType))
                {
                    if (entities == null)
                        entities = new List<Entity>(5);
                    entities.Add(other);
                    return true;
                }
                else if (otherType.IsSubclassOf(entityType))
                {
                    if (otherType.BaseType.Equals(entityType))
                    {
                        if (children == null)
                        {
                            children = new List<DatabaseNode>(2);
                            children.Add(new DatabaseNode(this, other));
                        }
                        else
                        {
                            foreach (var c in children)
                            {
                                if (c.AddEntityIntern(other))
                                    return true;
                            }
                            children.Add(new DatabaseNode(this, other));
                        }
                        return true;
                    }
                    else
                    {
                        Type nextInLine = otherType;
                        do
                        {
                            nextInLine = nextInLine.BaseType;
                        } while (!nextInLine.BaseType.Equals(entityType));

                        if (children == null)
                        {
                            children = new List<DatabaseNode>(2);
                        }
                        else
                        {
                            foreach (var c in children)
                            {
                                if (c.entityType.Equals(nextInLine))
                                {
                                    c.AddEntityIntern(other);
                                    return true;
                                }
                            }
                        }
                        DatabaseNode newNode = new DatabaseNode(this, nextInLine);
                        children.Add(newNode);
                        newNode.AddEntityIntern(other);
                        return true;
                    }
                }
                return false;
            }

            bool FindNodeOfType(Type typeToFind, out DatabaseNode node)
            {
                if (typeToFind.Equals(entityType))
                {
                    node = this;
                }
                else if (typeToFind.IsSubclassOf(entityType))
                {
                    if (children == null)
                    {
                        node = null;
                    }
                    else
                    {
                        foreach (var c in children)
                        {
                            if (c.FindNodeOfType(typeToFind, out node))
                            {
                                return true;
                            }
                        }
                        node = null;
                    }
                }
                else
                {
                    node = null;
                    return false;
                }
                return true;
            }

            void AddContentToList(ref List<Entity> inoutResult)
            {
                if (entities != null)
                    inoutResult.AddRange(entities);

                if (children == null)
                    return;

                foreach (var c in children)
                {
                    c.AddContentToList(ref inoutResult);
                }
            }
        }
    }
}
