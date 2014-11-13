using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level
	public static Vector2 respawn;

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
		if (col.tag == "Player") {
			Debug.Log("YOU ARE DEAD");
			//insert transition
			
			col.transform.position = respawn;
			col.rigidbody2D.velocity = Vector2.zero;
			col.GetComponent<PowerupManager>().EndPowerup();
			// do other possible resets!
		}
	}
}