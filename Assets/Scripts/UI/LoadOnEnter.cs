using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnEnter : StateMachineBehaviour {

	public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SceneManager.LoadScene("UI");
	}
}
