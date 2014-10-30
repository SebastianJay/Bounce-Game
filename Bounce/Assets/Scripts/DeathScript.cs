using UnityEngine;
using System.Collections.Generic;

public class DeathScript : MonoBehaviour {

	public static Vector2 respawn;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.name.Equals ("ball")) {
			Debug.Log("YOU ARE DEAD");
			col.transform.position = respawn;
		}
	}
}