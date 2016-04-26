using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

interface ICharacterControllerInput2D
{
    void SetNormalMovementVars(float gravity, float horizontalSpeed, float horizontalAcceleration, float verticalSpeed, float verticalAcceleration, float airSpeed, float airAcceleration, float jumpSpeed);
    void ResetMovementVars();
    void ResetRestrictions();
    void AddConstantForce(Vector2 force);
    void AddForce(CharacterInput.GetForce force);
    void RemoveConstantForce(Vector2 force);
    void AddRelativeForce(CharacterInput.GetForce force);
}
