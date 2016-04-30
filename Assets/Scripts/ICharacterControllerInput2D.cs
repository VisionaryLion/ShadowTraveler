using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

interface ICharacterControllerInput2D
{
    void ResetMovementVars();
    void AddConstantForce(Vector2 force);
    void AddForce(CharacterInput.GetForce force);
    void RemoveConstantForce(Vector2 force);
    void AddRelativeForce(CharacterInput.GetForce force);
}
