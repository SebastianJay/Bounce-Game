using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {

	public int checkPointID = 0;

	public static Dictionary<int,Vector3> checkpointTable = new Dictionary<int,Vector3>();
	

	void Awake (){
		if (!checkpointTable.ContainsKey (checkPointID)) {
			checkpointTable.Add (checkPointID, transform.position);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("Triggered");
		if (other.gameObject.GetComponent<PlayerDataManager> () != null) 
		{
			PlayerDataManager m = other.GetComponent<PlayerDataManager>();
			if(!m.previousCheckpoints.ContainsKey(Application.loadedLevel))
				m.previousCheckpoints[Application.loadedLevel] = new List<int>();

			if(!m.previousCheckpoints[Application.loadedLevel].Contains (checkPointID))
				m.previousCheckpoints[Application.loadedLevel].Add (checkPointID);

			m.lastCheckpoint = checkPointID;
			m.lastLevel = Application.loadedLevel;

			playerData dataToSave = new playerData();
			List<Entry> entries = new List<Entry>();
			foreach (int key in m.previousCheckpoints.Keys)
			{
				entries.Add (new Entry(key,m.previousCheckpoints[key]));
			}
			dataToSave.previousCheckpoints = entries;
			dataToSave.inventory = m.inventory.ToList();
			dataToSave.lastCheckpoint = m.lastCheckpoint;
			dataToSave.lastLevel = m.lastLevel;

			XmlSerialzer.Save(dataToSave);
			Debug.Log ("Saving");
		}
	}
}
