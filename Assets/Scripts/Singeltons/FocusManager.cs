using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Manager
{
    public class FocusManager
    {

        static FocusManager instance;
        public static FocusManager GetInstance() { if (instance == null) instance = new FocusManager(); return instance; }

        public IFocusable CurrentObjInFocus { get { return currentTarget; } }

        private IFocusable currentTarget;

        public FocusManager()
        {
            NonMonoUpdate.GetInstance().AddUpdateTarget(new NonMonoUpdate.OnUpdate(Update));
        }

        void Update()
        {
            if (currentTarget != null)
                currentTarget.OnFocus();
        }

        /// <summary>
        /// Will try to focus on the supplied object. Will return false, if the current focus target doesn't allow the focus to change.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Returns true, if the supplied object could acquire focus and false if not.</returns>
        public bool RequestFocus(IFocusable target)
        {
            if (currentTarget.LockedFocus())
                return false;

            currentTarget.OnLostFocus();
            currentTarget = target;
            currentTarget.OnGainFocus();
            Debug.Log(currentTarget+" gained focus.");
            return true;
        }
        /// <summary>
        /// Forces the gain of focus. It's guaranteed, that the supplied object gets focus.
        /// </summary>
        public void ForceFocus(IFocusable target)
        {
            currentTarget.OnLostFocus();
            currentTarget = target;
            currentTarget.OnGainFocus();
            Debug.Log(currentTarget + " forced focus.");
        }

        /// <summary>
        /// Check, if the supplied object has focus-
        /// </summary>
        /// <param name="target">The object to check against</param>
        /// <returns>True, if the supplied object has focus.</returns>
        public bool HasFocus(IFocusable target)
        {
            return currentTarget == target;
        }
    }

    /// <summary>
    /// Has to be implemented in order to use the FocusManager.
    /// </summary>
    public interface IFocusable
    {
        void OnGainFocus();
        void OnLostFocus();
        /// <summary>
        /// Will be called every frame this object has focus. Replacement for Update.
        /// </summary>
        void OnFocus();

        /// <summary>
        /// Return true, if you don't want to another object to gain focus. (Can be overridden by ForceFocus) Is only called, when this object has focus.
        /// </summary>
        bool LockedFocus();
    }
}
