using UnityEngine;
using System.Collections;

public class ForceField : MonoBehaviour {

    public Vector2 force;
    public bool linearLess;
    public Vector2 anker;
    public float length;
	
	void OnTriggerEnter2D (Collider2D other) {
        ICharacterControllerInput2D iInput = other.GetComponent<ICharacterControllerInput2D>();
        if (iInput != null)
        {
            iInput.AddConstantForce(force);
        }
	}

    void OnTriggerExit2D(Collider2D other)
    {
        ICharacterControllerInput2D iInput = other.GetComponent<ICharacterControllerInput2D>();
        if (iInput != null)
        {
            iInput.RemoveConstantForce(force);
        }
    }

    void OnDrawGizmos()
    {
        if (linearLess)
        {
            Gizmos.DrawRay(anker, force * length);
        }
    }
}
