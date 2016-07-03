using UnityEngine;
using System.Collections;

public class ShowBounds : MonoBehaviour {

    Bounds bounds;
    // Use this for initialization
    void Awake()
    {
        bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
    }

    void OnGUI()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(bounds.max);
        GUI.TextField(new Rect(pos.x, pos.y, 100, 25), bounds.max.ToString());

        pos = Camera.main.WorldToScreenPoint(bounds.min);
        GUI.TextField(new Rect(pos.x, pos.y, 100, 25), bounds.min.ToString());
    }
}
