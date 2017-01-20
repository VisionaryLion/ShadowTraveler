using UnityEngine;
using System.Collections;
using UnityEditor;

public class RefreshAllEntitiesWindow
{

    [MenuItem("Window/Entities/RefreshAll")]
    static void RefreshAllEntities()
    {
        Actors.Actor[] entities = GameObject.FindObjectsOfType<Actors.Actor>();
        foreach (var e in entities)
            e.Refresh();
    }
}