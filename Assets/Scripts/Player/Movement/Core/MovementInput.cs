using UnityEngine;

namespace CC2D { 
    public class MovementInput
    {
        public bool jump;
        public bool isJumpConsumed;
        public float timeOfLastJumpStateChange;
        public float horizontalRaw; //The input for the horizontal axis
        public float verticalRaw; //The input for the vertical axis
        public float horizontal; //The input for the horizontal axis
        public float vertical; //The input for the vertical axis

        public MovementInput()
        {
            timeOfLastJumpStateChange = float.MinValue;// Make sure it cant be bigger then any delay.
        }

        /// <summary>
        /// Evaluate, if a jump press is valid, taken the maxDelay and if the jump input was already consumed into account.
        /// </summary>
        /// <param name="maxDelay">The max delay between a jump input and its execution.</param>
        /// <returns></returns>
        public bool ShouldJump(float maxDelay)
        {
            return !isJumpConsumed && (jump || Time.time - timeOfLastJumpStateChange <= maxDelay);
        }
    }
}
