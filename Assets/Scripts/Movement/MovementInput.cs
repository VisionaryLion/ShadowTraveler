using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC2D { 
    public class MovementInput
    {
        public bool jump;
        public float timeOfJumpButtonDown;
        public float horizontalRaw; //The input for the horizontal axis
        public float verticalRaw; //The input for the vertical axis
    }
}
