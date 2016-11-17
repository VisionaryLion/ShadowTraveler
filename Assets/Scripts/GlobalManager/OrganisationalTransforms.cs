using UnityEngine;
using System.Collections;

public class OrganisationalTransforms : MonoBehaviour
{
    static OrganisationalTransforms instance;
    public static OrganisationalTransforms Instance { get { return instance; } }

    public Transform DroppedItemRoot;

    void Awake()
    {
        instance = this;
    }

}
