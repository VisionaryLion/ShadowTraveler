using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Manager
{
    public class GameStateManager
    {

        static GameStateManager instance;
        public static GameStateManager Instance { get { return instance; } }

        public IGameState CurrentState { get { return stateStack.Peek(); } }

        private Stack<IGameState> stateStack;
        private UnityEventHook.OnEvent update;
        private UnityEventHook.OnEvent onDestroy;

        public GameStateManager()
        {
            instance = this;
            update = new UnityEventHook.OnEvent(Update);
            onDestroy = new UnityEventHook.OnEvent(OnDestroy);
            UnityEventHook.GetInstance().AddUpdateListener(update);
            UnityEventHook.GetInstance().AddOnDestroyListener(onDestroy);
            stateStack = new Stack<IGameState>(2);
        }

        void Update()
        {
            Debug.Assert(CurrentState != null, "No state assigned. This should never happen!");

            CurrentState.OnStateActive();
        }

        void OnDestroy()
        {
            stateStack.Clear();
            instance = null;
        }

        public void AssignDefaultState(IGameState target)
        {
            stateStack.Push(target);
        }

        /// <summary>
        /// Will try to focus on the supplied object. Will return false, if the current focus target doesn't allow the focus to change.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Returns true, if the supplied object could acquire focus and false if not.</returns>
        public void StartNewState(IGameState target)
        {
            CurrentState.OnStateEnd();
            stateStack.Push(target);
            CurrentState.OnStateStart();
            Debug.Log(CurrentState + " gained focus.");
        }

        public void EndCurrentState()
        {
            Debug.Assert(CurrentState != null, "No state assigned. This should never happen!");
            Debug.Assert(stateStack.Count > 1, "Couldn't end the current state, because there is no fallback state left");

            CurrentState.OnStateEnd();
            stateStack.Pop();
            CurrentState.OnStateStart();
            Debug.Log(CurrentState + " gained focus.");
        }
    }

    /// <summary>
    /// Has to be implemented in order to use the FocusManager.
    /// </summary>
    public interface IGameState
    {
        void OnStateStart();
        void OnStateEnd();
        /// <summary>
        /// Will be called every frame this object has focus. Replacement for Update.
        /// </summary>
        void OnStateActive();
    }
}
