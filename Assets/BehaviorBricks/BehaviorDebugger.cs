using UnityEngine;
using Entities;

public class BehaviorDebugger : MonoBehaviour {
#if UNITY_EDITOR
    [AssignEntityAutomaticly]
    ActingEntity entity;

    string category;

    void Start()
    {
        GetComponent<BehaviorExecutor>().GetWorker().GetDebugger().SetExecutorObserver(new ExecutorDebugger.ExecutorObserver(Observer));
        category = entity.name + ": BT";
    }

	void Observer (Pada1.BBCore.Nodes.Node node, Pada1.BBCore.Tasks.TaskStatus status) {
        DebugPanel.Log("Node", category, node.id);
        DebugPanel.Log("TaskStatus", category, status);
    }

#endif
}
