using UnityEngine;
using System.Collections;
//imports all the UI code for unity
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	//creates score variable that is accessible by anyone else without having to go to the score manager
	public static int score;

	Text text;

	void Start()
	{
		//find text object in the game
		text = GetComponent<Text> ();

		score = 0;
	}

	void Update()
	{
		//make sure you there cannot be a score less than zero
		if (score < 0)
			score = 0;

		//puts the score on the screen
		text.text = "" + score;
	}
		
	public static void AddPoints (int pointsToAdd)
	{
		//add points to the score
		score += pointsToAdd;
	}

	//public static void SubtractPoints (int pointsToSubtract)
	//{
		//subtract points to the score
		//score -= pointsToSubtract;
	//}

	//reset score of player, for example if they die
	public static void Reset()
	{
		score = 0;
	}
}
