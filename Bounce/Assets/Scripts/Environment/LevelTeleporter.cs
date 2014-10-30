using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelTeleporter : MonoBehaviour {

	public int levelToTeleportTo = 0;
	public int targetID = 0;
	public static Dictionary<int,Vector3> teleporterTargetTable = new Dictionary<int, Vector3>();
	public static bool teleported = false;
	public static int teleportTarget = 0;



	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name.Equals ("ball")) {
			StartCoroutine (TimedTeleport ());
		}
	}

	IEnumerator TimedTeleport() {
		for (int i = 0; i < 20; i++) {
			//if (GameObject.FindGameObjectWithTag("background").audio.volume > 0.0f) {
				GameObject.FindGameObjectWithTag("background").audio.volume -= 0.05f;
				yield return new WaitForSeconds(0.05f);
			//}
		}

		teleported = true;
		teleportTarget = targetID;
		Application.LoadLevel(levelToTeleportTo);	
	}

}
