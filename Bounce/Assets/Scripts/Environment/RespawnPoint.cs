using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	private static GameObject lastCheckpoint;
	private GameObject screenFadeObj;
	private GameObject notifyObj;
	private GameObject camObj;
	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		notifyObj = GameObject.FindGameObjectWithTag ("NoteManager");
		camObj = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void OnTriggerEnter2D (Collider2D col){
		if (col.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			if (GetComponent<Checkpoint>() == null && notifyObj != null && lastCheckpoint != gameObject) {
				notifyObj.GetComponent<NotificationManager>().PushMessage(
					"Reached a checkpoint");
				if (audio != null)
					audio.Play ();
			}
			lastCheckpoint = gameObject;
			Death.respawn = this.gameObject.transform.position;
			Death.camConfig = camObj.GetComponent<CameraFollow>().GetConfig();
		}
	}
}
