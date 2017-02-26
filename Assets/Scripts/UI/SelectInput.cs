using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectInput : MonoBehaviour {

	[SerializeField]
	EventSystem eventSystem;
	[SerializeField]
	GameObject selectedObject;

	bool buttonSelected;

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetAxisRaw ("Vertical") != 0 && buttonSelected == false)
		{
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}
	}

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
