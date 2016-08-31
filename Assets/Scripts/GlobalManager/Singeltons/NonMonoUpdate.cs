using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NonMonoUpdate : MonoBehaviour
{
    public static NonMonoUpdate GetInstance() { return instance; }
    static NonMonoUpdate instance;

    public delegate void OnUpdate();
    List<OnUpdate> updateFuncs;
    List<OnUpdate> fixedUpdateFuncs;

    void Awake()
    {
        updateFuncs = new List<NonMonoUpdate.OnUpdate>();
        fixedUpdateFuncs = new List<NonMonoUpdate.OnUpdate>();
        instance = this;
    }

    void Update()
    {
        foreach (OnUpdate func in updateFuncs)
            func.Invoke();
    }

    void FixedUpdate()
    {
        foreach (OnUpdate func in fixedUpdateFuncs)
            func.Invoke();
    }

    /// <summary>
    /// The supplied function will be called every Update from now on.
    /// </summary>
    public void AddUpdateTarget(OnUpdate func)
    {
        updateFuncs.Add(func);
    }

    /// <summary>
    /// The supplied function will be called every FixedUpdate from now on.
    /// </summary>
    public void AddFixedUpdateTarget(OnUpdate func)
    {
        fixedUpdateFuncs.Add(func);
    }

    public void RemoveUpdateTarget(OnUpdate func)
    {
        updateFuncs.Remove(func);
    }

    public void RemoveFixedUpdateTarget(OnUpdate func)
    {
        fixedUpdateFuncs.Remove(func);
    }
}
