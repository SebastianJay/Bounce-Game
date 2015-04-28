using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {

	public int checkPointID = 0;

	private GameObject screenFadeObj;
	private GameObject notifyObj;
	void Awake (){
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		notifyObj = GameObject.FindGameObjectWithTag ("NoteManager");
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			if (other.gameObject.GetComponent<PlayerDataManager> () != null) 
			{
				if (!PlayerDataManager.previousCheckpoints.Contains(checkPointID) && notifyObj != null) {
					notifyObj.GetComponent<NotificationManager>().PushMessage(
						"Added landmark \"" + ImmutableData.GetCheckpointData()[checkPointID].name +"\" to Map");
					if (audio != null)
						audio.Play ();
				}
				PlayerDataManager.previousCheckpoints.Add(checkPointID);
				PlayerDataManager.lastCheckpoint = checkPointID;
				PlayerDataManager.lastLevel = Application.loadedLevel;
			}
		}
	}
}
