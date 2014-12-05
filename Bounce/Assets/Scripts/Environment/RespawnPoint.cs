using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D col){
		if (col.tag == "Player") {
			Death.respawn = this.gameObject.transform.position;
			Death.camConfig = GameObject.FindGameObjectWithTag("MainCamera").
				GetComponent<CameraFollow>().GetConfig();
		}
	}
}
