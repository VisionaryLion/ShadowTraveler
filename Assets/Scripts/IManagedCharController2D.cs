using UnityEngine;
using System.Collections;

namespace FakePhysics
{
    public interface IManagedCharController2D
    {
        void AddForce(Vector2 force);
        void AddConstantVelocity(Vector2 velocity);
        void RemoveConstantVelocity(Vector2 velocity);
        void AddVelocity(CharacterInput.GetVelocity velocity);
        void AddPlattformVelocity(CharacterInput.GetVelocity velocity);
        Vector2 Velocity { get; }
    }
}
