using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level
	public static Vector2 respawn;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
			Debug.Log("YOU ARE DEAD");
			//insert transition
			col.transform.position = respawn;
		}
	}
}