using UnityEngine;
using System.Collections;

public class DisplayInstructions : MonoBehaviour {
	
	public string instructions;
	
	// Update is called once per frame
	void OnGUI () {
		GUILayout.BeginArea(new Rect(20, 20, Screen.width-40, Screen.height-40));
		GUILayout.Box(instructions);
		GUILayout.EndArea();
	}
}
