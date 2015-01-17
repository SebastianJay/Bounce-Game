using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {

	public int checkPointID = 0;

	public static Dictionary<int, Vector3> posCheckTable = new Dictionary<int, Vector3>();
	public static Dictionary<int, CameraFollowConfig> camCheckTable = new Dictionary<int,CameraFollowConfig>();

	private GameObject screenFadeObj;
	void Awake (){
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		if (!posCheckTable.ContainsKey (checkPointID)) {
			posCheckTable.Add (checkPointID, transform.position);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			if (!camCheckTable.ContainsKey(checkPointID) && GameObject.FindGameObjectWithTag("MainCamera") != null) {
				camCheckTable.Add (checkPointID, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().GetConfig());
			}
			if (other.gameObject.GetComponent<PlayerDataManager> () != null) 
			{
				PlayerDataManager m = other.GetComponent<PlayerDataManager>();
				if(!m.previousCheckpoints.ContainsKey(Application.loadedLevel))
					m.previousCheckpoints[Application.loadedLevel] = new List<int>();

				if(!m.previousCheckpoints[Application.loadedLevel].Contains (checkPointID))
					m.previousCheckpoints[Application.loadedLevel].Add (checkPointID);

				m.lastCheckpoint = checkPointID;
				m.lastLevel = Application.loadedLevel;

				m.saveCurrent();
			}
		}
	}
}
