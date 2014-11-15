using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level!
	public static Vector2 respawn;

	private Collider2D playerCol;
	private bool locked = false;
	private GameObject screenFadeObj;

	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		CheckForDeath(collision.collider);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		CheckForDeath(col);
	}

	void CheckForDeath(Collider2D col)
	{
		if (col.tag == "Player" && !locked) {

			playerCol = col;
			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(DeathTransition);
			}
			else
				DeathTransition();
		}
	}

	void DeathTransition()
	{
		playerCol.transform.position = respawn;
		playerCol.rigidbody2D.velocity = Vector2.zero;
		playerCol.GetComponent<PowerupManager>().EndPowerup();
		// do other possible resets!
		locked = false;
	}
}