using UnityEngine;
using System.Collections.Generic;
using System;

namespace Actors
{
    /// <summary>
    /// A faster replacement for Unitys GameObject.Find methods.
    /// In order to find an GameObject it has to be added to the database first.
    /// Added entities are grouped by a group string.
    /// </summary>
    public class ActorDatabase
    {

        private static ActorDatabase instance;

        public static ActorDatabase GetInstance()
        {
            if (instance == null)
                instance = new ActorDatabase();
            return instance;
        }

        Dictionary<Type, List<Actor>> database = new Dictionary<Type, List<Actor>>(10);

        /// <summary>
        /// Adds the entity to the database. It can now be found by search requests.
        /// Note that duplicate entries aren't removed automatically.
        /// </summary>
        /// <param name="group">The group in which the entity can be found.</param>
        /// <param name="entity">The entity to add.</param>
        public void AddActor(Actor actor)
        {
            List<Actor> list;
            if (database.TryGetValue(actor.GetType(), out list))
            {
                list.Add(actor);
            }
            else
            {
                list = new List<Actor>(5);
                list.Add(actor);
                database.Add(actor.GetType(), list);
            }
        }

        /// <summary>
        /// Removes the given entity from the given group.
        /// Will throw an exception, if the group isn't defined.
        /// </summary>
        public void RemoveEntity(Actor actor)
        {
            List<Actor> list;
            if (database.TryGetValue(actor.GetType(), out list))
            {
                for (int iActor = 0; iActor < list.Count; iActor++)
                {
                    if (list[iActor] == actor)
                    {
                        list.RemoveAt(iActor);
                        if (list.Count == 0)
                            database.Remove(actor.GetType());
                        return;
                    }
                }
            }
            else
                throw new System.Exception("No \""+actor.GetType()+"\" in the entity database could be found that matches \"" + actor + "\"");
        }

        public Actor[] Find(Type actorType)
        {
            List<Actor> list;
            if (database.TryGetValue(actorType, out list))
                return list.ToArray();
            return null;
        }

        public T FindFirst<T>() where T : Actor
        {
            List<Actor> list;
            if (database.TryGetValue(typeof(T), out list))
                return (T)list[0];
            return null;
        }

        public Actor FindByGroupAndTag(Type actorType, string tag)
        {
            List<Actor> list;
            if (database.TryGetValue(actorType, out list))
            {
                for (int íActor = 0; íActor < list.Count; íActor++)
                {
                    if (list[íActor].CompareTag(tag))
                        return list[íActor];
                }
            }
            return null;
        }

        public Actor FindByGroupAndLayer(Type actorType, int layer)
        {
            List<Actor> list;
            if (database.TryGetValue(actorType, out list))
            {
                for (int iActor = 0; iActor < list.Count; iActor++)
                {
                    if (list[iActor].gameObject.layer == layer)
                        return list[iActor];
                }
            }
            return null;
        }

        public Actor FindByGroupAndName(Type actorType, string name)
        {
            List<Actor> list;
            if (database.TryGetValue(actorType, out list))
            {
                for (int iActor = 0; iActor < list.Count; iActor++)
                {
                    if (list[iActor].gameObject.name == name)
                        return list[iActor];
                }
            }
            return null;
        }
    }
}
