using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityEventHook : MonoBehaviour
{
    public static UnityEventHook GetInstance() { return instance; }
    static UnityEventHook instance;

    public delegate void OnEvent();
    List<OnEvent> updateFuncs;
    List<OnEvent> fixedUpdateFuncs;
    List<OnEvent> onDestroy;

    void Awake()
    {
        updateFuncs = new List<OnEvent>();
        fixedUpdateFuncs = new List<OnEvent>();
        onDestroy = new List<OnEvent>();
        instance = this;
    }

    void Update()
    {
        foreach (OnEvent func in updateFuncs)
            func.Invoke();
    }

    void FixedUpdate()
    {
        foreach (OnEvent func in fixedUpdateFuncs)
            func.Invoke();
    }

    void OnDestroy()
    {
        foreach (OnEvent func in onDestroy)
            func.Invoke();
        updateFuncs.Clear();
        fixedUpdateFuncs.Clear();
        onDestroy.Clear();
    }

    /// <summary>
    /// The supplied function will be called every Update from now on.
    /// </summary>
    public void AddUpdateListener(OnEvent func)
    {
        updateFuncs.Add(func);
    }

    /// <summary>
    /// The supplied function will be called every FixedUpdate from now on.
    /// </summary>
    public void AddFixedUpdateListener(OnEvent func)
    {
        fixedUpdateFuncs.Add(func);
    }

    public void AddOnDestroyListener(OnEvent func)
    {
        onDestroy.Add(func);
    }

    public void RemoveUpdateListener(OnEvent func)
    {
        updateFuncs.Remove(func);
    }

    public void RemoveFixedUpdateListener(OnEvent func)
    {
        fixedUpdateFuncs.Remove(func);
    }

    public void RemoveOnDestroyListener(OnEvent func)
    {
        onDestroy.Remove(func);
    }

    public void DelayActionBySeconds(DelayedAction action, float time, object parameter)
    {
        StartCoroutine(DelayForSeconds(action, time, parameter));
    }

    public void DelayActionBySeconds(DelayedActionNoParameter action, float time)
    {
        StartCoroutine(DelayForSeconds(action, time));
    }

    public delegate void DelayedAction(object data);
    public delegate void DelayedActionNoParameter();
    /// <summary>
    /// Executes the given "action" a by "delay" specified number of fixed frames later.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="data">The data, that will be supplied to the "action" method.</param>
    /// <param name="delay">Determines how many fixed frames the "action" should be delayed.</param>
    /// <returns></returns>
    IEnumerator DelayForSeconds(DelayedAction action, float seconds, object data)
    {
        yield return new WaitForSeconds(seconds);
        action(data);
    }

    IEnumerator DelayForSeconds(DelayedActionNoParameter action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}
