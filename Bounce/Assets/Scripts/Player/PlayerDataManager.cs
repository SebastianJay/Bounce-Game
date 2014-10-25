using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {

	// Use this for initialization
	public int lastCheckpoint = 0;
	public int lastLevel = 0;
	public bool debugNoLoad = false;
	public Dictionary<int,List<int>> previousCheckpoints = new Dictionary<int,List<int>>();
	public Inventory inventory = new Inventory();
	public HashSet<string> gameConstants = new HashSet<string>();

	public static bool loadedLevel = false;
	public static int initialLevel = 0;

	void Start () {
		LoadCurrentSave ();
	}

	public void LoadCurrentSave()
	{
		playerData myData = XmlSerialzer.Load ();
		if(myData != null && !loadedLevel && !debugNoLoad)
		{
			lastLevel = myData.lastLevel;
			initialLevel = lastLevel;
			Application.LoadLevel(myData.lastLevel);
			loadedLevel = true;
			
			
		}
	}

	void OnLevelWasLoaded(int level)
	{

		playerData myData = XmlSerialzer.Load ();
		if (myData != null && loadedLevel && level == initialLevel)
		{
			lastLevel = myData.lastLevel;
			
			lastCheckpoint = myData.lastCheckpoint;
			
			Vector3 myPos = Vector3.zero;
			Checkpoint.checkpointTable.TryGetValue(lastCheckpoint,out myPos);
			
			transform.position = myPos;
			
			List<Entry> entries = myData.previousCheckpoints;
			previousCheckpoints.Clear();
			foreach (Entry e in entries)
			{
				previousCheckpoints[e.key] = e.value;
			}
			
			inventory.Load(myData.inventory);
		}


		if (LevelTeleporter.teleported) {
			LevelTeleporter.teleported = false;
			transform.position = LevelTeleporter.teleporterTargetTable[LevelTeleporter.teleportTarget];
		}
	}

}
