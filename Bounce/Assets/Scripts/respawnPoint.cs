using UnityEngine;
using System.Collections;

public class respawnPoint : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D col){
		if (col.name.Equals ("ball")) {
			DeathScript.respawn = this.gameObject.transform.position;
		}
	}
}
