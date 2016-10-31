using UnityEngine;
using System.Collections;
using NavMesh2D;

public class NavData2DManager : MonoBehaviour {

    private static NavData2DManager instance;
    public static NavData2DManager Instance { get { return instance; } }


    public NavigationData2D NavData2D { get { return navData; } }

    [SerializeField]
    NavigationData2D navData;

    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }
}
