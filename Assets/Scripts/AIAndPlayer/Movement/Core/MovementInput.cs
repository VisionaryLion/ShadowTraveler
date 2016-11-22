using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CC2D
{
    public class MovementInput
    {
        public bool jump;
        public bool isJumpConsumed;
        public float timeOfLastJumpStateChange;
        public float horizontalRaw; //The input for the horizontal axis
        public float verticalRaw; //The input for the vertical axis
        public float horizontal; //The input for the horizontal axis
        public float vertical; //The input for the vertical axis

<<<<<<< HEAD
        /// <summary>
        /// Toggle crouching
        /// </summary>
        public bool toggleCrouch;

        List<InputEvent> InputEvents = new List<InputEvent>();

=======
        List<InputEvent> InputEvents = new List<InputEvent>();

        public void ResetToNeutral()
        {
            jump = false;
            isJumpConsumed = false;
            timeOfLastJumpStateChange = 0;
            horizontalRaw = 0;
            verticalRaw = 0;
            horizontal = 0;
            vertical = 0;
            InputEvents.Clear();
        }

>>>>>>> refs/remotes/origin/master
        internal void AddEvent(InputEvent newEvent)
        {
            if (newEvent.AllowMultiple == false)
            {
                InputEvents.RemoveAll(f => f.GetType() == newEvent.GetType());
            }
            InputEvents.Add(newEvent);
        }

<<<<<<< HEAD
        public MovementInput()
=======
        internal IEnumerable<InputEvent> GetEvents()
>>>>>>> refs/remotes/origin/master
        {
            for (int position = 0; position < InputEvents.Count; position++)
            {
                var inputEvent = InputEvents[position];
                if (inputEvent.ExpirationTime < Time.time)
                    continue;
                yield return inputEvent;
            }
            InputEvents.Clear();
        }

        internal InputEvent GetNextEvent()
        {
            while (InputEvents.Any())
            {
                var inputEvent = InputEvents[0];
                if (inputEvent.ExpirationTime < Time.time)
                {
                    InputEvents.RemoveAt(0);
                    continue;
                }
                return inputEvent;
            }
            return null;
        }

        internal void ConsumeEvent(InputEvent inputEvent)
        {
            InputEvents.Remove(inputEvent);
        }
    }

    internal abstract class InputEvent
    {
        internal bool AllowMultiple;
        /// <summary>
        /// Amount of time this event will live in seconds
        /// </summary>
        private float LiveTime = .3f;
        /// <summary>
        /// Time until event expires, in milliseconds
        /// </summary>
        internal float ExpirationTime;

        internal InputEvent(float liveTime) : this()
        {
            LiveTime = liveTime;
        }
        internal InputEvent()
        {
            ExpirationTime = Time.time + LiveTime;
        }
    }

    internal class JumpEvent : InputEvent
    {
        internal JumpEvent(float maxJumpExecutionDelay) : base(maxJumpExecutionDelay)
        {
<<<<<<< HEAD
            return !isJumpConsumed && !toggleCrouch && (jump || Time.time - timeOfLastJumpStateChange <= maxDelay);
        }

        internal IEnumerable<InputEvent> GetEvents()
        {
            for (int position = 0; position < InputEvents.Count; position++)
            {
                var inputEvent = InputEvents[position];
                if (inputEvent.ExpirationTime < Time.time)
                    continue;
                yield return inputEvent;
            }
            InputEvents.Clear();
        }

        internal InputEvent GetNextEvent()
        {
            while (InputEvents.Any())
            {
                var inputEvent = InputEvents[0];
                if (inputEvent.ExpirationTime < Time.time)
                {
                    InputEvents.RemoveAt(0);
                    continue;
                }
                return inputEvent;
            }
            return null;
        }

        internal void ConsumeEvent(InputEvent inputEvent)
        {
            InputEvents.Remove(inputEvent);
        }
    }

    internal abstract class InputEvent
    {
        internal bool AllowMultiple;
        /// <summary>
        /// Amount of time this event will live in milliseconds
        /// </summary>
        private float LiveTime = 300f;
        /// <summary>
        /// Time until event expires, in milliseconds
        /// </summary>
        internal float ExpirationTime;

        internal InputEvent(float liveTime) : this()
        {
            LiveTime = liveTime;
        }
        internal InputEvent()
        {
            ExpirationTime = Time.time + LiveTime / 1000f;
        }
    }

    internal class JumpEvent : InputEvent
    {
        internal JumpEvent() : base()
        {
            AllowMultiple = true;
        }
    }

=======
            AllowMultiple = true;
        }
    }

>>>>>>> refs/remotes/origin/master
    internal class CrouchEvent : InputEvent
    {
    }
}
