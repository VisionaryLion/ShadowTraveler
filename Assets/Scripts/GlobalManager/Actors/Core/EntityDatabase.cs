using UnityEngine;
using System.Collections.Generic;
using System;

namespace Entity
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

        Dictionary<Type, List<Entity>> database = new Dictionary<Type, List<Entity>>(10);


        /// <summary>
        /// Adds the entity to the database. It can now be found by search requests.
        /// Note that duplicate entries aren't removed automatically.
        /// </summary>
        /// <param name="group">The group in which the entity can be found.</param>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity)
        {
            List<Entity> list;
            if (database.TryGetValue(entity.GetType(), out list))
            {
                list.Add(entity);
            }
            else
            {
                list = new List<Entity>(5);
                list.Add(entity);
                database.Add(entity.GetType(), list);
            }
        }

        /// <summary>
        /// Removes the given entity from the given group.
        /// Will throw an exception, if the group isn't defined.
        /// </summary>
        public void RemoveEntity(Entity entity)
        {
            List<Entity> list;
            if (database.TryGetValue(entity.GetType(), out list))
            {
                for (int iEntity = 0; iEntity < list.Count; iEntity++)
                {
                    if (list[iEntity] == entity)
                    {
                        list.RemoveAt(iEntity);
                        if (list.Count == 0)
                            database.Remove(entity.GetType());
                        return;
                    }
                }
            }
            else
                throw new System.Exception("No \"" + entity.GetType() + "\" in the entity database could be found that matches \"" + entity + "\"");
        }

        public Entity[] Find(Type entityType)
        {
            List<Entity> list;
            if (database.TryGetValue(entityType, out list))
                return list.ToArray();
            return new Entity[0];
        }

        public T[] Find<T>() where T : Entity
        {
            List<Entity> list;
            if (database.TryGetValue(typeof(T), out list))
                return (T[])list.ToArray();
            return new T[0];
        }

        public T FindFirst<T>() where T : Entity
        {
            List<Entity> list;
            if (database.TryGetValue(typeof(T), out list))
                return (T)list[0];
            return null;
        }

        public Entity FindByGroupAndTag(Type entityType, string tag)
        {
            List<Entity> list;
            if (database.TryGetValue(entityType, out list))
            {
                for (int iEntity = 0; iEntity < list.Count; iEntity++)
                {
                    if (list[iEntity].CompareTag(tag))
                        return list[iEntity];
                }
            }
            return null;
        }

        public Entity FindByGroupAndLayer(Type entityType, int layer)
        {
            List<Entity> list;
            if (database.TryGetValue(entityType, out list))
            {
                for (int iEntity = 0; iEntity < list.Count; iEntity++)
                {
                    if (list[iEntity].gameObject.layer == layer)
                        return list[iEntity];
                }
            }
            return null;
        }

        public Entity FindByGroupAndName(Type entityType, string name)
        {
            List<Entity> list;
            if (database.TryGetValue(entityType, out list))
            {
                for (int iEntity = 0; iEntity < list.Count; iEntity++)
                {
                    if (list[iEntity].gameObject.name == name)
                        return list[iEntity];
                }
            }
            return null;
        }

        public Entity FindByGameObject(GameObject gameObject)
        {
            foreach (var pair in database)
            {
                foreach (var entity in pair.Value)
                {
                    if (entity.gameObject == gameObject)
                        return entity;
                }

            }
            return null;
        }

        public Entity FindByGameObject(Type requiredSuperType, GameObject gameObject)
        {
            foreach (var pair in database)
            {
                if (!pair.Key.IsSubclassOf(requiredSuperType))
                    continue;

                foreach (var entity in pair.Value)
                {
                    if (entity.gameObject == gameObject)
                        return entity;
                }

            }
            return null;
        }
    }
}
