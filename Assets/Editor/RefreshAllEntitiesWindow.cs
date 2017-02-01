using UnityEngine;
using System.Collections;
using UnityEditor;

public class RefreshAllEntitiesWindow
{
    [MenuItem("Window/Entities/RefreshAll")]
    static void RefreshAllEntities()
    {
        Entities.Entity[] entities = GameObject.FindObjectsOfType<Entities.Entity>();
        foreach (var e in entities)
            e.Refresh();
    }
}